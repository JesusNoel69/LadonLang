using LadonLang.Data;
namespace LadonLang.Parser.Models
{
    public class AssignStmt : StmtNode
    {
        public Token Variable {get;set;} = null!;
        public Token AssignOperator { get; set; } = null!; // "="
        public Expr Assignment {get;set;} = null!;
    }
}