using System.Runtime.InteropServices;
using LadonLang.Data;
using LadonLang.Lexer;
using LadonLang.Parser;
using LadonLang.Parser.Models;
using LadonLang.Semantic;
/*
foreach (var row in TransitonMatrix.Matrix)
{
    foreach (var col in row)
    {
        System.Console.Write(col.ToString()+", ");
    }
    System.Console.WriteLine();
}
*/
string? line;
string currentDirectory="";
string archiveName = "Calls.ll";
int numLine=1;
bool next=true;
currentDirectory=Directory.GetCurrentDirectory();
currentDirectory=currentDirectory.Replace("\\", "/");
currentDirectory+="/Test";
StreamReader archive = new(@$"{currentDirectory}/{archiveName}");
Console.WriteLine("el directtorio es: "+currentDirectory);
int numCol=1;
while((line = archive.ReadLine())!= null ){
    next=Lexer.Scan(line,numLine++, numCol++);
}
/*
Console.WriteLine(Lexer.TokenVector.Count());

foreach (var item in Lexer.TokenVector.Where(x=>x.TokenType!="SPACE"))//Lexer.TokenVector)
{
    Console.WriteLine("¡"+item.TokenType.ToString()+"¡");
}*/

var vector = Lexer.TokenVector.Where(x=>x.TokenType!="SPACE").ToList();
var programNode = Parser.Parse(vector);
/*foreach (var item in programNode.Statements)
{
    Console.WriteLine(item);
}*/
var Semantic = new SemanticAnalyzer();
Semantic.Analyze(programNode);