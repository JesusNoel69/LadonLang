using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
        private string RenderVarDecl(VarDeclStmt declaration)
        {
            var sb = new System.Text.StringBuilder();
            //in this version only have 1 type but foreach already prepared for future
            declaration.Decl.Declarators.ForEach(d =>
            {
                var type = string.Join("", d.TypeArguments.Select(t => t.BaseTypeKeyword.Lexeme));
                
                var identifier = d.Identifier.Lexeme;
                if (type == "string")
                {
                    //in c
                    /*
                    type = "char";
                    identifier+="[]";
                    */
                }
                // initializer
                string initText = "";
                if (d.Initializer is not null && d.Initializer.Expressions.Count > 0)
                {
                    initText = string.Join(", ", d.Initializer.Expressions.Select(RenderExpr));
                }
                var line = string.IsNullOrWhiteSpace(initText)
                    ? $"{type} {identifier};"
                    : $"{type} {identifier} = {initText};";
                //globalBlock+=line+"\n";
                sb.AppendLine(line);
            });
            return sb.ToString().TrimEnd();
        }
        private string RenderLiteral(object? value)
        {
            if (value is null) return "null";

            if (value is Token t)
            {
                if(t.TokenType=="STRING" || t.TokenType=="TEXT")
                {
                    return $"\"{t.Lexeme}\"";
                }
                return t.Lexeme;
            }

            return value switch
            {
                bool b => b ? "true" : "false",
                string s => $"\"{s.Replace("\"", "\\\"")}\"",
                char c => $"'{(c == '\'' ? "\\'" : c.ToString())}'",
                _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "null"
            };
        }
        private string RenderAssign(AssignStmt a)
        {
            return $"{a.Variable.Lexeme} {a.AssignOperator.Lexeme} {RenderExpr(a.Assignment)};";
        }

    }
}