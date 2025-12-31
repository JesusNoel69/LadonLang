using LadonLang.Data;
namespace LadonLang.Parser.Models
{
    public class FunctionStmt: StmtNode
    {
        public Token Name {get; set;} = null!;
        public List<Parameter> Parameters { get; set;} = [];
        public List<Token> ReturnTypes { get; set;} = [];
        public Parameter? OutParameter {get; set;}
        public BlockStmt? Block { get; set; } = null!;
    }
    public class Parameter : AstNode
    {
        public Token Name { get; set; } = null!;
        public Token? Type{get; set;}
        public bool IsOutParameter {get; set;} = false;
        public int Position {get; set;}
    }
}