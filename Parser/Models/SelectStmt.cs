using LadonLang.Data;
namespace LadonLang.Parser.Models
{
    public class SelectStmt : StmtNode
    {
        public Token? Name {get; set;}
        //"TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"
        public Token Value { get; set; } //should be a expr in future
        public List<OptionStmt> Options { get; set;} =[];
    }
    public class OptionStmt
    {
        public bool IsDefault {get; set;} = false;
        //"TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"
        public Token? Value { get; set; } //null if it's deault
        public BlockStmt Block { get; set; } = null!;
    }
}