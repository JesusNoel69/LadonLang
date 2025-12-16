using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Data
{
    public class Node(int id, string token, string tokenType, int lines)
    {
        public int Id = id;
        public string Token = token;
        public string TokenType = tokenType;
        public int Lines = lines;
        public void Listar(){
            Console.WriteLine($"Id: {Id}\nToken: {Token}\nTipo de token: {TokenType}\nNumero de linea: {Lines}\n--------------------------------------------------");
        }
    }
}