using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        //FunctionCallStmt ::= IDENTIFIER "(" ArgList? ")" ";"
        //ArgList ::= Expr ("," Expr)*
        public static FunctionCallStmt? FunctionCallStmt()
        {
            int start = _index;
            string startToken = token;

            if (!Match("IDENTIFIER")) { _index = start; token = startToken; return null; }
            var nameCall = _tokenVector[_index-1];
            if (!Match("OPARENTHESIS")) { _index = start; token = startToken; return null; }
            var arguments = new List<Expr>();
            // ArgList?  (empty if ')')
            if (PeekType(0) != "CPARENTHESIS")
            {
                var firstArgument = Expr();
                if (firstArgument==null) { _index = start; token = startToken; return null; }
                arguments.Add(firstArgument);
                while (Match("COMMA"))
                {
                    var nextArgument = Expr();
                    if (nextArgument==null) { _index = start; token = startToken; return null; }
                    arguments.Add(nextArgument);
                }
            }

            if (!Expect("CPARENTHESIS")) { _index = start; token = startToken; return null; }
            if (!Expect("SEMICOLON")) { _index = start; token = startToken; return null; }
            return new()
            {
                Arguments=arguments,
                Name=nameCall
            };
        }


        // NammedSentenceCall ::= "@" IDENTIFIER "(" (IDENTIFIER "=" IDENTIFIER)? ")" ";"
        public static NammedSentenceCallStmt? NammedSentenceCall()
        {
            int start = _index;
            string startToken = token;
            Token? internalVariable = null;
            Token? assignation = null;


            if (!Match("ARROBA")) { _index = start; token = startToken; return null; }
            if (!Match("IDENTIFIER")) { _index = start; token = startToken; return null; }
            var nammedSentence = _tokenVector[_index-1];
            if (!Expect("OPARENTHESIS")) { _index = start; token = startToken; return null; }

            // (IDENTIFIER "=" IDENTIFIER)? opcional
            if (PeekType(0) != "CPARENTHESIS")
            {
                if (!Match("IDENTIFIER")) { _index = start; token = startToken; return null; }
                internalVariable=_tokenVector[_index-1];
                if (!Expect("EQUAL")) { _index = start; token = startToken; return null; }
                if (!Expect("IDENTIFIER")) { _index = start; token = startToken; return null; }
                assignation=_tokenVector[_index-1];
            }

            if (!Expect("CPARENTHESIS")) { _index = start; token = startToken; return null; }
            if (!Expect("SEMICOLON")) { _index = start; token = startToken; return null; }

            return new()
            {
                Assignation=assignation,
                InternalVariable=internalVariable,
                Name=nammedSentence
            };
        }
        
    }
}