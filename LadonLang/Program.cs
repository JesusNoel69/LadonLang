﻿using LadonLang;
// using LadonLangAST;
string? line, lineLower;
int numLine=1;
bool next=true;
List<Node> tokenVector=[];
    Console.WriteLine("empezando");
//C:\Users\hp\Documents\Proyectos\Language Programing\LadonLang\Archives\InitialLexer.ll
StreamReader archive = new(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/LexerToParserDeclarations.ll");
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
    Console.WriteLine("=======");
    AstConstructor ast = new(tokenVector);
    //
    List<SymbolTable> table=[];
    ast.Start(ref table);
    //
    Console.WriteLine(ast.root.Count+" hola");
    // foreach(var t in ast.root){
    //     t.Print();
    // }  
    // SymbolTable.ShowTable(table);
    SemanticAnalizer astSemanticAnalysis = new(ref table,ref ast);
    astSemanticAnalysis.Analize();
}