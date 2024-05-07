using LadonLang;
string? line, lineLower;
int numLine=1;
bool next=true;

StreamReader archive = new StreamReader(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/Initial.ll");
while((line = archive.ReadLine())!= null && next==true){
    lineLower=line.ToLower();
    next=Lexer.Scan(line,lineLower,numLine++);
}
if(next){
    List<Node> tokenVector = Lexer.GetTokenVector();
    Console.WriteLine("Vector de tokens");
    Console.WriteLine("=======");
    foreach (var token in tokenVector)
    {
        token.Listar();
    }
    Console.WriteLine("=======");
}