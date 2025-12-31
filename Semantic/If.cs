using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Parser.Models;

namespace LadonLang.Semantic
{
    public partial class SemanticAnalyzer 
    {
        private void VisitIf(IfStmt node)
        {
            //nammed sentence register
            if (node.Name != null)
            {
                var sym = new NamedSentenceSymbol(
                    Name: node.Name.Lexeme,
                    DeclToken: node.Name
                );
                if (!_symbols.Current.TryDeclare(sym, out var err))
                    throw err!;
            }

            //analyze condtion
            var condType = VisitExpr(node.Condition);
            if (!IsBooleanType(condType))
                throw new ParserExeption(
                    "La condición de un <if> debe ser booleana.",
                    TokenFromExpr(node.Condition)
                );

            // then block
            _symbols.Push("if:then");
            VisitBlock(node.ThenBlock);
            _symbols.Pop();

            // elifs blocks
            foreach (var elif in node.ElifStmtList)
            {
                var elifType = VisitExpr(elif.Condition);
                if (!IsBooleanType(elifType))
                    throw new ParserExeption(
                        "<elif> condition should be boolean",
                        TokenFromExpr(elif.Condition)
                    );

                _symbols.Push("if:elif");
                VisitBlock(elif.Block);
                _symbols.Pop();
            }

            // else block if exists
            if (node.ElseStmt != null){
                _symbols.Push("if:else");
                VisitBlock(node.ElseStmt.Block);
                _symbols.Pop();
            }
        }
    }
}