using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Semantic
{
    public partial class SemanticAnalyzer
    {
        private void VisitFunction(FunctionStmt fn)
        {
            int? outPos = fn.OutParameter?.Position;
            var paramSymbols = new List<VariableSymbol>();
            VariableSymbol? outParam = null;
            foreach (var p in fn.Parameters ?? new List<Parameter>())
            {
                // explicit optional type
                var allowed = new List<TypeRef>();
                if (p.Type != null)
                    allowed.Add(TypeRefFromToken(p.Type));
                // if have no explicit type it meant unkown at moment (or void)
                var ps = new VariableSymbol(
                    Name: p.Name.Lexeme,
                    DeclToken: p.Name,
                    AllowedTypes: allowed,
                    IsOutParam: p.IsOutParameter,
                    IsParameter: true
                );

                paramSymbols.Add(ps);
            }
            VariableSymbol? outParamSym = null;
            if (fn.OutParameter != null)
            {
                var p = fn.OutParameter;
                var allowed = new List<TypeRef>();
                if (p.Type != null)
                    allowed.Add(TypeRefFromToken(p.Type));

                outParamSym = new VariableSymbol(
                    Name: p.Name.Lexeme,
                    DeclToken: p.Name,
                    AllowedTypes: allowed,
                    IsOutParam: true,
                    IsParameter: true
                );
                outPos=p.Position;
            }
            //return types
            var returns = (fn.ReturnTypes ?? new List<Token>())
                .Select(TypeRefFromToken)
                .ToList();
            var funcSym = new FunctionSymbol(
                Name: fn.Name.Lexeme,
                DeclToken: fn.Name,
                Parameters: paramSymbols,
                OutParameter: outParam,
                ReturnTypes: returns,
                OutPosition: outPos
            );
            if (!_symbols.Current.TryDeclare(funcSym, out var err))
                throw new DuplicateSymbolException(fn.Name.Lexeme, fn.Name);
            //enter the scope
            _symbols.Push($"fn:{fn.Name.Lexeme}");
            foreach (var ps in funcSym.Parameters)
            {
                if (!_symbols.Current.TryDeclare(ps, out var perr))
                    throw perr!;
            }
            if (outParamSym != null)
            {
                if (!_symbols.Current.TryDeclare(outParamSym, out var oerr))
                    throw oerr!;
            }

            var previous = _currentFunction;
                _currentFunction = funcSym;
            if (fn.Block != null)
                VisitBlock(fn.Block);
            _currentFunction = previous;
            _symbols.Pop();
        }
        private void VisitBlock(BlockStmt block)
        {
            foreach (var stmt in block.Statements)
                //VisitStatement(stmt);
                Analyze(stmt);
        }
        private void VisitReturn(ReturnStmt stmt)
        {
            if (_currentFunction == null)
                throw new ParserExeption("Return statement is only allowed inside a function.", stmt.ReturnKeyWord);
            //today expr is null
            if (stmt.Expression != null)
            {
                throw new ParserExeption("Return expressions are not supported yet. Use <return/> only.", stmt.ReturnKeyWord);
            }
        }

    }
}