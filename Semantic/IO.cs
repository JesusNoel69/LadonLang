using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Parser.Models;

namespace LadonLang.Semantic
{
    public partial class SemanticAnalyzer
    {
        private void VisitInput(InputStmt node)
        {
            // si es set[var]
            if (node.SetVariable != null)
            {
                var sym = _symbols.Current.Resolve(node.SetVariable.Lexeme);
                if (sym is not VariableSymbol vs)
                    throw new UndeclaredSymbolException(node.SetVariable);
            }
        }
        private void VisitOutput(OutputStmt node)
        {
            if (node.GetVariable != null)
            {
                var sym = _symbols.Current.Resolve(node.GetVariable.Lexeme);
                if (sym is not VariableSymbol)
                    throw new UndeclaredSymbolException(node.GetVariable);
            }

            if (node.PrintList != null)
            {
                foreach (var e in node.PrintList)
                    _ = VisitExpr(e);
            }
        }
    }
}