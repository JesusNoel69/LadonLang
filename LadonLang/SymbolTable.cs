namespace LadonLang
{
    public class SymbolTable(string name, string scope, string offset, int size)
    {
        public string  Name{get; set;}=name;//required
        public string? Type;
        public string Scope=scope;//alcance     required
        public string? DataType;
        public string[]? Parameters;
        public  string OffSet=offset;//required
        public int Size = size; //required
        public string? Context;
        public static void ShowTable(List<SymbolTable> symbols)
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Name       | Type       | Scope     | DataType   | OffSet  | Size   | Context       | Parameters        |");
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
            foreach (var symbol in symbols)
            {
                string parameters = symbol.Parameters != null ? string.Join(", ", symbol.Parameters) : "-";
                Console.WriteLine($"| {symbol.Name,-10} | {symbol.Type ?? "-", -10} | {symbol.Scope, -9} | {symbol.DataType ?? "-", -10} | {symbol.OffSet, -7} | {symbol.Size, -6} | {symbol.Context ?? "-", -12} | {parameters, -18} |");
            }
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        }

    }
}