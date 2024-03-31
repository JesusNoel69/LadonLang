using LadonLang;
//Console.WriteLine(TransitionMatrix.Matrix[0][2]);
// string? input = Console.ReadLine();
string? line;
int numLine=1;
StreamReader archive = new StreamReader(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/Initial.ll");
while((line = archive.ReadLine())!= null){
    Console.WriteLine(line);
    Scanner.Scan(line,numLine);
    numLine++;
}