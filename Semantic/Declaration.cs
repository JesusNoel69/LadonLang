
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Semantic
{
    public partial class SemanticAnalyzer 
    {
           private void VisitVarDecl(VarDeclStmt declaration)
        {
            var decl = declaration.Decl;

            foreach (var d in decl.Declarators)
            {
                // get name
                var nameTok = d.Identifier;
                var name = nameTok.Lexeme;
                //AllowedTypes (union)
                /*var allowed = (d.TypeArguments ?? new List<DataTypeNode>())
                    .Select(ToTypeRef)
                    .ToList();
                */
                var allowed = d.TypeArguments?.Select(ToTypeRef).ToList()
                        ?? new List<TypeRef>();
                //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(allowed));
                var sym = new VariableSymbol(
                    Name: name,
                    DeclToken: nameTok,
                    AllowedTypes: allowed
                );
                //declare scope
                if (!_symbols.Current.TryDeclare(sym, out var error))
                {
                    throw new DuplicateSymbolException(sym.Name,d.Identifier);  
                }
                //validate if exists
                if (d.Initializer != null)
                {
                    ValidateInitializerTypes(sym, d.Initializer);
                }
            }
        }
        private void VisitAssign(AssignStmt assign)
        {
            var targetName = assign.Variable.Lexeme;
            var sym = _symbols.Current.Resolve(targetName);

            if (sym is not VariableSymbol varSym)
                throw new UndeclaredSymbolException(assign.Variable);

            var rhsType = VisitExpr(assign.Assignment);

            // if has no explicit types
            if (varSym.AllowedTypes.Count == 0)
                return;

            // if it has one or more explicit types 
            bool ok = varSym.AllowedTypes.Any(t => IsCompatible(rhsType, t));
            if (!ok)
            {
                var expected = string.Join(" | ", varSym.AllowedTypes.Select(t => t.Name));
                throw new TypeMismatchException(
                    $"Cannot assign '{rhsType.Name}' to '{varSym.Name}'. Expected: {expected}",
                    assign.Variable
                );
            }
        }
        private TypeRef VisitExpr(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr lit:
                {
                    if (lit.Value is Token tok)
                        return TypeOfLiteral(tok);
                    return new TypeRef("unknown");
                }

                case VariableExpr v:
                {
                    var sym = _symbols.Current.Resolve(v.Name.Lexeme);
                    if (sym is not VariableSymbol vs)
                        throw new UndeclaredSymbolException(v.Name);
                    if (vs.AllowedTypes.Count == 1) return vs.AllowedTypes[0];
                    if (vs.AllowedTypes.Count > 1) return new TypeRef("unknown");
                    return new TypeRef("unknown"); // no explicit types
                }

                case UnaryExpr u:
                {
                    var rt = VisitExpr(u.Right);
                    if (u.Operator.TokenType == "MINUS")
                    {
                        if (rt.Name is "int" or "float") return rt;
                        throw new TypeMismatchException("Unary '-' requires numeric type", u.Operator);
                    }
                    if (u.Operator.TokenType == "DIFFERENT") // '!' or 'not'
                    {
                        if (rt.Name == "bool" || rt.Name == "unknown") return new TypeRef("bool");
                        throw new TypeMismatchException("Unary '!' requires boolean type", u.Operator);
                    }
                    return new TypeRef("unknown");
                }

                case BinaryExpr b:
                {
                    var lt = VisitExpr(b.Left);
                    var rt = VisitExpr(b.Right);

                    return b.Operator.TokenType switch
                    {
                        "PLUS" or "MINUS" or "ASTERISK" or "SLASH" or "PERCENT"
                            => NumericResult(lt, rt, b.Operator),

                        "LTHAN" or "MTHAN" or "LTHAN_EQUAL" or "MTHAN_EQUAL" =>
                            ((lt.Name is "int" or "float" or "unknown") && (rt.Name is "int" or "float" or "unknown"))
                                ? new TypeRef("bool")
                                : throw new TypeMismatchException("Comparison operators require numeric operands", b.Operator),

                       "EQUAL_EQUAL" or "DIFFERENT_EQUAL" =>
                            // if two are known they should be compatibles
                            (lt.Name != "unknown" && rt.Name != "unknown" && !IsCompatible(lt, rt) && !IsCompatible(rt, lt))?
                                throw new TypeMismatchException("Equality operands are not comparable", b.Operator):
                            new TypeRef("bool")
                        ,

                       "AND" or "OR" =>
                            (IsBooleanType(lt) || lt.Name == "unknown") && (IsBooleanType(rt) || rt.Name == "unknown")
                                ? new TypeRef("bool")
                                : throw new TypeMismatchException("Logical operators require boolean operands", b.Operator),


                        _ => new TypeRef("unknown")
                    };
                }

                case AssignExpr a:
                {
                    // a = b = c
                    var sym = _symbols.Current.Resolve(a.Name.Lexeme);
                    if (sym is not VariableSymbol vs)
                        throw new UndeclaredSymbolException(a.Name);

                    var rhs = VisitExpr(a.Value);
                    if (vs.AllowedTypes.Count > 0 && !vs.AllowedTypes.Any(t => IsCompatible(rhs, t)))
                    {
                        var expected = string.Join(" | ", vs.AllowedTypes.Select(t => t.Name));
                        throw new TypeMismatchException(
                            $"Cannot assign '{rhs.Name}' to '{vs.Name}'. Expected: {expected}",
                            a.Name
                        );
                    }
                    return rhs;
                }
                default:
                    return new TypeRef("unknown");
            }
        }
    }
}