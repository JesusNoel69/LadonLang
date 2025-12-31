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
        private void VisitLoop(LoopStmt node)
        {
            // Nammed sentence register
            if (node.Name != null)
            {
                var ns = new NamedSentenceSymbol(node.Name.Lexeme, node.Name);
                if (!_symbols.Current.TryDeclare(ns, out var err))
                    throw err!;
            }
            // scope
            _symbols.Push(node.Name != null ? $"loop:{node.Name.Lexeme}" : "loop");
            var header = node.LoopHeader;
            if (header != null)//infinite case
            {
                VisitLoopHeader(header);            
            }
            // might be null
            if (node.Block != null)
                VisitBlock(node.Block);
            _symbols.Pop();
        }

        private void VisitLoopHeader(LoopHeader header)
        {
            switch (header)
            {
                case LoopIn lin:
                    VisitLoopIn(lin);
                    break;
                case LoopOf lof:
                    VisitLoopOf(lof);
                    break;
                case LoopDo ldo:
                    VisitLoopDo(ldo);
                    break;
                case LoopIt lit:
                    VisitLoopIt(lit);
                    break;
                case null:
                    break;
                default:
                    throw new ParserExeption("Unknown loop header type", GetHeaderToken(header));
            }
        }

        private void VisitLoopIn(LoopIn lin)
        {
            // item in items
            EnsureIdentifierDeclared(lin.Iterable);
            // iterator local of loop
            DeclareLocalVariable(
                nameTok: lin.IteratorName,
                allowed: new List<TypeRef>() // unknown
            );
        }

        private void VisitLoopOf(LoopOf lof)
        {
            // item of items
            EnsureIdentifierDeclared(lof.Iterable);
            DeclareLocalVariable(
                nameTok: lof.EachName,
                allowed: new List<TypeRef>() // unknown
            );
        }

        private void VisitLoopDo(LoopDo ldo)
        {
            // <loop condicion;>  o <loop condicion; pass>
            // pconditionshould be null
            if (ldo.Condition != null)
            {
                var t = VisitExpr(ldo.Condition);
                if (!IsBooleanType(t) && t.Name != "unknown")
                    throw new TypeMismatchException("Loop condition must be boolean", GetExprToken(ldo.Condition));
            }
        }

        private void VisitLoopIt(LoopIt lit)
        {
            // <loop i=0; i<10; i=i+1;>

            if (lit.Init is AssignExpr initAssign)
            {
                var nameTok = initAssign.Name;
                var existing = _symbols.Current.Resolve(nameTok.Lexeme);

                if (existing is not VariableSymbol)
                {
                    var inferred = InferTypeFromExpr(initAssign.Value);
                    var allowed = inferred.Name == "unknown"
                        ? new List<TypeRef>()
                        : new List<TypeRef> { inferred };

                    var sym = new VariableSymbol(
                        Name: nameTok.Lexeme,
                        DeclToken: nameTok,
                        AllowedTypes: allowed
                    );

                    if (!_symbols.Current.TryDeclare(sym, out var err))
                        throw err!;
                }
                _ = VisitExpr(lit.Init);
            }
            else if (lit.Init != null)
            {
                _ = VisitExpr(lit.Init);
            }

            // should be null or unknown
            if (lit.Condition != null)
            {
                var ct = VisitExpr(lit.Condition);
                if (!IsBooleanType(ct) && ct.Name != "unknown")
                    throw new TypeMismatchException("Loop-it condition must be boolean", GetExprToken(lit.Condition));
            }
            //assign or expr
            if (lit.Step != null)
            {
                _ = VisitExpr(lit.Step);
            }
        }
        private void DeclareLocalVariable(Token nameTok, List<TypeRef> allowed)
        {
            var sym = new VariableSymbol(
                Name: nameTok.Lexeme,
                DeclToken: nameTok,
                AllowedTypes: allowed
            );

            if (!_symbols.Current.TryDeclare(sym, out var err))
                throw new DuplicateSymbolException(sym.Name, nameTok);
        }
        private void EnsureIdentifierDeclared(Token idTok)
        {
            var sym = _symbols.Current.Resolve(idTok.Lexeme);
            if (sym is null)
                throw new UndeclaredSymbolException(idTok);
        }
        private Token GetExprToken(Expr expr) =>
            expr switch
            {
                VariableExpr v => v.Name,
                AssignExpr a   => a.Name,
                UnaryExpr u    => u.Operator,
                BinaryExpr b   => b.Operator,
                LiteralExpr l  => l.Value as Token ?? DummyToken(),
                _              => DummyToken()
            };

        private Token GetHeaderToken(LoopHeader h) =>
            h switch
            {
                LoopIn x => x.IteratorName,
                LoopOf x => x.EachName,
                LoopDo _ => DummyToken(),
                LoopIt _ => DummyToken(),
                _ => DummyToken()
            };
        private Token DummyToken() => new Token(0, "", "", 0, 0);
        private TypeRef InferTypeFromExpr(Expr e)
        {
            if(e is LiteralExpr lit && lit.Value is Token tok)
            {
                return TypeOfLiteral(tok);
            }
            return new TypeRef("unknown");
        }
    }
}