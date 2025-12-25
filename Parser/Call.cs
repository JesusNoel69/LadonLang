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
            bool prevSilent = _silent;

            //might have errors
            _silent=true;
            if (!Match("IDENTIFIER")) 
            { 
                /*_index = start; 
                token = startToken;
                */
                _silent = prevSilent;
                return null; 
            }
            var nameCall = _tokenVector[_index-1];
            if (!Match("OPARENTHESIS")) 
            { 
                _index = start; 
                token = startToken;
                _silent=prevSilent;
                return null;
            }
            var arguments = new List<Expr>();
            // ArgList?  (empty if ')')
            if (PeekType(0) != "CPARENTHESIS")
            {
                var firstArgument = Expr();
                if (firstArgument==null) 
                {
                    _index = start; 
                    token = startToken; 
                    throw new ParserExeption("An expression expected as call argument", CurrentToken());
                }
                arguments.Add(firstArgument);
                while (Match("COMMA"))
                {
                    var nextArgument = Expr();
                    if (nextArgument==null) 
                    { 
                        _index = start; 
                        token = startToken; 
                        throw new ParserExeption("Expression expected after ','.", CurrentToken()); 
                    }
                    arguments.Add(nextArgument);
                }
            }
            Expect("CPARENTHESIS");
            Expect("SEMICOLON");
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
            bool prevSilent = _silent;
            _silent=true;

            if (!Match("ARROBA"))
            { 
                _index = start; 
                token = startToken;
                _silent = prevSilent;
                return null; 
            }
            if (!Match("IDENTIFIER")) 
            {
                _index = start; 
                token = startToken; 
                _silent = prevSilent;
                throw new UnexpectedTokenException("IDENTIFIER after '@'", CurrentToken());
            }
            var nammedSentence = _tokenVector[_index-1];
            if (!Match("OPARENTHESIS"))
            {
                _index = start;
                token = startToken;
                _silent = prevSilent;
                throw new UnexpectedTokenException("OPARENTHESIS '(' after @Identifier", CurrentToken());
            }
            // (IDENTIFIER "=" IDENTIFIER)? optional
            if (PeekType(0) != "CPARENTHESIS")
            {
                if (!Match("IDENTIFIER"))
                {
                    _index = start;
                    token = startToken;
                    throw new UnexpectedTokenException("IDENTIFIER (internal variable) or ')'", CurrentToken()); 
                }
                internalVariable=_tokenVector[_index-1];
                Expect("EQUAL");
                if (!Match("IDENTIFIER")) 
                { 
                    _index = start; 
                    token = startToken; 
                    throw new UnexpectedTokenException("IDENTIFIER (value assignation)", CurrentToken());
                }
                assignation=_tokenVector[_index-1];
            }
            Expect("CPARENTHESIS");
            Expect("SEMICOLON");
            return new()
            {
                Assignation=assignation,
                InternalVariable=internalVariable,
                Name=nammedSentence
            };
        }
        
    }
}