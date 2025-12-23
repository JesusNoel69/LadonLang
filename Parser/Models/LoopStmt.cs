using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;

namespace LadonLang.Parser.Models
{
    public class LoopStmt : StmtNode
    {
        public string Name {get; set;} = string.Empty;
        public LoopHeader LoopHeader{get; set;}
        public BlockStmt Block { get; set;}
    }
    public class LoopHeader : AstNode
    {}
    public class LoopIn : LoopHeader
    {
        public Token IteratorName { get; set; } = null!; // IDENTIFIER
        public Token Iterable { get; set; }
        
    }
    public class LoopOf : LoopHeader
    {
        public Token EachName { get; set; } = null!; // IDENTIFIER
        public Token Iterable { get; set; } = null;
    }
    public class LoopIt : LoopHeader
    {
        public Expr? Init { get; set; }      // AssignStmt o VarDeclStmt
        public Expr? Condition { get; set; }
        public Expr? Step { get; set; }          // i = i + 1, i++
    }
    public class LoopDo : LoopHeader
    {
        public Expr? Condition {get; set;}=null;
        public bool IsPostTest { get; set; } //while or do while
    }
}