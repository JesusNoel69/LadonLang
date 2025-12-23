using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Parser.Models
{
    public class FunctionCallStmt : StmtNode
    {
        public Token Name {get; set;} = null!;
        public List<Expr> Arguments { get; set;} = [];
    }
    public class NammedSentenceCallStmt : StmtNode
    {
        public Token Name {get; set;} = null!;
        public Token? InternalVariable {get; set;} = null;
        public Token? Assignation {get; set;} = null;
    }
}