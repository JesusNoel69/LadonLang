using LadonLang.Data;

namespace LadonLang.Parser.Models
{
    public class BlockStmt : StmtNode
    {
        public List<StmtNode> Statements { get; set;} = new();
    }
    //"TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"
    
    public class IfStmt : StmtNode
    {
        public Token? Name {get; set;}
        public Expr Condition { get; set; } = null!;
        public BlockStmt ThenBlock { get; set; } = null!;
        public ElseStmt? ElseStmt {get; set;}
        public List<ElifStmt> ElifStmtList { get; set;} = [];
    }
    public class ElseStmt : StmtNode
    {
        public BlockStmt Block { get; set; } = null!;
    }
    public class ElifStmt : StmtNode
    {
        public Expr Condition {get; set;}= null!;
        public BlockStmt Block { get; set; } = null!;
    }
    
   
    /*class ExprStmt : StmtNode { } // expression as statement
    //Expressions
    class IdentifierExpr : ExprNode { Token Name; }
    class LiteralExpr : ExprNode { Token Value; }
    class BinaryExpr : ExprNode { ExprNode Left, Right; Token Op; }
    class CallExpr : ExprNode {  }
    class LambdaExpr : ExprNode {  }
    class RawExprNode : ExprNode { List<Token> Tokens; }
    */

}