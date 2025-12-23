using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;

namespace LadonLang.Parser.Models
{
    public class SelectStmt : StmtNode
    {
        public Token? Name {get; set;}
        //"TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"
        public Token Value { get; set; }
        public List<OptionStmt> Options { get; set;} =[];
    }
    /*public class CaseValue
    {
        public Token Token { get; }
        public CaseValue(Token token) => Token = token;
    }*/
    public class OptionStmt
    {
        public bool IsDefault {get; set;} = false;
        //"TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"
        public Token? Value { get; set; } //null if it's deault
        public BlockStmt Block { get; set; } = null!;
    }
}