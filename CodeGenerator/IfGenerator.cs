using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Parser.Models;

namespace LadonLang.CodeGenerator
{
    public partial class CCodegenerator
    {
        private string RenderIf(IfStmt stmt)
        {
            var sb = new System.Text.StringBuilder();

            // if (...)
            sb.Append("if(")
            .Append(RenderExpr(stmt.Condition))
            .AppendLine(")")
            .AppendLine(RenderBlock(stmt.ThenBlock));

            // elif (...)
            foreach (var elif in stmt.ElifStmtList)
            {
                sb.Append("else if(")
                .Append(RenderExpr(elif.Condition))
                .AppendLine(")")
                .AppendLine(RenderBlock(elif.Block));
            }

            // else
            if (stmt.ElseStmt != null)
            {
                sb.AppendLine("else")
                .AppendLine(RenderBlock(stmt.ElseStmt.Block));
            }

            return sb.ToString();
        }

    }
}