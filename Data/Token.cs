using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Data
{
    public class Token(int id, string token, string tokenType, int lines, int columns)
    {
        public int Id = id;
        public string Lexeme = token;
        public string TokenType = tokenType;
        public int Lines = lines;
        public int Columns = columns;

        public void Listar(){
            Console.WriteLine($"Id: {Id}\nToken: {Lexeme}\nTipo de token: {TokenType}\nNumero de linea: {Lines}\n--------------------------------------------------");
        }
    }
}