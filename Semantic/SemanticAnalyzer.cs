using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Semantic
{
    public partial class SemanticAnalyzer
    {
        private FunctionSymbol? _currentFunction;
        private SymbolTable _symbols = new();
        public SymbolTable GetSymbolTable()
        {
            return _symbols;
        }
        public void Analyze(ProgramNode program)
        {
            foreach (var stmt in program.Statements)
            {
                Analyze(stmt);
            }
        }
        private void Analyze(StmtNode stmt)
        {
            switch (stmt)
            {
                case VarDeclStmt v: VisitVarDecl(v); break;
                case AssignStmt a:  VisitAssign(a);  break;
                case FunctionStmt f: VisitFunction(f); break;
                case IfStmt i: VisitIf(i); break;
                case SelectStmt s: VisitSelect(s); break;
                case LoopStmt l: VisitLoop(l); break;
                case InputStmt i: VisitInput(i); break;
                case OutputStmt o: VisitOutput(o); break;
                case FunctionCallStmt f: VisitFunctionCall(f); break;
                case ReturnStmt r: VisitReturn(r); break;
                case NammedSentenceCallStmt n: VisitNamedSentenceCall(n); break;
            }
        }

        private static TypeRef ToTypeRef(DataTypeNode dt)
        {
            double? a1 = TryNum(dt.SizeOrArg1);
            double? a2 = TryNum(dt.SizeOrArg2);

            var name = dt.BaseTypeKeyword.TokenType switch
            {
                "INT_KEYWORD"    => "int",
                "FLOAT_KEYWORD"  => "float",
                "BOOL_KEYWORD"   => "bool",
                "CHAR_KEYWORD"   => "char",
                "STRING_KEYWORD" => "string",
                "TEXT_KEYWORD"   => "text",
                //maybe should be a keyword
                _ => dt.BaseTypeKeyword.Lexeme
            };
            return new TypeRef(name, a1, a2);
        }

        private static double? TryNum(Token? t)
        {
            if (t == null) return null;
            if (double.TryParse(t.Lexeme, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
                return d;
            return null;
        }
        private static TypeRef NumericResult(TypeRef a, TypeRef b, Token op)
        {
            bool lnum = a.Name is "int" or "float" or "unknown";
            bool rnum = b.Name is "int" or "float" or "unknown";
            if (!lnum || !rnum)
                throw new TypeMismatchException("Arithmetic operators require numeric operands", op);

            if (a.Name == "float" || b.Name == "float") return new TypeRef("float");
            if (a.Name == "int" && b.Name == "int") return new TypeRef("int");
            return new TypeRef("unknown");
        }
        private void ValidateInitializerTypes(VariableSymbol sym, VarInitializerNode init)
        {
            var exprs = init.Expressions;
            // infer kind of every expresion
            var exprTypes = exprs.Select(VisitExpr).ToList();
            if (sym.AllowedTypes == null || sym.AllowedTypes.Count == 0)
                return;
            if (sym.AllowedTypes.Count == exprTypes.Count)
            {
                for (int i = 0; i < exprTypes.Count; i++)
                {
                    if (!IsCompatible(exprTypes[i], sym.AllowedTypes[i]))
                        throw new TypeMismatchException(
                            $"Initializer type mismatch for '{sym.Name}' at index {i}: expected {sym.AllowedTypes[i].Name}, got {exprTypes[i].Name}",
                            sym.DeclToken
                        );
                }
                return;
            }
            if (sym.AllowedTypes.Count == 1)
            {
                var expected = sym.AllowedTypes[0];
                for (int i = 0; i < exprTypes.Count; i++)
                {
                    if (!IsCompatible(exprTypes[i], expected))
                        throw new TypeMismatchException(
                            $"Initializer type mismatch for '{sym.Name}': expected {expected.Name}, got {exprTypes[i].Name}",
                            sym.DeclToken
                        );
                }
                return;
            }
            throw new TypeMismatchException(
                $"Initializer rarity mismatch for '{sym.Name}': declared {sym.AllowedTypes.Count} type(s) but got {exprTypes.Count} expression(s)",
                sym.DeclToken
            );
        }
        private static TypeRef TypeOfLiteral(Token tok)
        {
            return tok.TokenType switch
            {
                "TRUE_KEYWORD" or "FALSE_KEYWORD" => new TypeRef("bool"),
                "INTEGER_NUMBER" => new TypeRef("int"),
                "FLOAT_NUMBER" => new TypeRef("float"),
                "CHARACTER" => new TypeRef("char"),
                "STRING" => new TypeRef("string"),
                _ => new TypeRef("unknown")
            };
        }
        private static bool IsCompatible(TypeRef got, TypeRef expected)
        {
            if (expected.Name == "unknown") return true; // if apears unknown type 
            if (got.Name == expected.Name) return true;
            if (got.Name == "int" && expected.Name == "float") return true;
            return false;
        }
        private static TypeRef TypeRefFromToken(Token t)
        {
            var name = t.TokenType switch
            {
                "INT_KEYWORD" => "int",
                "FLOAT_KEYWORD" => "float",
                "BOOL_KEYWORD" => "bool",
                "CHAR_KEYWORD" => "char",
                "STRING_KEYWORD" => "string",
                "TEXT_KEYWORD" => "text",
                "IDENTIFIER" => t.Lexeme, // class
                _ => t.Lexeme // fallback
            };
            return new TypeRef(name);
        }
        private bool IsBooleanType(TypeRef? type)
        {
            //if cant inferr is allowed
            if (type == null)
                return true;

            return type.Name == "bool";
        }
        private static Token TokenFromExpr(Expr expr)
        {
            return expr switch
            {
                VariableExpr v => v.Name,
                AssignExpr a => a.Name,
                UnaryExpr u => u.Operator,
                BinaryExpr b => b.Operator,
                LiteralExpr l when l.Value is Token tok => tok,
                _ => throw new Exception("Expr without anchor token. Add StartToken to expr.")
            };
        }

    }
}