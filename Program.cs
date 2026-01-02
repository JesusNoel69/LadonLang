using System.Runtime.InteropServices;
using LadonLang.CodeGenerator;
using LadonLang.Lexer;
using LadonLang.Parser;
using LadonLang.Semantic;

string? line;
string currentDirectory="";
string archiveName = "FileToC.ll";
int numLine=1;
bool next=true;
currentDirectory=Directory.GetCurrentDirectory();
currentDirectory=currentDirectory.Replace("\\", "/");
string directoryOut = currentDirectory;
currentDirectory+="/Test";
StreamReader archive = new(@$"{currentDirectory}/{archiveName}");
int numCol=1;
while((line = archive.ReadLine())!= null ){
    next=Lexer.Scan(line,numLine++, numCol++);
}
var vector = Lexer.TokenVector.Where(x=>x.TokenType!="SPACE").ToList();
var programNode = Parser.Parse(vector);

var Semantic = new SemanticAnalyzer();
Semantic.Analyze(programNode);

var generator = new CCodegenerator(programNode, directoryOut, Semantic.GetSymbolTable());
generator.TraverseAst();
