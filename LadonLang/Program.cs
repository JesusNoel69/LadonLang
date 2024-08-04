using System.Diagnostics;
using LadonLang;
string? line, lineLower;
int numLine=1;
bool next=true;
List<Node> tokenVector;
//Directory.GetCurrentDirectory()
// dotnet run -Compile archiveName.ll 
// dotnet run -Compile -Path "path/.../archiveName.ll" 
string compile=args[0];
string archiveName=args[1];
string currentDirectory="";


List<SymbolTable> table=[];
if(compile =="-Compile"){

    if(archiveName!="-Path"){
        currentDirectory=Directory.GetCurrentDirectory();
        currentDirectory=currentDirectory.Replace("\\", "/");

    }else{
        string currentDirectoryComplete=args[2];
        currentDirectoryComplete=currentDirectoryComplete.Replace("\\", "/");

        string[] pathParts = currentDirectoryComplete.Split('/');
        currentDirectory = string.Join('/', pathParts[..^1]);
        currentDirectory+="/";
        System.Console.WriteLine("el diirectorio: "+currentDirectory);
        archiveName = pathParts[^1];
    }

    if (!Directory.Exists(currentDirectory+"Compiled/"))
    {
        Console.WriteLine(args[1]);
        // Crear el directorio
        CreateDirectory(currentDirectory+"Compiled/");
        Console.WriteLine($"Directorio creado: {currentDirectory}");
    }
}
void CreateDirectory(string path)
{
    // Crear el proceso para ejecutar el comando mkdir
    ProcessStartInfo processInfo = new("cmd.exe", $"/c mkdir \"{path}\"")
    {
        CreateNoWindow = true,
        UseShellExecute = false
    };
    Process? process = Process.Start(processInfo);
    process?.WaitForExit();
}


//archives is a temporal file only for test in develop
StreamReader archive = new(@$"{currentDirectory}/{archiveName}");
while((line = archive.ReadLine())!= null && next==true){
    //se le agrega un espacio en la linea para que reconozca en caso de que no lleve ; pero siga siendo parte de otra instruccion consecuente
    line+=' ';
    lineLower=line.ToLower();
    next=Lexer.Scan(line,lineLower,numLine++);
}
if(next){
    tokenVector = Lexer.GetTokenVector();
    Parser.Structure(tokenVector);
    AstConstructor ast = new(tokenVector);
    ast.Start(ref table);
    //
    SemanticAnalyzer astSemanticAnalysis = new(ref table,ref ast);
    astSemanticAnalysis.Analize();

    CodeGenerator code = new(ref ast, ref table, currentDirectory+"Compiled/");
    code.TraverseAst();

    string source=currentDirectory+"Compiled/main.cpp";
    string outPut=currentDirectory+"Compiled/program.exe";

    CompileCppProgram(source,outPut);
    //ExecuteProgram(outPut);
    ProcessStartInfo info = new ()
    {
        UseShellExecute = true,
        FileName = "program.exe",
        WorkingDirectory = currentDirectory+"Compiled/"
    };

    Process.Start(info);

}


void CompileCppProgram(string sourceFilePath, string outputFilePath)
{
    string compilerPath = "g++";
    string arguments = $"-o \"{outputFilePath}\" \"{sourceFilePath}\"";

System.Console.WriteLine(arguments);
    ProcessStartInfo processInfo = new()
    {
        FileName = compilerPath,
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process? process = Process.Start(processInfo))
    {
        string? output = process?.StandardOutput.ReadToEnd();
        string? error = process?.StandardError.ReadToEnd();
        process?.WaitForExit();

        if (process?.ExitCode == 0)
        {
            Console.WriteLine("Compilación exitosa.");
        }
        else
        {
            Console.WriteLine("Error en la compilación.");
            Console.WriteLine("Salida: " + output);
            Console.WriteLine("Errores: " + error);
        }
    }
}
void ExecuteProgram(string filePath)
{
    ProcessStartInfo processRun = new()
    {
        FileName = filePath,
        RedirectStandardOutput = false,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process? process = Process.Start(processRun))
    {
        // Leer la salida estándar
        string? output = process?.StandardOutput.ReadToEnd();
        // Leer los errores estándar
        string? error = process?.StandardError.ReadToEnd();

        // Esperar a que el proceso termine
        process?.WaitForExit();

        // Mostrar la salida y los errores en la consola
        Console.WriteLine("Salida del programa:");
        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine("Errores del programa:");
            Console.WriteLine(error);
        }
    }
}