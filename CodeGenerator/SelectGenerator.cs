using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
        private string RenderSelect(SelectStmt select)
        {
            var sb = new System.Text.StringBuilder();
            if(select.Name != null)
            {
                //make function not yet
            }
            //maybe onsider use a expr in a future
            var switchExpr = select.Value.Lexeme;
            if (!CanUseCSwitch(select.Value))
            {
                // fallback if/else
                return RenderSelectAsIfElse(select);
            }

            sb.Append("switch(").Append(switchExpr).AppendLine(")")
            .AppendLine("{");

            //bool hasDefault = false;

            foreach (var option in select.Options)
            {
                if (option.IsDefault || option.Value is null)
                {
                    //hasDefault = true;
                    sb.AppendLine("default:");
                }
                else
                {
                    var caseValue = RenderCaseValue(option.Value);
                    sb.Append("case ").Append(caseValue).AppendLine(":");
                }
                sb.Append(RenderBlockWithoutCurly(option.Block));
                sb.AppendLine("break;");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
        private string RenderSelectAsIfElse(SelectStmt select)
        {
            var sb = new System.Text.StringBuilder();
            var expr = select.Value.Lexeme;
            bool isString = select.Value.TokenType is "STRING" or "TEXT";
            bool first = true;

            foreach (var option in select.Options)
            {
                if (option.IsDefault || option.Value is null)
                {
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.Append(RenderBlockWithoutCurly(option.Block));
                    sb.AppendLine("}");
                    continue;
                }

                string condition;
                if (isString)
                {
                    //expr should be char* or char[]; for local array use char*
                    var v = option.Value.Lexeme;
                    var lit = v.StartsWith("\"") ? v : $"\"{v}\"";
                    condition = $"strcmp({expr}, {lit}) == 0";
                }
                else
                {
                    // float/double/others
                    condition = $"{expr} == {option.Value.Lexeme}";
                }
                sb.Append(first ? "if(" : "else if(")
                .Append(condition).AppendLine(")")
                .AppendLine("{");

                sb.Append(RenderBlockWithoutCurly(option.Block));
                sb.AppendLine("}");
                first = false;
            }
            return sb.ToString();
        }

        private bool CanUseCSwitch(Token t)
        {
            return t.TokenType switch
            {
                "INTEGER_NUMBER" => true,
                "CHARACTER"      => true,
                "TRUE_KEYWORD"   => true,
                "FALSE_KEYWORD"  => true,
                //for simpliity use any identifier with if/else
                "IDENTIFIER"     => false, // resolve in symbol table and verify is allowed
                _ => false
            };
        }
        private string RenderCaseValue(Token t)
        {
            return t.TokenType switch
            {
                "CHARACTER" => t.Lexeme.StartsWith("'") ? t.Lexeme : $"'{t.Lexeme}'",
                "TRUE_KEYWORD" => "true",
                "FALSE_KEYWORD" => "false",
                _ => t.Lexeme
            };
        }
        private string RenderBlockWithoutCurly(BlockStmt block)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var s in block.Statements)
                sb.AppendLine(RenderStmt(s));
            return sb.ToString();
        }
    }
}