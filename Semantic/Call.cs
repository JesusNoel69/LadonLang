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
        //ToDo: cambiar el parser para que guarde la posicion del outParameter
        private void VisitFunctionCall(FunctionCallStmt call)
        {
            var sym = _symbols.Current.Resolve(call.Name.Lexeme);
            if (sym is not FunctionSymbol fn)
                throw new UndeclaredSymbolException(call.Name);
            var hasOut = fn.OutPosition != null;
            var expectedCount = fn.Parameters.Count + (hasOut ? 1 : 0);
            var givenCount = call.Arguments?.Count ?? 0;
            if (givenCount != expectedCount)
                throw new ParserExeption(
                    $"Function '{fn.Name}' expects {expectedCount} arguments, got {givenCount}.",
                    call.Name
                );
            VariableSymbol? outVar = null;
            if (hasOut)
            {
                int outIndex = fn.OutPosition!.Value; // 0 based
                if (outIndex < 0 || outIndex >= givenCount)
                    throw new ParserExeption($"Invalid out position {outIndex} for function '{fn.Name}'.", call.Name);

                var outArgExpr = call.Arguments![outIndex];
                outVar = RequireDeclaredVariableAsOutArg(outArgExpr, GetExprToken(outArgExpr));
                if (fn.OutParameter != null && fn.OutParameter.AllowedTypes.Count > 0)
                {
                    var outArgType = VisitExpr(outArgExpr);
                    bool ok = fn.OutParameter.AllowedTypes.Any(t => IsCompatible(outArgType, t));
                    if (!ok)
                    {
                        throw new TypeMismatchException(
                            $"Out argument type mismatch for '{fn.Name}': expected {FormatTypes(fn.OutParameter.AllowedTypes)}, got {outArgType.Name}.",
                            GetExprToken(outArgExpr)
                        );
                    }
                }
            }
            /*for (int i = 0; i < expectedCount; i++)
            {
                var param = fn.Parameters[i];
                var argExpr = call?.Arguments?[i];
                var argType = VisitExpr(argExpr??null);
                //if the paramenter have no explicit parameter
                if (param.AllowedTypes.Count == 0) continue;
                bool ok = param.AllowedTypes.Any(t => IsCompatible(argType, t));
                if (!ok)
                {
                    var expected = string.Join(" | ", param.AllowedTypes.Select(t => t.Name));
                    throw new TypeMismatchException(
                        $"Argument #{i+1} for '{fn.Name}' expected {expected}, got {argType.Name}.",
                        GetExprToken(argExpr)
                    );
                }
            }*/
            for (int argIndex = 0; argIndex < givenCount; argIndex++)
            {
                if (hasOut && argIndex == fn.OutPosition!.Value)
                    continue; //validate as out parameter

                int paramIndex = hasOut
                    ? MapArgIndexToParamIndex(argIndex, fn.OutPosition!.Value)
                    : argIndex;

                var param = fn.Parameters[paramIndex];
                var argExpr = call.Arguments![argIndex];
                var argType = VisitExpr(argExpr);

                if (param.AllowedTypes.Count == 0) continue;

                bool ok = param.AllowedTypes.Any(t => IsCompatible(argType, t));
                if (!ok)
                {
                    var expected = FormatTypes(param.AllowedTypes);
                    throw new TypeMismatchException(
                        $"Argument #{argIndex + 1} for '{fn.Name}' expected {expected}, got {argType.Name}.",
                        GetExprToken(argExpr)
                    );
                }
            }
        }
        private VariableSymbol RequireDeclaredVariableAsOutArg(Expr argExpr, Token blameToken)
        {
            if (argExpr is VariableExpr v)
            {
                var sym = _symbols.Current.Resolve(v.Name.Lexeme);
                if (sym is not VariableSymbol varSym)
                    throw new ParserExeption($"Out argument must be a declared variable: '{v.Name.Lexeme}'.", v.Name);
                return varSym;
            }

            // Extend obj.field or arr[i] in a future extension.
            throw new ParserExeption("Out argument must be a variable (assignable l-value), not an expression/value.", blameToken);
        }
        private int MapArgIndexToParamIndex(int argIndex, int outIndex)
        {
            return argIndex < outIndex ? argIndex : argIndex - 1;
        }
        //maybe this need nomre permisive or improve asignation management
        private void VisitNamedSentenceCall(NammedSentenceCallStmt call)
        {
            var sym = _symbols.Current.Resolve(call.Name.Lexeme);
            if (sym is not NamedSentenceSymbol)
                throw new UndeclaredSymbolException(call.Name);

            // @nombre();
            if (call.InternalVariable == null && call.Assignation == null)
                return;

            // if one exists, another one too
            if (call.InternalVariable == null || call.Assignation == null)
                throw new ParserExeption(
                    "Named sentence call mapping must be 'x=y' or nothing.",
                    call.Name
                );

            /*
            // internal varible exists
            var rhs = _symbols.Current.Resolve(call.Assignation.Lexeme);
            if (rhs is not VariableSymbol)
                throw new UndeclaredSymbolException(call.Assignation);
                */
            var left = _symbols.Current.Resolve(call.InternalVariable.Lexeme);
            if (left is not VariableSymbol leftVar)
                throw new UndeclaredSymbolException(call.InternalVariable);

            var right = _symbols.Current.Resolve(call.Assignation.Lexeme);
            if (right is not VariableSymbol rightVar)
                throw new UndeclaredSymbolException(call.Assignation);

            // si twice have explicit types check compatbility
            if (leftVar.AllowedTypes.Count > 0 && rightVar.AllowedTypes.Count > 0)
            {
                bool ok = rightVar.AllowedTypes.Any(rt => leftVar.AllowedTypes.Any(lt => IsCompatible(rt, lt)));
                if (!ok)
                {
                    throw new TypeMismatchException(
                        $"Cannot bind '{rightVar.Name}' ({FormatTypes(rightVar.AllowedTypes)}) to '{leftVar.Name}' ({FormatTypes(leftVar.AllowedTypes)}).",
                        call.Assignation
                    );
                }
            }
        }
        private static string FormatTypes(IEnumerable<TypeRef> types)
            => string.Join(" | ", types.Select(t => t.Name));
    }
}