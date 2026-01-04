using System.Diagnostics;
using System.Text;
using LadonLang.CodeGenerator;
using LadonLang.Lexer;
using LadonLang.Parser;
using LadonLang.Semantic;
/*
ladon compile file.ll (generate and compile)
ladon run file.ll (compile and execute)
ladon gen file.ll (only generate ++ and it goes out)
flags:
    -o <dir> out (default: ./out)
    --cpp <path> for use anothe compiler (default: g++)
    --keep no delete temporals
    --no-build (equivalent to gen)
*/
//dotnet run -- compile Test/fibonacci.ll -o ../fibonacci
//dotnet run -- compile Test/factorial.ll -o ../factorial
try
{
    var opt = ParseArgs(args);
    var cppPath = RunLadonPipeline(opt.InputFile, opt.OutDir);
    if (opt.Command == CommandKind.Gen)
    {
        Console.WriteLine($"Generated: {cppPath}");
        return 0;
    }
    //compile
    var exePath = Path.Combine(opt.OutDir, "program.exe");
    Console.WriteLine("Running: " + exePath);

    CompileCpp(opt.Compiler, cppPath, exePath);

    //execute if it's indicated
    if (opt.Command == CommandKind.Run)
        RunExe(exePath, opt.OutDir);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

static CliOptions ParseArgs(string[] args)
{
    if (args.Length < 2)
        throw new ArgumentException(
            "Usage:\n" +
            "  ladon compile <file.ll> [-o <outDir>] [--cpp <compiler>]\n" +
            "  ladon run     <file.ll> [-o <outDir>] [--cpp <compiler>]\n" +
            "  ladon gen     <file.ll> [-o <outDir>]\n"
        );

    CommandKind cmd = args[0].ToLowerInvariant() switch
    {
        "compile" => CommandKind.Compile,
        "run"     => CommandKind.Run,
        "gen"     => CommandKind.Gen,
        _ => throw new ArgumentException($"Unknown command '{args[0]}'. Use: compile | run | gen")
    };

    string input = args[1];
    if (!File.Exists(input))
        throw new FileNotFoundException($"Input file not found: {input}");

    string outDir = Path.GetFullPath("./out");
    string compiler = "g++";

    for (int i = 2; i < args.Length; i++)
    {
        var a = args[i];
        if (a == "-o" && i + 1 < args.Length)
        {
            outDir = Path.GetFullPath(args[++i]);
        }
        else if (a == "--cpp" && i + 1 < args.Length)
        {
            compiler = args[++i];
        }
        else
        {
            throw new ArgumentException($"Unknown argument: {a}");
        }
    }
    Directory.CreateDirectory(outDir);
    return new CliOptions(cmd, Path.GetFullPath(input), outDir, compiler);
}

static string RunLadonPipeline(string inputFile, string outDir)
{
    string? line;
    int numLine=1;
    bool next=true;
    int numCol=1;
    StreamReader archive = new(inputFile);
    while((line = archive.ReadLine())!= null ){
        next=Lexer.Scan(line,numLine++, numCol++);
    }

    var vector = Lexer.TokenVector.Where(x=>x.TokenType!="SPACE").ToList();
    var programNode = Parser.Parse(vector);

    var Semantic = new SemanticAnalyzer();
    Semantic.Analyze(programNode);

    var generator = new CCodegenerator(programNode, outDir, Semantic.GetSymbolTable());
    generator.TraverseAst();
    var cpp = Path.Combine(outDir, "main.cpp");
    if (!File.Exists(cpp))
    {
        var alt = Path.Combine(outDir, "out", "main.cpp");
        if (File.Exists(alt)) cpp = alt;
        else throw new FileNotFoundException("Generated C++ file not found. Check code generator output path.");
    }
    return cpp;
}

static void CompileCpp(string compilerArg, string source, string outputExe)
{
    var compiler = FindCppCompiler(compilerArg) 
        ?? throw new Exception("No se encontró compilador C++ (g++ / clang++). Instala MSYS2/LLVM o pasa la ruta con --cpp.");

    Directory.CreateDirectory(Path.GetDirectoryName(outputExe)!);

    var args = $"-std=c++17 -O2 \"{source}\" -o \"{outputExe}\"";

    var psi = new ProcessStartInfo
    {
        FileName = compiler,
        Arguments = args,
        WorkingDirectory = Path.GetDirectoryName(outputExe)!, // o Path.GetDirectoryName(source)!
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        StandardOutputEncoding = Encoding.UTF8,
        StandardErrorEncoding = Encoding.UTF8,
    };

    // Importante en Windows+MSYS2: asegurar que el bin del compilador esté en PATH del proceso.
    var binDir = Path.GetDirectoryName(compiler);
    if (!string.IsNullOrWhiteSpace(binDir))
    {
        var oldPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        psi.Environment["PATH"] = binDir + ";" + oldPath;
    }

    Console.WriteLine($"Compiler: {compiler}");
    Console.WriteLine($"Args: {args}");

    using var p = Process.Start(psi) ?? throw new Exception("Failed to start compiler process.");
    var stdout = p.StandardOutput.ReadToEnd();
    var stderr = p.StandardError.ReadToEnd();
    p.WaitForExit();

    if (!string.IsNullOrWhiteSpace(stdout))
        Console.WriteLine(stdout);

    if (!string.IsNullOrWhiteSpace(stderr))
        Console.Error.WriteLine(stderr);

    if (p.ExitCode != 0)
        throw new Exception($"C++ compilation failed (exit {p.ExitCode}).");
    CopyMsysRuntimeDlls(compiler, outputExe);
    Console.WriteLine("Compilation OK.");
}

static void CopyMsysRuntimeDlls(string compilerPath, string outputExe)
{
    var binDir = Path.GetDirectoryName(compilerPath)!;
    var outDir = Path.GetDirectoryName(outputExe)!;

    string[] dlls =
    {
        "libstdc++-6.dll",
        "libgcc_s_seh-1.dll",
        "libwinpthread-1.dll"
    };

    foreach (var dll in dlls)
    {
        var src = Path.Combine(binDir, dll);
        var dst = Path.Combine(outDir, dll);
        if (File.Exists(src))
            File.Copy(src, dst, overwrite: true);
    }
}

static void RunExe(string exePath, string workingDir)
{
    var psi = new ProcessStartInfo
    {
        FileName = exePath,
        WorkingDirectory = workingDir,
        UseShellExecute = false
    };
    Process.Start(psi);
}
static string? FindCppCompiler(string preferred)
{
    if (!string.IsNullOrWhiteSpace(preferred) && File.Exists(preferred))
        return preferred;
    if (ExistsOnPath(preferred)) return preferred;
    if (ExistsOnPath("clang++")) return "clang++";
    if (ExistsOnPath("g++")) return "g++";

    // paths of MSYS2 
    var candidates = new[]
    {
        @"C:\msys64\clang64\bin\clang++.exe",
        @"C:\msys64\mingw64\bin\g++.exe",
        @"C:\msys64\mingw32\bin\g++.exe",
        @"C:\msys64\ucrt64\bin\g++.exe",
        @"C:\msys64\ucrt64\bin\clang++.exe",
    };

    foreach (var c in candidates)
        if (File.Exists(c)) return c;

    return null;
}

static bool ExistsOnPath(string exe)
{
    var paths = (Environment.GetEnvironmentVariable("PATH") ?? "")
        .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

    foreach (var p in paths)
    {
        var full = Path.Combine(p.Trim(), exe);
        if (File.Exists(full)) return true;
        if (!exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(full + ".exe"))
            return true;
    }
    return false;
}

enum CommandKind { Compile, Run, Gen }
sealed record CliOptions(
    CommandKind Command,
    string InputFile,
    string OutDir,
    string Compiler
);

/*
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
*/