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
        private void VisitSelect(SelectStmt node)
        {
            // Nammed sentnce register
            if (node.Name != null)
            {
                var sym = new NamedSentenceSymbol(
                    Name: node.Name.Lexeme,
                    DeclToken: node.Name
                );

                if (!_symbols.Current.TryDeclare(sym, out var err))
                    throw err!;
            }

            // sentinel type
            var selectTok = node.Value;
            var selectType = TypeFromValueToken(selectTok);

            // validate options
            bool hasDefault = false;
            var seenValues = new HashSet<string>(); // detect duplicates

            foreach (var opt in node.Options ?? new List<OptionStmt>())
            {
                if (opt.IsDefault)
                {
                    if (hasDefault)
                        throw new ParserExeption("Only one <option default> is allowed in <select>.", selectTok);
                    hasDefault = true;
                }
                else
                {
                    if (opt.Value == null)
                        throw new ParserExeption("<option> without value must be marked as default.", selectTok);
                    var key = $"{opt.Value.TokenType}:{opt.Value.Lexeme}";
                    if (!seenValues.Add(key))
                        throw new ParserExeption($"Duplicate <option value={opt.Value.Lexeme}> in <select>.", opt.Value);

                    var optType = TypeFromValueToken(opt.Value);

                    EnsureComparable(
                        selectType,
                        optType,
                        opt.Value,
                        context: "<select>"
                    );
                }
                //option block
                _symbols.Push(opt.IsDefault
                    ? "select:default"
                    : $"select:case:{opt.Value!.Lexeme}");
                VisitBlock(opt.Block);
                _symbols.Pop();
            }
        }
        private TypeRef TypeFromValueToken(Token tok)
        {
            // Sentinel o option value pueden ser IDENTIFIER o literal
            if (tok.TokenType == "IDENTIFIER")
            {
                var sym = _symbols.Current.Resolve(tok.Lexeme);
                if (sym is not VariableSymbol vs)
                    throw new UndeclaredSymbolException(tok);

                if (vs.AllowedTypes.Count == 1) return vs.AllowedTypes[0];
                if (vs.AllowedTypes.Count > 1) return new TypeRef("unknown");//union
                return new TypeRef("unknown");
            }
            // Literal tokens = TRUE/FALSE/INTEGER/FLOAT/CHARACTER/STRING
            return TypeOfLiteral(tok);
        }

        private bool IsKnown(TypeRef t) => t.Name != "unknown" && t.Name != "union";
        private void EnsureComparable(TypeRef selectType, TypeRef optionType, Token blameTok, string context)
        {
            if (!IsKnown(selectType) || !IsKnown(optionType))
                return;
            if (IsCompatible(optionType, selectType) || IsCompatible(selectType, optionType))
                return;
            throw new TypeMismatchException(
                $"{context}: option value type '{optionType.Name}' is not comparable with select type '{selectType.Name}'.",
                blameTok
            );
        }
    }
}