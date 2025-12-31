namespace LadonLang.Parser.Models
{
    public abstract class AstNode{ }
    public abstract class StmtNode : AstNode { }
    public abstract class ExprNode : AstNode { }
    public sealed class ProgramNode : AstNode
    {
        public List<StmtNode> Statements { get; } = new();
    }

}