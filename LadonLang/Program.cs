using LadonLang;
string? line, lineLower;
int numLine=1;
bool next=true;
List<Node> tokenVector=[];
    Console.WriteLine("empezando");
//C:\Users\hp\Documents\Proyectos\Language Programing\LadonLang\Archives\InitialLexer.ll
StreamReader archive = new StreamReader(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/LexerToParserDeclarations.ll");
while((line = archive.ReadLine())!= null && next==true){
    Console.WriteLine("empezando");
    //se le agreg un espacio en la linea para que reconozca en caso de que no lleve ; pero siga siendo parte de otra instruccion consecuente
    line+=' ';
    lineLower=line.ToLower();
    next=Lexer.Scan(line,lineLower,numLine++);
}

if(next){
    tokenVector = Lexer.GetTokenVector();
    // Console.WriteLine("Vector de tokens");
    Console.WriteLine("=======");
    foreach (var token in tokenVector)
    {
        token.Listar();
    }
    Console.WriteLine("=======");
    Console.WriteLine("empezando");
    Parser.Structure(tokenVector);
        // foreach (var statement in Parser._statements)
        // {
        //     statement.Print();
        //     Console.WriteLine(); // asegura una nueva línea despues de la impresion de cada expresion
        // }
    System.Console.WriteLine("terminado");
}