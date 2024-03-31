using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Schema;

namespace LadonLang
{
    public class Scanner
    {
        //ToDo: cambiar por _colecciones mas eficientes
        private static char[] _digits = ['0','1','2','3','4','5','6','7','8','9'];
        private static char[] _symbols = ['b', 'B', 'c', 'C', 'g', 'h', 'H', 'j', 'J', 'k', 'K', 'M', 'O', 'q', 'Q', 'T', 'v', 'V', 'w', 'X', 'Y', 'z', 'Z','_'];
        private static char[] _delimiters=['-', ';' , ' ', ',', '[', ']','(',')','#','=','+','%','&','/','|','*','!']; //---
        private static char[] _arithmeticOperators = ['+','-','*','/','%','=']; //"**, ++, --, +=, /=, *=, 
        private static string[] _relationalOperators = ["==","!=", "<","<=",">",">="];
        private static string[] _logicOperators = ["&&","||","!"];
        private static int _col;
        private static int _state;
        private static int _countDelimiters=0;
        public static int[][] Matrix=[
            [0,1,3,1,1,6,1,1,9,33,11,1,13,1,1,1,1,1,1,1,27,1,1,1,1,36,1,40,43,1,1,1,50,53],
            [100,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [200,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [300,-1,3,-1,4,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1],
            [1,-2,5,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [6,-2,5,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2],
            [1,1,1,-3,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,8,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [500,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,10,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [600,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,12,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [700,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,15,1,14,1,1,1,1,1,1,1,24,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [800,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,16,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,17,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,18,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [900,1,1,-3,1,1,1,1,19,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,20,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,21,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,22,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,23,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1000,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,25,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,26,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1100,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,28,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,29,1,1,1,1,31,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,30,1,1,1,1,1,1,1,1,1,1,1,1],
            [1200,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,32,1,1,1,1,1,1,1,1,1,1,1],
            [1300,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,34,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,35,1,1,1,1,1,1,1,1,1],
            [1400,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,37,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,38,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,39,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1500,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,41,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,42,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1600,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,44,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,45,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,46,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,47,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,48,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,49,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1700,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,51,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,52,1,1],
            [1800,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,54,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,55,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,56,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1,1,1,-3,1,1,1,1,1,1,1,1,1,1,57,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            [1900,1,1,-3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
        ];
        public static void Scan(string? input, int numLine){
            _col=0;
            _state=0;
            
            for(int i=0; i<input.Length;i++){
                _col=SetColumn(_col,input, i);
                if(!AcceptanceHandler(_state)){
                    _state=Matrix[_state][_col];                    
                }else{
                    // _state=0;
                    _countDelimiters=0;
                    if(!CharIn(_delimiters,input[i])){
                        _state=0;
                        _state=Matrix[_state][_col];
                    }
                }
                    Console.WriteLine($"state{_state} y col{_col}");
                //ToDo: retirar al programar el vector de tokens
                AcceptanceHandler(_state); //para identificar al ultimo
                if(ErrorHandler(_state)){
                   break;
                }
            }      
        }
        //ToDo: usar enums para representar los estados de aceptacion 
        public static bool AcceptanceHandler(int _state){
            if(_state>=100){
                if(_countDelimiters==0){//se verifica que solo exista un dentificador al final para cuando este el vector de tokens, aunque halla mas no los guardara
                    if(_state==100){
                        System.Console.WriteLine("Identifier");
                    }else if(_state==200){
                        System.Console.WriteLine("String");
                    }else if(_state==300){
                        System.Console.WriteLine("Number");
                    }else if(_state==400){
                        System.Console.WriteLine("Decimal Number");
                    }else if(_state==500){
                        System.Console.WriteLine("Any");
                    }else if(_state==600){
                        System.Console.WriteLine("FN");
                    }else if(_state==700){
                        System.Console.WriteLine("Go");
                    }else if(_state==800){
                        System.Console.WriteLine("If");
                    }else if(_state==900){
                        System.Console.WriteLine("Index");
                    }else if(_state==1000){
                        System.Console.WriteLine("IndexFirst");
                    }else if(_state==1100){
                        System.Console.WriteLine("Iter");
                    }else if(_state==1200){
                        System.Console.WriteLine("Long");
                    }else if(_state==1300){
                        System.Console.WriteLine("Loop");
                    }else if(_state==1400){
                        System.Console.WriteLine("Num");
                    }else if(_state==1500){
                        System.Console.WriteLine("Read");
                    }else if(_state==1600){
                        System.Console.WriteLine("Str");
                    }else if(_state==1700){
                        System.Console.WriteLine("Default");
                    }else if(_state==1800){
                        System.Console.WriteLine("Url");
                    }else if(_state==1900){
                        System.Console.WriteLine("Write");
                    }
                _countDelimiters++;

                }
                return true; //se acepta
            }
            _countDelimiters=0;
            return false; //continua 
        }
        
        public static int SetColumn(int _col, string input, int i){
            if(CharIn(_delimiters,input[i])){
                _col=0;
            }else if(CharIn(_symbols,input[i])){
                _col=1;
            }else if(CharIn(_digits,input[i])){
                _col=2;
            }else if(CharIn(['"'],input[i])){
                _col=3;
            }else if(CharIn(['.'],input[i])){
                _col=4;
            }else if(CharIn(['A'],input[i])){
                _col=5;
            }else if(CharIn(['n'],input[i])){
                _col=6;
            }else if(CharIn(['y'],input[i])){
                _col=7;
            }else if(CharIn(['F'],input[i])){
                _col=8;
            }else if(CharIn(['N'],input[i])){
                _col=9;
            }else if(CharIn(['G'],input[i])){
                _col=10;
            }else if(CharIn(['o'],input[i])){
                _col=11;
            }else if(CharIn(['I'],input[i])){
                _col=12;
            }else if(CharIn(['d'],input[i])){
                _col=13;
            }else if(CharIn(['e'],input[i])){
                _col=14;
            }else if(CharIn(['x'],input[i])){
                _col=15;
            }else if(CharIn(['t'],input[i])){
                _col=16;
            }else if(CharIn(['i'],input[i])){
                _col=17;
            }else if(CharIn(['r'],input[i])){
                _col=18;
            }else if(CharIn(['s'],input[i])){
                _col=19;
            }else if(CharIn(['L'],input[i])){
                _col=20;
            }else if(CharIn(['g'],input[i])){
                _col=21;
            }else if(CharIn(['p'],input[i])){
                _col=22;
            }else if(CharIn(['u'],input[i])){
                _col=23;
            }else if(CharIn(['m'],input[i])){
                _col=24;
            }else if(CharIn(['R'],input[i])){
                _col=25;
            }else if(CharIn(['a'],input[i])){
                _col=26;
            }else if(CharIn(['S'],input[i])){
                _col=27;
            }else if(CharIn(['D'],input[i])){
                _col=28;
            }else if(CharIn(['f'],input[i])){
                _col=29;
            }else if(CharIn(['u'],input[i])){
                _col=30;
            }else if(CharIn(['U'],input[i])){
                _col=31;
            }else if(CharIn(['W'],input[i])){
                _col=32;
            }
            return _col;
        }
       //ToDo: usar enum para representar los estados de error
       //ToDo: usar un diccionario para representar los mensajes de error segun su estado de error
        public static bool ErrorHandler(int _state){
            if(_state<0){
                if(_state==-1)
                    Console.WriteLine("error al formar numeros enteros");
                else if(_state==-2)
                    Console.WriteLine("error al formar numeros reales");
                else if(_state==-3)
                    Console.WriteLine("Error al formar cadenas");
                return true; //hay un error
            }
            return false;//no hay error
        }
        public static bool CharIn(char[] alphabet, char character){
            for(int j=0; j<alphabet.Length; j++){
                if(character == alphabet[j]){
                    return true;
                }
            }
            return false;
        } 
    }
}
