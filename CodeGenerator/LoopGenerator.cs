using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Parser.Models;
using LadonLang.Semantic;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
                private string RenderLoop(LoopStmt loop)
        {
            var sb = new System.Text.StringBuilder();
            if(loop.Name != null)
            {
                //make function not yet
            }
            switch(loop.LoopHeader)
            {
                case LoopIt loopIt:
                {   
                    string init = loopIt.Init != null ? TrimForPart(RenderForInit(loopIt.Init)) : "";
                    string cond = loopIt.Condition != null ? TrimForPart(RenderExpr(loopIt.Condition)) : "";
                    string step = loopIt.Step != null ? TrimForPart(RenderExpr(loopIt.Step)) : "";
                    sb.Append("for(")
                        .Append(init).Append("; ")
                        .Append(cond).Append("; ")
                        .Append(step).AppendLine(")")
                        //.AppendLine("{");
                        .AppendLine(RenderBlock(loop.Block));
                        break;
                }
                case LoopDo d:
                {
                    var cond = d.Condition != null ? RenderExpr(d.Condition) : "true";

                    if (d.IsPostTest)
                    {
                        // do { ... } while(cond);
                        sb.AppendLine("do")
                        .AppendLine(RenderBlock(loop.Block))
                        .Append("while(").Append(cond).AppendLine(");");
                    }
                    else
                    {
                        // while(cond) { ... }
                        sb.Append("while(").Append(cond).AppendLine(")")
                        .AppendLine(RenderBlock(loop.Block));
                    }
                    break;
                }
            }
            //sb.AppendLine("}");
            return sb.ToString();
        }
        private string RenderForInit(Expr init)
        {
            // i = 0
            if (init is AssignExpr a)
            {
                var name = a.Name.Lexeme;

                var existing = table.Current.Resolve(name);

                if (existing == null)
                {
                    //type inferred from rhs
                    var ctype = InferCTypeFromExpr(a.Value);
                    var ctypeText = CTypeToCText(ctype);

                    var sym = new VariableSymbol(
                        Name: name,
                        DeclToken: a.Name,
                        AllowedTypes: new List<TypeRef>(),
                        IsOutParam: false,
                        IsParameter: false
                    );

                    if (!table.Current.TryDeclare(sym, out var error))
                    {
                        return $"{name} = {RenderExpr(a.Value)}";
                    }
                    //initialized declaration
                    return $"{ctypeText} {name} = {RenderExpr(a.Value)}";
                }

                // already exists only reasign
                return $"{name} = {RenderExpr(a.Value)}";
            }
            // fallback
            return RenderExpr(init);
        }
        private string CTypeToCText(CType t) => t switch
        {
            CType.Int => "int",
            CType.Double => "double",
            CType.Bool => "bool",
            CType.CharPtr => "char*",
            _ => "/*unknown*/ int"
        };

    }
}