using LadonLang.Data;
using LadonLang.Parser.Models;
namespace LadonLang.Parser
{
    public partial class Parser
    {
        private static char[] _delimiters=[' ','-', ';',',','[', ']','(',')','#','=','+','%','&','/','|','*','!','@',':','<','>','\n']; 
        private static readonly char[] _numbers=['0','1','2','3','4','5','6','7','8','9'];
        private static char[] _letters = ['A',
        'b', 'B', 'c', 'C', 'D','E', 'F', 'G', 'h', 'H', 'I', 'j', 'J', 'k', 'K',
        'L', 'M','N','O','p','P', 'q', 'Q','R', 'S' ,'T','U', 'v', 'V', 'w', 'X','Y', 'z', 'Z','_'];
        private static int _col=0;
        public static int _index = 0;
        private static List<Token> _tokenVector=[];      
        public static string token = "";
        public static string nextToken = "";
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
            //System.Console.WriteLine("x"+type);
            if (Match(type)) return true;
            Console.WriteLine($"Error: se esperaba {type}, se encontró {token}");
            return false;
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
        public static bool Parse(List<Token> tokens)
        {
            _tokenVector = tokens;
            _index = 0;
            token = _tokenVector.Count > 0 ? _tokenVector[0].TokenType : "";
            while (_index < _tokenVector.Count)
            {
                var stmt = Statement();
                if (stmt == null)
                {
                    Console.WriteLine("Error to sentence parse.");
                    return false;
                }
            }

            return true;
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
            //RETURN
            if (PeekType(0) == "LTHAN" && PeekType(1) == "RETURN_KEYWORD")
                return ReturnStmt();
            // INPUT
            System.Console.WriteLine("t0: "+t0+" t1: "+t1);
            if (t0 == "LTHAN" && t1 == "INPUT_KEYWORD")
                return Input();

            // OUTPUT
            if (t0 == "LTHAN" && t1 == "OUTPUT_KEYWORD")
                return Output();
            
            // LOOP
            if (t0 == "LTHAN" && t1 == "LOOP_KEYWORD")
            {
                System.Console.WriteLine("ientifica que es un loop");
                return Loop();                
            }
            // SELECT
            if (t0 == "LTHAN" && t1 == "SELECT_KEYWORD")
            {
                System.Console.WriteLine("ientifica que es un select");
                return Select();                
            }
            // IF
            if (t0 == "LTHAN" && t1 == "IF_KEYWORD")
            {
                System.Console.WriteLine("ientifica que es un if");
                return If();                
            }
            //FUNCTION
            if(t0 == "FN_KEYWORD")
            {
                System.Console.WriteLine("ientifica que es una funcion");
                return Function();
            }
            //SENTENCE NAMMED
            if (t0 == "ARROBA"){
                return NammedSentenceCall();
            }
            //FUNCTION CALL
            if (PeekType(0) == "IDENTIFIER" && PeekType(1) == "OPARENTHESIS")
            {
                return FunctionCallStmt();
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
                return VarDeclStmt(); // VarDeclStmt(): VarDeclStmt?

            // ASSIGN
            if (t0 == "IDENTIFIER" && t1 == "EQUAL")
                return Assign(); // Assign(): AssignStmt?

            System.Console.WriteLine("No coincide"+token);
            return null;
        }
        // ReturnStmt ::= "<" return "/>"
        public static ReturnStmt? ReturnStmt()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("RETURN_KEYWORD")) return null;
            var returnKeyWord = _tokenVector[_index-1];
            if (!Match("SLASH")) return null;
            if (!Match("MTHAN")) return null;
            return new()
            {
                ReturnKeyWord=returnKeyWord
            };
        }
    }
}