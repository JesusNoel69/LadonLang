using System.Data;
using System.Reflection.Metadata;
using LadonLang.Data;
using LadonLang.Parser;

namespace LadonLang.Lexer
{
   
    public class Lexer
    {
        private static bool _insideString = false;
        private static bool _insideChar = false;

        //,' '
        private static char[] _delimiters=[' ','-', ';',',','[', ']','(',')','#','=','+','%','&','/','|','*','!','@',':','<','>','\n']; 
        private static readonly char[] _numbers=['0','1','2','3','4','5','6','7','8','9'];
        private static char[] _letters = ['A',
        'b', 'B', 'c', 'C', 'D','E', 'F', 'G', 'h', 'H', 'I', 'j', 'J', 'k', 'K',
        'L', 'M','N','O','p','P', 'q', 'Q','R', 'S' ,'T','U', 'v', 'V', 'w', 'X','Y', 'z', 'Z','_'];
        private static int _col=0;
        private static int _state=0;
        private static string _token="";
        private static string _tokenType="";
        public static List<Node> TokenVector {get;} = [];
        private static int _nodeId =0;
        public static bool AcceptHandler(int _state) => FinalStates.ContainsKey(_state);
        public static bool ErrorHandler(int _state)=> ErrorStates.ContainsKey(_state);
        
        public static bool Scan(string? input, int line)
        {
            //Console.WriteLine("¡" + input + "¡");
            if (input == null) return false;

            input += " "; // end of line
            ResetTokenInfo();

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (_insideString)
                {
                    if (ch == '"')
                    {
                        // Close String
                        _insideString = false;
                        _nodeId++;
                        Node n = new(_nodeId, _token, "STRING", line);
                        TokenVector.Add(n);
                        ResetTokenInfo();
                    }
                    else
                    {
                        _token += ch;
                    }
                    continue;
                }
                if (_insideChar)
                {
                    if (ch == '\'')
                    {
                        if (_token.Length != 1)
                        {
                            Console.WriteLine("Error: invalid CHAR literal.");
                            return false;
                        }
                        _insideChar = false;
                        _nodeId++;
                        Node n = new(_nodeId, _token, "CHARACTER", line);
                        TokenVector.Add(n);
                        ResetTokenInfo();
                    }
                    else
                    {
                        _token += ch;
                    }
                    continue;
                }
                if (ch == '"')
                {
                    _insideString = true;
                    _token = "";
                    continue; 
                }
                if (ch == '\'')
                {
                    _insideChar = true;
                    _token = "";
                    continue; 
                }
                Console.WriteLine($"Antes column: {_col} state: {_state} input: {ch}");
                _col = SetColumn(_col, ch);
                _state = TransitonMatrix.Matrix[_state][_col];
                Console.WriteLine($"Despues column: {_col} state: {_state} input: {ch}");
                bool isDelimiter = _delimiters.Contains(ch);
                if (ErrorHandler(_state))
                    return false;

                if (AcceptHandler(_state))
                {
                    _nodeId++;
                    _tokenType = FinalStates[_state];
                    //_token = _token.Trim();
                    //System.Console.WriteLine(_token);
                    Node newNode = new(_nodeId, _token, _tokenType, line);
                    TokenVector.Add(newNode);

                    ResetTokenInfo();
                    //maybe I need creat a special cases for all special characters not in delmiters but it has handle as delimiter
                    if (isDelimiter || ch=='.' )//&& !char.IsWhiteSpace(ch)
                    {
                        _nodeId++;
                        string delimLexeme = ch.ToString();
                        _tokenType = DelimiterType(ch);

                        newNode = new(_nodeId, delimLexeme, _tokenType, line);
                        TokenVector.Add(newNode);
                        ResetTokenInfo();
                    }

                    continue;
                }
                if (!isDelimiter)
                {
                    _token += ch;
                }
                else
                {
                    /*if (!char.IsWhiteSpace(ch))
                    {*/
                        _nodeId++;
                        string delimLexeme = ch.ToString();
                        _tokenType = DelimiterType(ch);

                        Node newNode = new(_nodeId, delimLexeme, _tokenType, line);
                        TokenVector.Add(newNode);
                    //}

                    ResetTokenInfo();
                }
            }

            if (_insideString)
            {
                Console.WriteLine("Error: string without closing \"");
                return false;
            }

            if (_insideChar)
            {
                Console.WriteLine("Error: char without closing '");
                return false;
            }
            if (_token.Length > 0 && !_insideString && !_insideChar)
            {
                if (AcceptHandler(_state))
                {
                    _nodeId++;
                    _tokenType = FinalStates[_state];
                    Node newNode = new(_nodeId, _token, _tokenType, line); //_token.Trim()
                    TokenVector.Add(newNode);
                }
                else
                {
                    _nodeId++;
                    Node newNode = new(_nodeId, _token, _tokenType, line); //_token.Trim()
                    TokenVector.Add(newNode);
                }
            }
            return true;
        }
        
        public static int SetColumn(int col, char input)
        {
            if(_delimiters.Contains(input)) col=0;
            else if(input=='a' || input=='A' ) col=1;
            else if(input=='b' || input=='B' ) col=2;
            else if(input=='c' || input=='C' ) col=3;
            else if(input=='d' || input=='D' ) col=4;
            else if(input=='e' || input=='E' ) col=5;
            else if(input=='f' || input=='F' ) col=6;
            else if(input=='g' || input=='G' ) col=7;
            else if(input=='h' || input=='H' ) col=8;
            else if(input=='i' || input=='I' ) col=9;
            else if(input=='k' || input=='K' ) col=10;
            else if(input=='l' || input=='L' ) col=11;
            else if(input=='m' || input=='M' ) col=12;
            else if(input=='n' || input=='N' ) col=13;
            else if(input=='o' || input=='O' ) col=14;
            else if(input=='p' || input=='P' ) col=15;
            else if(input=='r' || input=='R' ) col=16;
            else if(input=='s' || input=='S' ) col=17;
            else if(input=='t' || input=='T' ) col=18;
            else if(input=='u' || input=='U' ) col=19;
            else if(input=='v' || input=='V' ) col=20;
            else if(input=='y' || input=='Y' ) col=21;
            else if(input=='x' || input=='X' ) col=22;
            else if(_letters.Contains(input)) col=23;
            else if(input=='_' ) col=24;
            else if(_numbers.Contains(input)) col=25;
            else if(input=='\'' ) col=26;
            else if(input=='"' ) col=27;
            else if(input=='.' ) col=28;

            return col;
        }
        private static void ResetTokenInfo(){
            _col=0;
            _state=0;
            _token = "";
            _tokenType = "";
        }
        public static string DelimiterType(char character)
        {
            //System.Console.WriteLine("es: "+character+"¡");
            foreach (DelimiterSymbols s in Enum.GetValues(typeof(DelimiterSymbols)))
            {
                if ((char)s == character)
                    return s.ToString();
            }
            return "Undefined";
        }

        public static Dictionary<int, string> ErrorStates = new()
        {
            { -1, "Error at create integer" },
            { -2, "Error at create float" },
            { -3, "Error character expected '" },
            { -4, "Error at create identifier" },
        };
        public static Dictionary<int, string> FinalStates =new()
        {
            {1100,"IDENTIFIER"},
            {1200,"FLOAT_NUMBER"},
            {1300, "INTEGER_NUMBER"},
            {1400, "CHARACTER"},
            {1500, "STRING"},
            {1600, "AS_KEYWORD"},
            {1700, "BOOL_KEYWORD"},
            {1800, "CLASS_KEYWORD"},
            {1900, "CHAR_KEYWORD"},
            {2000, "DEFAULT_KEYWORD"},
            {2100, "ELSE_KEYWORD"},
            {2200, "ELIF_KEYWORD"},
            {2300, "FN_KEYWORD"},
            {2400, "FLOAT_KEYWORD"},
            {2500, "FIELD_KEYWORD"},
            {2600, "FALSE_KEYWORD"},
            {2700, "GET_KEYWORD"},
            {2800, "INPUT_KEYWORD"},
            {2900, "INT_KEYWORD"},
            {3000, "IN_KEYWORD"},
            {3100, "ID_KEYWORD"},
            {3200, "IF_KEYWORD"},
            {3300, "KEY_KEYWORD"},
            {3400, "LOOP_KEYWORD"},
            {3500, "METHOD_KEYWORD"},
            {3600, "NUMBER_KEYWORD"},
            {3700, "OUT_KEYWORD"},
            {3800, "OPTION_KEYWORD"},
            {3900, "OF_KEYWORD"},
            {4000, "OUTPUT_KEYWORD"},
            {4100, "STRING_KEYWORD"},
            {4200, "RETURN_KEYWORD"},
            {4300, "SELECT_KEYWORD"},
            {4400, "TRUE_KEYWORD"},
            {4500, "TEXT_KEYWORD"},
            {4600, "TYPE_KEYWORD"},
            {4700, "USE_KEYWORD"},
            {4800, "VAR_KEYWORD"},
            {4900, "VALUE_KEYWORD"},
            {5000, "PASS_KEYWORD"},
            {5100, "SET_KEYWORD"},
        };
    }
}