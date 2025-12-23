using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;

namespace LadonLang.Parser.Models
{
    public class ReturnStmt : StmtNode
    {
        public Token ReturnKeyWord {get; set;} = null!;
        //this is only if a day needed, this property is not implemented yet
        public Expr? Expression { get; set; }
    }
}