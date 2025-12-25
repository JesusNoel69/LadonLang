using LadonLang.Data;
namespace LadonLang.Parser.Models
{
    public abstract class AssignTarget : AstNode { }
    public class IdentifierTarget : AssignTarget
    {
        public Token Identifier { get; set; } = null!;
    }
    public class InputStmt : StmtNode
    {
        //set
        public Token? SetVariable { get; set; }//set[nombre_variable]
        public Token? Key { get; set; }//key='A'
        public bool UseType {get; set;}//type=[key]
    }
    public class OutputStmt : StmtNode
    {
        //get
        public Token? GetVariable { get; set; }
        public List<Expr> PrintList{get;set;}=[];
    }
}