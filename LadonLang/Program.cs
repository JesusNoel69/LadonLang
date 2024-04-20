using LadonLang;
string? line, lineLower;
int numLine=1;
StreamReader archive = new StreamReader(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/Initial.ll");
while((line = archive.ReadLine())!= null){

    lineLower=line.ToLower();
    Scanner.Scan(line,lineLower,numLine++);
}
Console.WriteLine("=======");
foreach (var item in Scanner.tokenVector)
{
    item.Listar();
}

