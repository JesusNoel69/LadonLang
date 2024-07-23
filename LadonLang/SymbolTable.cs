namespace LadonLang
{
    public class SymbolTable
    {
        public string? Name{get; set;}="-";//required
        public string? Type{get; set;}="-";
        public string? Scope{get; set;}="-";//alcance     required
        public string? DataType{get; set;}="-";
        public List<string>? Parameters{get; set;}=[];
        public  IntPtr? OffSet{get; set;}=null;//required
        public int? Size{get; set;}=null; //required
        public List<string>? Context{get; set;}=[];
        public string? ExtraData{get; set;}="-";
        public static void ShowTable(List<SymbolTable> symbols)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Name       | Type              | Scope     | DataType   | OffSet  | Size   | Context         | Parameters                     | Extra D                   |");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------");
            foreach (var symbol in symbols)
            {
                string parameters = symbol.Parameters != null ? string.Join(", ", symbol.Parameters) : "-";
                string context = "-";
                symbol.Context?.ForEach((eachContext)=>{
                    context+=eachContext+"->";
                });
                string? offset = symbol.OffSet != null ? symbol.OffSet.ToString() : "-";
                Console.WriteLine($"| {symbol.Name,-10} | {symbol.Type ?? "-", -17} | {symbol.Scope, -9} | {symbol.DataType ?? "-", -10} | {offset, -7} | {symbol.Size, -6} | { context, -15} | {parameters, -30} | {symbol.ExtraData, -25} |");
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }

    }
}