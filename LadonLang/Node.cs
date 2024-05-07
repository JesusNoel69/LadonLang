namespace LadonLang
{
    public class Node(int id, string token, string tipoToken, int lineas)
    {
        public int idNodo = id;
        public string token = token;
        public string tipoToken = tipoToken;
        public int nLineas = lineas;
        public void Listar(){
            Console.WriteLine($"Id: {idNodo}\nToken: {token}\nTipo de token: {tipoToken}\nNumero de linea: {nLineas}\n--------------------------------------------------");
        }
    }
}