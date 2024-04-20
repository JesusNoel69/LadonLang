namespace LadonLang
{
    public class Scanner
    {
        //
        private static char[] _digits = ['0','1','2','3','4','5','6','7','8','9'];
        private static char[] _symbols = ['A',
        'b', 'B', 'c', 'C', 'D','E', 'F', 'G', 'h', 'H', 'I', 'j', 'J', 'k', 'K',
        'L', 'M','N','O','p','P', 'q', 'Q','R', 'S' ,'T','U', 'v', 'V', 'w', 'X','Y', 'z', 'Z','_'];
        // private static char[] _arithmeticOperators = ['+','-','*','/','%','=']; //"**, ++, --, +=, /=, *=, 
        // private static string[] _relationalOperators = ["==","!=", "<","<=",">",">="];
        // private static string[] _logicOperators = ["&&","||","!"];
        private static char[] _delimiters=['-', ';',',','[', ']','(',')','#','=','+','%','&','/','|','*','!',' ', '@']; //---
        private static int _col=0;
        private static int _state=0;
        private static string _token="";
        private static string _typeToken="";
        public static List<Node> tokenVector = [];
        private static int _id=0;
        //ToDo: los -200 de la columna de las cadenas ver si se reemplazan por el error de formar cadenas
        public static int[][] transitionMatrix = [
                    [0,1,2,0,-200,5,7,41,1,10,59,17,51,35,1,19,1,21,1,44,48,1,1,54],
            [100,1,1,100,-200,-4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [200,-1,2,200,3,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1],
            [-2,-2,4,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [300,-2,4,300,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [5,5,5,5,5,6,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5],
            [400,-3,-3,5,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3,-3],
            [100,1,1,100,-200,-4,1,8,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-4,1,1,9,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [500,1,1,500,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,11,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,12,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,13,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,14,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,15,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,16,1,1,1,1,1,1,1,1,1],
            [600,1,1,600,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,18,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [700,1,1,700,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,20,1,1,1,1,1,1,1],
            [800,1,1,800,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,23,1,1,1,22,1,1,32,1,1,1,1,1,1,1,1,1],
            [900,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,24,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,25,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,26,1,1,1,1,1],
            [1000,1,1,1000,-200,-200,1,1,1,1,1,27,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,28,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,29,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,30,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,31,1,1,1,1,1,1,1,1,1],
            [1100,1,1,1100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,33,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,34,1,1,1,1],
            [1200,1,1,1200,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,36,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,37,1,1,1,1,1,1,1,1,39,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,38,1,1,1,1,1,1,1,1],
            [1300,1,1,1300,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,40,1,1],
            [1400,1,1,1400,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,42,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,43,1],
            [1500,1,1,1500,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,45,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,46,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,47,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1600,1,1,1600,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,49,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,50,1,1,1,1],
            [1700,1,1,1700,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,52,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,53,1,1,1,1,1,1,1,1,1,1],
            [1800,1,1,1800,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,55,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,56,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,57,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,58,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1900,1,1,1900,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,60,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,61,1,1,1,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,1,1,1,62,1,1,1,1,1,1],
            [100,1,1,100,-200,-200,1,1,1,1,1,1,1,1,63,1,1,1,1,1,1,1,1,1],
           [100,1,1,100,-200,-200,1,1,64,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [2000,1,1,2000,-200,-200,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
                ];

        public static int[][] TransitionMatrix { get => transitionMatrix; set => transitionMatrix = value; }
        public static void Scan(string? inputNormal, string? input,int numLine){ //input es lower
            // Console.WriteLine(input);
            _col=0;
            _state=0;
            _token = "";
            _typeToken = "";
             for(int i=0; i<input.Length;i++){
                _col=SetColumn(_col,input, i);
                _token+=inputNormal[i];  
                if(!AcceptanceHandler(_state)) _state=TransitionMatrix[_state][_col];
                else _state=0;
                if(ErrorHandler(_state)) return;
                if(_delimiters.Contains(input[i])){ //si el ultmo caracter es un delimitador
                    if (!string.IsNullOrEmpty(_token) && _state!=5 )//verifica que no se anulo y que no pertencezca a las cadena el estado
                    {
                        //antes de agregar el token se verifica su estado final para ver que si se tiene el tipo de token adecuado
                        AcceptanceHandler(_state);
                        string aux="";
                        string last="";
                        int n=0;
                        foreach (char e in _token)
                        {
                            if(n<_token.Length-1){
                                aux+=e;
                                n++;
                            }
                            last=_token[n].ToString();
                        }
                        // Console.WriteLine("aux -"+aux+"-");
                        // Console.WriteLine("last -"+last+"-");
                        if(aux!="" && aux!=" "){
                            tokenVector.Add(new Node(_id++,aux,_typeToken,numLine));
                        }
                         if(last!="" && last!=" "){
                            tokenVector.Add(new Node(_id++,last,"Delimiter",numLine));
                        }
                        _token = "";
                        _state=0;
                    }
                }   
            }
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
        //ToDo: verificar que lo errores correspondan y ver si es necesario el usar un enum para definirlos
        public static bool ErrorHandler(int _state){
            if(_state<0){
                    if(_state==-1) Console.WriteLine("error al formar numeros enteros");
                    else if(_state==-2) Console.WriteLine("error al formar numeros reales");
                    else if(_state==-3) Console.WriteLine("Error al formar cadenas");   
                    else if(_state==-4) Console.WriteLine("Error al formar identificadores");   
                return true; //hay un error
            }
            return false;//no hay error
        }
        public static bool AcceptanceHandler(int _state){
            if(_state>=99){
                if(_state==100) _typeToken="identifier";
                else if(_state==200) _typeToken="Number";
                else if(_state==300) _typeToken="Decimal Number";
                else if(_state==400) _typeToken="String";
                else if(_state==500) _typeToken="Any";
                else if(_state==600) _typeToken="Default";
                else if(_state==700) _typeToken="FN";
                else if(_state==800) _typeToken="GO";
                else if(_state==900) _typeToken="IF";
                else if(_state==1000) _typeToken="Index";
                else if(_state==1100) _typeToken="IndexFirst";
                else if(_state==1200) _typeToken="Iter";
                else if(_state==1300) _typeToken="Long";
                else if(_state==1400) _typeToken="Loop";
                else if(_state==1500) _typeToken="Num";
                else if(_state==1600) _typeToken="Read";
                else if(_state==1700) _typeToken="Str";
                else if(_state==1800) _typeToken="Url";
                else if(_state==1900) _typeToken="Write";
                else if(_state==2000) _typeToken="Entity";
                return true; //estado final
            }
            return false; //aun no es estado final
        }
    }
}