using LadonLang.Data;
using LadonLang.Parser.Models;
namespace LadonLang.Parser
{
    public partial class Parser
    {
        public static int _index = 0;
        private static List<Token> _tokenVector=[];      
        static string token = "";
        static bool _silent = false;
        public static void Advance()
        {
            _index++;
            if (_index < _tokenVector.Count)
            {
                token = _tokenVector[_index].TokenType;
            }
            else
            {
                token = ""; // Avoid out of range accessess
            }
        }
        // Helpers
        static Token CurrentToken()
        {
            if (_tokenVector == null || _tokenVector.Count == 0)
                return new Token(0, "", "EOF", 0, 0);

            return (_index >= 0 && _index < _tokenVector.Count)
                ? _tokenVector[_index]
                : _tokenVector[_tokenVector.Count - 1];
        }
        public static bool Match(string type)
        {
            if (token == type)
            {
                Advance();
                return true;
            }
            return false;
        }
        public static bool Match(params string[] types)
        {
            foreach (var t in types)
            {
                if (token == t)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        public static Token DoubleToken(Token t1, Token t2)
        {
            Token t3 = new Token(0,t1.Lexeme+t2.Lexeme, t1.TokenType+"_"+t2.TokenType,t2.Lines, t1.Columns);
            return t3;
        }
        public static bool Expect(string type)
        {
            if (Match(type))
            {
                return true;
            }
            throw new UnexpectedTokenException(type, CurrentToken());
        }
        //how if eof is neccesary, actually token vector have no eof token
        static string PeekType(int offset)
        {
            int idx = _index + offset;
            if (idx >= 0 && idx < _tokenVector.Count)
                return _tokenVector[idx].TokenType;
            return "EOF";
        }
        // two characters != == <= 
        static bool MatchDouble(string first, string second)
        {
            if (token == first && PeekType(1) == second)
            {
                Advance(); // consume first
                Advance(); // consume second
                return true;
            }
            return false;
        }
        static void ExpectDouble(string first, string second, string expectedLabel)
        {
            if (!MatchDouble(first, second))
                throw new UnexpectedTokenException(expectedLabel, CurrentToken());
        }
        static bool MatchSingleAssign()
        {
            // '=' different to '=='
            if (token == "EQUAL" && PeekType(1) != "EQUAL")
            {
                Advance();
                return true;
            }
            return false;
        }
        public static BlockStmt Parse(List<Token> tokens)
        {
            _tokenVector = tokens;
            _index = 0;
            token = _tokenVector.Count > 0 ? _tokenVector[0].TokenType : "";
            var program = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                var stmt = Statement();
                if (stmt == null)
                {
                    throw new ParserExeption("Statement unexpected null return", CurrentToken());
                }
                program.Statements.Add(stmt);
            }
            return program;
        }
        public static StmtNode? Statement()
        {
            if (_index >= _tokenVector.Count)
            {
                return null;
            }
            token = _tokenVector[_index].TokenType;
            string t0 = PeekType(0);
            string t1 = PeekType(1);
            //embebed method to return statement
            static StmtNode Require(string name, Func<StmtNode?> parser)
            {
                int start = _index;
                string startToken = token;
                var node = parser();
                if (node != null) return node;
                _index = start;
                token = startToken;
                throw new ParserExeption($"invalid sentence: {name}.", CurrentToken());
            }

            //RETURN
            if (PeekType(0) == "LTHAN" && PeekType(1) == "RETURN_KEYWORD")
                return Require("<return/>", ReturnStmt);
            // INPUT
            if (t0 == "LTHAN" && t1 == "INPUT_KEYWORD")
                return Require("<input/>", Input);

            // OUTPUT
            if (t0 == "LTHAN" && t1 == "OUTPUT_KEYWORD")
                return Require("<output>", Output);
            
            // LOOP
            if (t0 == "LTHAN" && t1 == "LOOP_KEYWORD")
            {
                return Require("<loop>", Loop);              
            }
            // SELECT
            if (t0 == "LTHAN" && t1 == "SELECT_KEYWORD")
            {
                return Require("<select>", Select);               
            }
            // IF
            if (t0 == "LTHAN" && t1 == "IF_KEYWORD")
            {
                return Require("<if>", If);                
            }
            //FUNCTION
            if(t0 == "FN_KEYWORD")
            {
                return Require("fn", Function);
            }
            //SENTENCE NAMMED
            if (t0 == "ARROBA"){
                return Require("@call", NammedSentenceCall);
            }
            //FUNCTION CALL
            if (PeekType(0) == "IDENTIFIER" && PeekType(1) == "OPARENTHESIS")
            {
                return Require("FunctionCall", FunctionCallStmt);
            }
            //LAMBDA
            /*if (PeekType(0) == "IDENTIFIER" && (PeekType(1) == "LTHAN" || PeekType(1) == "EQUAL"))
            {
                int s = _index; var st = token;
                if (LambdaAssign()) return true;
                _index = s; token = st;
            }*/

            /*if (token == "VAR_KEYWORD" )
            {
                var varDecl = VarDeclStmt();
                if (varDecl==null)
                {
                    Console.WriteLine("Error: declaración de variable inválida.");
                    return null;
                }
                return ;
            }
            if(token == "IDENTIFIER" && PeekType(1) == "EQUAL")
            {
                System.Console.WriteLine("asignar");
                return Assign();
            }*/
            // VAR DECL
            if (t0 == "VAR_KEYWORD")
                return Require("VarDecl", VarDeclStmt); // VarDeclStmt(): VarDeclStmt?

            // ASSIGN
            if (t0 == "IDENTIFIER" && t1 == "EQUAL")
                return Require("Assign", Assign); // Assign(): AssignStmt?

            throw new UnexpectedTokenException(
                "sentence start (return/input/output/loop/select/if/fn/@call/call/var/assign)",
                CurrentToken()
            );
        }
        // ReturnStmt ::= "<" return "/>"
        public static ReturnStmt? ReturnStmt()
        {
            int start = _index;
            string startToken = token;
            if (!Match("LTHAN")) return null;
            if (!Match("RETURN_KEYWORD"))
            {
                _index = start;
                token = startToken;
                return null;
            }
            var returnKeyWord = _tokenVector[_index-1];
            if (!Match("SLASH"))
            {
                throw new UnexpectedTokenException("SLASH '/' to close <return/>", CurrentToken());
            }
            if (!Match("MTHAN"))
            {
                 throw new UnexpectedTokenException("MTHAN '>' to close <return/>", CurrentToken());
            }
            return new()
            {
                ReturnKeyWord=returnKeyWord
            };
        }
    }
}