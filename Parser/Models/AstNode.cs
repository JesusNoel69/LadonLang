using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser.Models
{
    public abstract class AstNode{ }
    public abstract class StmtNode : AstNode { }
    public abstract class ExprNode : AstNode { }
}