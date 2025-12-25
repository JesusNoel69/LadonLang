using LadonLang.Data;
namespace LadonLang.Parser.Models
{
    public class VarDeclStmt : StmtNode
    {
        public VarDecl Decl { get; set; } = null!;
    }
    public class VarDecl : AstNode
    {
        public Token? VarKeyword { get; set; } // var keyword
        public List<VarDeclarator> Declarators { get; } = new();
    }
    public class VarDeclarator : AstNode
    {
        public Token Identifier { get; set; } = null!; // IDENTIFIER
        public List<DataTypeNode> TypeArguments { get; set; } =[]; 

        public VarInitializerNode? Initializer { get; set; } // null if not "="
    }
    public class DataTypeNode : AstNode
    {
        public Token BaseTypeKeyword { get; set; } = null!; // INT_KEYWORD/FLOAT_KEYWORD/...
        //Qty in bytes of data type
        public Token? SizeOrArg1 { get; set; }              // INTEGER_NUMBER or FLOAT_NUMBER optional
        //if is float and decmal bytes are defined as well
        public Token? SizeOrArg2 { get; set; }              // INTEGER_NUMBER or FLOAT_NUMBER optional (if has DOT)
    }
    public class VarInitializerNode : AstNode
    {
        public Token AssignOperator { get; set; } = null!;
        public List<Expr> Expressions { get; } = new();
    }
}