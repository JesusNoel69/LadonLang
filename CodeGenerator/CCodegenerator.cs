using System.Text;
using LadonLang.Data;
using LadonLang.Parser.Models;
using LadonLang.Semantic;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
        public CCodegenerator(ProgramNode program, string path, SymbolTable table)
        {
            this.program = program;
            this.path = path;
            this.table = table;
        }
        private readonly StringBuilder mainSb = new();
        ProgramNode program;
        string typeDecls ="";
        SymbolTable table;
        List<string> functionPrototypes = new();
        List<string> functionDefs = new();
        string path;
        string runtime = "struct LadonRT{char ch;};";
        //string directives = $"#include <cstdint>\n#include <string>\n#include <vector>\n#include <iostream>\nusing namespace std;\n";
        public void TraverseAst()
        {
            foreach (var statement in program.Statements)
            {
                if (statement is FunctionStmt fn)
                    RegisterFunction(fn);
                else
                    mainSb.AppendLine(RenderStmt(statement));
            }
            var outDir = System.IO.Path.Combine(path, "");
            Directory.CreateDirectory(outDir);
            var outFile = Path.Combine(outDir, "main.cpp");
            using var file = new StreamWriter(outFile);

            file.WriteLine("#include <cstdint>");
            file.WriteLine("#include <cstdlib>");
            file.WriteLine("#include <string>");
            file.WriteLine("#include <vector>");
            file.WriteLine("#include <iostream>");
            file.WriteLine("using namespace std;");
            file.WriteLine();

            if (!string.IsNullOrWhiteSpace(typeDecls))
                file.WriteLine(typeDecls);

            foreach (var p in functionPrototypes) file.WriteLine(p);
            file.WriteLine();
            file.WriteLine(runtime);//struct for runtime

            file.WriteLine("int main() {");
            file.WriteLine("LadonRT ladon;");//instance of runtime
            file.WriteLine(mainSb.ToString());
            file.WriteLine(" cout << \"Press Enter to exit...\";cin.get();");
            file.WriteLine("return 0;");
            file.WriteLine("}");
            file.WriteLine();

            foreach (var d in functionDefs) file.WriteLine(d);
        }
       
        private static string TrimForPart(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            if (s.EndsWith(";")) s = s[..^1].Trim();
            return s;
        }
       
        private string RenderExpr(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr lit:
                    return RenderLiteral(lit.Value);
                case VariableExpr v:
                    return v.Name.Lexeme;
                case UnaryExpr u:
                    return $"({u.Operator.Lexeme}{RenderExpr(u.Right)})";
                case BinaryExpr b:
                    return $"({RenderExpr(b.Left)} {b.Operator.Lexeme} {RenderExpr(b.Right)})";
                case AssignExpr a:
                    return $"({a.Name.Lexeme} = {RenderExpr(a.Value)})";
                default:
                    throw new NotSupportedException($"Expr not supported: {expr.GetType().Name}");
            }
        }
      
        private string RenderStmt(StmtNode stmt)
        {
            return stmt switch
            {
                VarDeclStmt v => RenderVarDecl(v),
                LoopStmt l    => RenderLoop(l),
                ReturnStmt r  => RenderReturn(r),
                BlockStmt b   => RenderBlock(b),
                SelectStmt s  => RenderSelect(s),
                IfStmt i      => RenderIf(i),
                AssignStmt a  => RenderAssign(a),
                FunctionCallStmt fc => RenderFunctionCall(fc),
                InputStmt i => RenderInput(i),
                OutputStmt o => RenderOutput(o),

                _ => throw new NotSupportedException($"Stmt no soportado: {stmt.GetType().Name}")
            };
        }

        private string RenderReturn(ReturnStmt r)
        {
            return "return;";
        }

        private string RenderBlock(BlockStmt block)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("{");
            foreach (var s in block.Statements)
                sb.AppendLine(RenderStmt(s));
            sb.Append('}');
            return sb.ToString();
        }

        //helpers for infer types:
        private enum CType { Int, Double, Bool, CharPtr, Unknown }
        private CType InferCTypeFromExpr(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr lit:
                    return InferFromLiteral(lit.Value);

                case VariableExpr v:
                    return ResolveVarCType(v.Name.Lexeme);

                case UnaryExpr u:
                    return InferCTypeFromExpr(u.Right);

                case BinaryExpr b:
                    var lt = InferCTypeFromExpr(b.Left);
                    var rt = InferCTypeFromExpr(b.Right);
                    return PromoteBinary(lt, rt);

                case AssignExpr a:
                    return InferCTypeFromExpr(a.Value);

                default:
                    return CType.Unknown;
            }
        }

        private CType ResolveVarCType(string varName)
        {
            var sym = table.Current.Resolve(varName);
            if (sym == null)
                return CType.Unknown;

            if (sym is VariableSymbol v)
            {
                //if last has explicit type
                if (v.AllowedTypes != null && v.AllowedTypes.Count > 0)
                {
                    return MapTypeRefToCType(v.AllowedTypes[0]);
                }
            }

            return CType.Unknown;
        }

        private CType MapTypeRefToCType(TypeRef type)
        {
            return type.Name switch
            {
                "int"    => CType.Int,
                "float"  => CType.Double,
                "double" => CType.Double,
                "bool"   => CType.Bool,
                "string" => CType.CharPtr,
                _ => CType.Unknown
            };
        }

        private CType InferFromLiteral(object? value)
        {
            if (value is Token t)
            {
                return t.TokenType switch
                {
                    "INTEGER_NUMBER" => CType.Int,
                    "FLOAT_NUMBER"   => CType.Double,
                    "TRUE" or "FALSE" or "BOOL" => CType.Bool,
                    "STRING" or "TEXT" => CType.CharPtr,
                    _ => CType.Unknown
                };
            }

            return value switch
            {
                int => CType.Int,
                long => CType.Int,
                float => CType.Double,
                double => CType.Double,
                bool => CType.Bool,
                string => CType.CharPtr,
                _ => CType.Unknown
            };
        }

        private CType PromoteBinary(CType a, CType b)
        {
            // if any is double produces double
            if (a == CType.Double || b == CType.Double) return CType.Double;
            if (a == CType.Int || b == CType.Int) return CType.Int;
            if (a == CType.Bool && b == CType.Bool) return CType.Bool;
            return CType.Unknown;
        }
    }
}