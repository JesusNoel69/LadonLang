using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using  LadonLang.Parser.Models;

namespace LadonLang.Parser.Models
{
     public class AssignStmt : StmtNode
    {
        public Token Variable {get;set;} = null!;
        public Token AssignOperator { get; set; } = null!; // "="
        public Expr Assignment {get;set;} = null!;
    }
}