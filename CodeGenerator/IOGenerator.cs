using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Parser.Models;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator 
    {
        private string RenderOutput(OutputStmt o)
        {
            var sb = new System.Text.StringBuilder();

            // <output get[x]/>
            if (o.GetVariable != null)
            {
                sb.Append("cout << ").Append(o.GetVariable.Lexeme).Append(" << \"\\n\";");
                return sb.ToString();
            }

            // <output> ... </output>
            sb.Append("cout");
            foreach (var e in o.PrintList)
                sb.Append(" << ").Append(RenderExpr(e));
            sb.Append(" << \"\\n\";");

            return sb.ToString();
        }
        private bool IsKeyInput(InputStmt i) => i.UseType; // type=[key]
        private char? StopKey(InputStmt i)
        {
            if (i.Key == null) return null;
            var lex = i.Key.Lexeme.Trim();
            if (lex.Length == 1) return lex[0];
            if (lex.Length == 3 && lex[0] == '\'' && lex[2] == '\'') return lex[1];
            return null;
        }
        private string RenderInput(InputStmt input)
        {
            // <input type=[key] .../> 
            if (input.UseType)
            {
                // if has st, try to save key inthat variable
                var setName = input.SetVariable?.Lexeme;

                // key='A' (Token) -> char
                string? stopChar = null;
                if (input.Key != null)
                {
                    var k = input.Key.Lexeme;
                    if (k.Length >= 1)
                    {
                        var c = k.Trim();
                        if (c.Length >= 3 && c[0] == '\'' && c[^1] == '\'') c = c.Substring(1, c.Length - 2);
                        stopChar = c.Length > 0 ? c[0].ToString() : null;
                    }
                }

                if (stopChar == null)
                {
                    if (!string.IsNullOrWhiteSpace(setName))
                    {
                        return ReadKeyAssignToVar(setName);
                    }
                    return "std::cin.get();";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(setName))
                    {
                        return
                            $@"
                            do {{ladon.ch = (char)std::cin.get(); }} while(ladon.ch != '{stopChar}');
                            {AssignCharToVar(setName, "_ch")}";
                    }

                    return
                        $@"
                        do {{ladon.ch = (char)std::cin.get(); }} while(ladon.ch != '{stopChar}');";
                }
            }

            // <input set[var]/>
            if (input.SetVariable != null)
            {
                var varName = input.SetVariable.Lexeme;
                return ReadLineAssignToVar(varName);
            }

            // <input/>
            return "std::string _tmp; std::getline(std::cin, _tmp);";
        }

        private string ReadLineAssignToVar(string varName)
        {
            var t = ResolveVarCType(varName);
            return t switch
            {
                CType.Int =>
                    $@"std::string _line;
                    std::getline(std::cin, _line);
                    {varName} = std::stoi(_line);",

                CType.Double =>
                    $@"std::string _line;
                    std::getline(std::cin, _line);
                    {varName} = std::stod(_line);",

                CType.Bool =>
                    $@"std::string _line;
                    std::getline(std::cin, _line);
                    {varName} = (_line == ""true"" || _line == ""1"");",

                CType.CharPtr => 
                    $@"std::getline(std::cin, {varName});",

                _ =>
                
                    $@"std::string _line;
                    std::getline(std::cin, _line);
                    // TODO: tipo desconocido para '{varName}', no se asignó"
            };
        }

        private string ReadKeyAssignToVar(string varName)
        {
            var t = ResolveVarCType(varName);

            if (t == CType.CharPtr)
            {
                return
                    $@"char _ch = (char)std::cin.get();
                    {varName} = std::string(1, _ch);";
            }
            return
                $@"char _ch = (char)std::cin.get();
                // TODO: asignación de tecla a '{varName}' (tipo: {t})
                ";
        }

        private string AssignCharToVar(string varName, string chExpr)
        {
            var t = ResolveVarCType(varName);
            if (t == CType.CharPtr)
                return $"{varName} = std::string(1, {chExpr});";
            return $"// TODO: assign {chExpr} to {varName} (type: {t})";
        }

    }
}