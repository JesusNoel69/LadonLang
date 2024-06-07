using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using LadonLang.Data;
namespace LadonLang
{
    public class Lexer
    {
        private static char[] _digits = ['0','1','2','3','4','5','6','7','8','9'];
        private static char[] _symbols = ['A',
        'b', 'B', 'c', 'C', 'D','E', 'F', 'G', 'h', 'H', 'I', 'j', 'J', 'k', 'K',
        'L', 'M','N','O','p','P', 'q', 'Q','R', 'S' ,'T','U', 'v', 'V', 'w', 'X','Y', 'z', 'Z','_'];
        private static char[] _delimiters=['-', ';',',','[', ']','(',')','#','=','+','%','&','/','|','*','!',' ', '@',':','<','>','.','\n',' ']; //---
        private static int _col=0;
        private static int _state=0;
        private static string _token="";
        private static string _typeToken="";
        private static List<Node> _tokenVector = [];
        public static List<Node> GetTokenVector(){
            return ImprovedTokenVector();
        }
        private static int _id=0;
        public static bool Scan(string? inputNormal, string? input,int numLine){ //input es lower
            ResetTokenInfo();
            
            if(input==null){return false;}
            if(inputNormal==null){return false;}
             for(int i=0; i<input.Length;i++){
                _col=SetColumn(_col,input, i);
                _token+=inputNormal[i];  
                if(!AcceptanceHandler(_state)) _state=TransitionMatrix.Matrix[_state][_col];
                else _state=0;
                if(ErrorHandler(_state)) return false;
                if(_delimiters.Contains(input[i])){ //si el ultmo caracter es un delimitador
                    if (!string.IsNullOrEmpty(_token) && _state!=5 )//verifica que no se anulo y que no pertencezca a las cadena el estado
                    {
                        //antes de agregar el token se verifica su estado final para ver que si se tiene el tipo de token adecuado
                        AcceptanceHandler(_state);
                        string aux="";
                        char last=' ';
                        int n=0;
                        foreach (char e in _token)
                        {
                            if(n<_token.Length-1){
                                aux+=e;
                                n++;
                            }
                            last=_token[n];
                        }
                        if(aux!="" && aux!=" "){
                        _tokenVector.Add(new Node(_id++,aux,_typeToken,numLine));
                        }
                        if(last!=' '){ //last!=" " && last!=""
                            _tokenVector.Add(new Node(_id++,last.ToString(),DelimiterType(last),numLine));
                        }
                        ResetTokenInfo();
                    }
                }   
            }
            return true;
        }
        
        private static readonly char[] _composedOperands=['-','=','+','%','&','/','|','*','<','>']; //---

        public static List<Node> ImprovedTokenVector(){
            List<Node> auxToken =[];
            string val="";
            int j=0;
            for (int i=0; i<_tokenVector.Count();i++){   
                if((i+2)<_tokenVector.Count()
                    &&Array.Exists(_composedOperands, elemento => elemento == _tokenVector[i].token[0])
                    &&Array.Exists(_composedOperands, elemento => elemento == _tokenVector[i+1].token[0]) 
                    && Array.Exists(_composedOperands, elemento => elemento == _tokenVector[i+2].token[0])
                    && _tokenVector[i].token.Length==1){
                val = _tokenVector[i].token[0].ToString() + _tokenVector[i+1].token[0].ToString()+_tokenVector[i+2].token[0].ToString();
                i+=2;
                // Console.WriteLine(val);
                auxToken.Add(new Node(j,val,"CONTEXT_TOKEN",_tokenVector[i].nLineas));
                }else{
                    if(Array.Exists(_composedOperands, elemento => elemento == _tokenVector[i].token[0])
                    &&Array.Exists(_composedOperands, elemento => elemento == _tokenVector[i+1].token[0])
                    && _tokenVector[i].token.Length==1){
                        val = _tokenVector[i].token[0].ToString() + _tokenVector[i+1].token[0].ToString();
                        i++;
                        Console.WriteLine(val);
                        auxToken.Add(new Node(j,val,ComposedOperators.TypeComposedOperator(val),_tokenVector[i].nLineas));
                    }else{
                        auxToken.Add(new Node(j,_tokenVector[i].token,_tokenVector[i].tipoToken,_tokenVector[i].nLineas));
                    }
                }
                j++;
            }
            return auxToken;
        }

        public static string DelimiterType(char character){
            if(character==((char)DelimiterSymbols.EQUAL)) return "EQUAL";
            if(character==((char)DelimiterSymbols.CPARENTHESIS)) return "CLOSE_PARENTHESIS";
            if(character==((char)DelimiterSymbols.OPARENTHESIS)) return "OPEN_PARENTHESIS";
            if(character==((char)DelimiterSymbols.OCORCHETES)) return "OPEN_CORCHETES";
            if(character==((char)DelimiterSymbols.CCORCHETES)) return "CLOSE_CORCHETES";
            if(character==((char)DelimiterSymbols.OKEY)) return "OPEN_KEY";
            if(character==((char)DelimiterSymbols.CKEY)) return "CLOSE_KEY";
            if(character==((char)DelimiterSymbols.ASTERSIC)) return "ASTERISK";
            if(character==((char)DelimiterSymbols.SLASH)) return "SLASH";
            if(character==((char)DelimiterSymbols.PERCENT)) return "PERCENT";
            if(character==((char)DelimiterSymbols.MINUS)) return "MINUS";
            if(character==((char)DelimiterSymbols.PLUS)) return "PLUS";
            if(character==((char)DelimiterSymbols.SHARP)) return "SHARP";
            if(character==((char)DelimiterSymbols.DIFFERENT)) return "DIFFERENT";
            if(character==((char)DelimiterSymbols.SEMICOLON)) return "SEMICOLON";
            if(character==((char)DelimiterSymbols.OR)) return "OR";
            if(character==((char)DelimiterSymbols.AND)) return "AND";
            if(character==((char)DelimiterSymbols.lTHAN)) return "LESS_THAN";
            if(character==((char)DelimiterSymbols.MTHAN)) return "MORE_THAN";
            if(character==((char)DelimiterSymbols.DOT)) return "DOT";
            if(character==((char)DelimiterSymbols.OPTIONAL)) return "OPTIONAL";
            if(character==((char)DelimiterSymbols.COMMA)) return "COMMA";
            if(character==((char)DelimiterSymbols.DDOT)) return "DOUBLE_DOT";
            return "Undefined"; 
        }
        public static int SetColumn(int _col, string input, int i){
            if('.'==input[i]) _col=4;
            else if('"'==input[i]) _col=5;
            else if(' '==input[i]) _col=3;
            else if('a'==input[i]) _col=6;
            else if('n'==input[i]) _col=7;
            else if('y'==input[i]) _col=8;
            else if('d'==input[i]) _col=9;
            else if('e'==input[i]) _col=10;
            else if('f'==input[i]) _col=11;
            else if('u'==input[i]) _col=12;
            else if('l'==input[i]) _col=13;
            else if('t'==input[i]) _col=14;
            else if('g'==input[i]) _col=15;
            else if('o'==input[i]) _col=16;
            else if('i'==input[i]) _col=17;
            else if('x'==input[i]) _col=18;
            else if('r'==input[i]) _col=19;
            else if('s'==input[i]) _col=20;
            else if('p'==input[i]) _col=21;
            else if('m'==input[i]) _col=22;
            else if('w'==input[i]) _col=23;
            else if(_delimiters.Contains(input[i])) _col=0;
            else if(_symbols.Contains(input[i])) _col=1;
            else if(_digits.Contains(input[i])) _col=2;
            return _col;
        }
        public static bool ErrorHandler(int _state){
            if(_state<0){
                    if(_state==-1) Console.WriteLine("Error al formar numeros enteros");
                    else if(_state==-2) Console.WriteLine("error al formar numeros reales");
                    else if(_state==-3) Console.WriteLine("Error al formar cadenas");   
                    else if(_state==-4) Console.WriteLine("Error al formar identificadores");   
                    else if(_state==-5) Console.WriteLine("Error al usar un simbolo");   
                return true; //hay un error
            }
            return false;//no hay error
        }
        public static bool AcceptanceHandler(int _state){
            if(_state>=99){
                if(_state==100) _typeToken="IDENTIFIER";
                else if(_state==200) _typeToken="NUMBER";
                else if(_state==300) _typeToken="DECIMAL_NUMBER";
                else if(_state==400) _typeToken="STRING";
                else if(_state==500) _typeToken="ANY";
                else if(_state==600) _typeToken="DEFAULT";
                else if(_state==700) _typeToken="FN";
                else if(_state==800) _typeToken="GO";
                else if(_state==900) _typeToken="IF";
                else if(_state==1000) _typeToken="INDEX";
                else if(_state==1100) _typeToken="INDEXFIRST";
                else if(_state==1200) _typeToken="ITER";
                else if(_state==1300) _typeToken="LONG";
                else if(_state==1400) _typeToken="LOOP";
                else if(_state==1500) _typeToken="NUM";
                else if(_state==1600) _typeToken="READ";
                else if(_state==1700) _typeToken="STR";
                else if(_state==1800) _typeToken="URL";
                else if(_state==1900) _typeToken="WRITE";
                else if(_state==2000) _typeToken="ENTITY";
                else if(_state==2100) _typeToken="OUT";
                return true; //estado final
            }
            return false; //aun no es estado final
        }
        private static void ResetTokenInfo(){
            _col=0;
            _state=0;
            _token = "";
            _typeToken = "";
        }
    }
}
