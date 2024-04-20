namespace LadonLang
{
    public class Node
    {
        public Node(int id, string token, string tipoToken, int lineas){
            this.idNodo=id;
            this.token=token;
            this.nLineas=lineas;
            this.tipoToken=tipoToken;
        }
        public int idNodo;
        public string token;
        public string tipoToken;
        public int nLineas;
        public void Listar(){
            Console.WriteLine($"Id: {idNodo}\nToken: {token}\nTipo de token: {tipoToken}\nNumero de linea: {nLineas}\n---");
        }
    }
}