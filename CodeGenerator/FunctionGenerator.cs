using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
        private string RenderParam(Parameter p)
        {
            //infer type, if can't infer use int default
            var baseType = p.Type != null ? MapType(p.Type) : "string";
            if (p.IsOutParameter)
                return $"{baseType}& {p.Name.Lexeme}";
            if (baseType == "string")
                return $"const {baseType}& {p.Name.Lexeme}";
            return $"{baseType} {p.Name.Lexeme}";
        }

        private (string retType, string? retStructName) ResolveFunctionReturn(FunctionStmt fn)
        {
            // creates a structure for types, but multi return it's not implemented yet
            //so, even if the logic is programmed it shouldn't create structures
            if (fn.ReturnTypes != null && fn.ReturnTypes.Count > 1)
            {
                var structName = $"{fn.Name.Lexeme}_ret";
                EnsureReturnStruct(structName, fn.ReturnTypes);
                return (structName, structName);
            }

            // that was for type in func, but at this moment handling value using pointers
            if (fn.ReturnTypes != null && fn.ReturnTypes.Count == 1)
                //at this moment oly use vid and return use reference 
                //return (MapType(fn.ReturnTypes[0]), null);
                return ("void",null);

            return ("void", null);
        }

        private void EnsureReturnStruct(string structName, List<Token> returnTypes)
        {
            // for no duplicate
            if (typeDecls.Contains($"typedef struct {structName}")) return;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"typedef struct {structName} {{");
            for (int i = 0; i < returnTypes.Count; i++)
            {
                var t = returnTypes[i];
                sb.Append("  ").Append(MapType(t)).Append(" ").Append($"_{i}").AppendLine(";");
            }
            sb.AppendLine($"}} {structName};");
            typeDecls += sb.ToString() + "\n";
        }
        private void RegisterFunction(FunctionStmt fn)
        {
            var (retType, retStructName) = ResolveFunctionReturn(fn);
            var paramList = new List<string>();
            foreach (var p in fn.Parameters)
                paramList.Add(RenderParam(p));

            if (fn.OutParameter != null)
            {
                var op = fn.OutParameter;
                var cType = op.Type != null ? MapType(op.Type) : "string";
                paramList.Add($"{cType}& {op.Name.Lexeme}");
            }

            var paramsText = paramList.Count == 0 ? "void" : string.Join(", ", paramList);
            var signature = $"{retType} {fn.Name.Lexeme}({paramsText})";

            functionPrototypes.Add(signature + ";");
            functionDefs.Add(RenderFunctionDefinition(fn, signature, retStructName));
        }

        private string RenderFunctionDefinition(FunctionStmt fn, string signature, string? retStructName)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(signature);
            sb.AppendLine("{");

            // body
            if (fn.Block != null)
            {
                foreach (var s in fn.Block.Statements)
                    sb.AppendLine(RenderStmt(s));
            }

            // if there's no return explicit and c require it  
            var needReturn = signature.StartsWith("void ") == false;
            if (needReturn)
            {
                if (retStructName != null)
                {
                    sb.AppendLine($"{retStructName} _r = {{0}};");
                    sb.AppendLine("return _r;");
                }
                else
                {
                    // return type
                    var t = fn.ReturnTypes[0];
                    //sb.AppendLine($"return {DefaultValueFor(t)};");
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
        private string MapType(Token t)
        {
            var lex = t.Lexeme;
            return lex switch
            {
                "int" => "int",
                "float" => "double",
                "char" => "char",
                "string" => "string",
                _ => "string" // fallback (or error)
            };
        }
        private string RenderFunctionCall(FunctionCallStmt call)
        {
            var args = call.Arguments != null && call.Arguments.Count > 0
                ? string.Join(", ", call.Arguments.Select(RenderExpr))
                : "";

            return $"{call.Name.Lexeme}({args});";
        }

        private string DefaultValueFor(Token t)
        {
            return t.Lexeme switch
            {
                "int" => "0",
                "float" => "0.0",
                "char" => "'\\0'",
                "string" => "NULL",
                _ => "0"
            };
        }

    }
}