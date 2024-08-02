using System.Reflection;
using LadonLang.Data;
namespace LadonLang
{
    public class CodeGenerator(ref AstConstructor ast, ref List<SymbolTable> symbolTable)
    {
        private readonly List<ASTNode> ast = ast.root;
        private List<SymbolTable> symbolTable = symbolTable;
        private List<string> context=[];
        public const string l ="\n";
        private string directives=$"#include <iostream> {l}#include <fstream>{l}#include <string>{l}using namespace std;{l}";
        //public string Header =$"void _Read(const string& , const string &);{l}template<typename T>{l} T _Read();{l}void _Write(const string &);{l}";
        
        public string Header =$"void _Read(const string& , const string &);{l}void _Write(const string &);{l}";
        public string principalBlock="";
        // static string templateFunctions=$"template<typename T>{l}T _Read(){l} {{ {l}T inp;{l}cin>> inp;{l}return inp;{l} }}{l}template<>{l}string _Read(){l} {{ {l}string inp;{l}cin>> inp;{l}return inp;{l} }}{l}";
        public string functions=$"void _Write(const string &url){{ {l}ifstream file(url);{l}if (!file) {{ {l}cerr << \"Error al abrir el archivo para leer.\\n\";{l}return;{l} }}{l} string line;{l} while (getline(file, line)) {{ {l}cout << line << '\\n';{l} }}{l}file.close();{l} }}{l}void _Read(const string& content,const string &Url) {{ {l}ofstream file(Url, ios::app);{l}if (!file) {{ {l}cerr << \"Error al abrir el archivo para escribir.\"; {l}return;{l} }}{l}file << content << '\\n';{l}file.close(); {l} }} {l}";
        public void TraverseAst(){
            MakeHeader();
            principalBlock+=$"int main() {{ {l}";

            foreach(ASTNode eachNode in ast){
                TypeNode(eachNode,"");
            }
            principalBlock+=$"return 0;{l} }}{l}";

            Console.WriteLine(directives);
            // Console.WriteLine("---fin diirectives---");
            Console.WriteLine(Header);
            // Console.WriteLine("---fin header---");
            System.Console.WriteLine(principalBlock);
            System.Console.WriteLine();
            System.Console.WriteLine(functions);
            try
            {
                using (StreamWriter fileCPP = new StreamWriter(@"C:/Users/hp/Documents/Proyectos/Language Programing/LadonLang/Archives/main.cpp"))
                {
                    fileCPP.WriteLine(directives);
                    fileCPP.Write(Header);
                    fileCPP.Write(principalBlock);
                    fileCPP.Write(functions);
                }
                Console.WriteLine("Archivo escrito exitosamente.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al escribir en el archivo: " + e.Message);
            }

        }
        public void TypeNode(ASTNode node, string whereBlock){
            if(node is DeclarationNode){
                DeclarationNode? declaration = node as DeclarationNode;
                MakeDeclaration(declaration,whereBlock);
            }else if(node is LoopNode){
                LoopNode? loop = node as LoopNode;
                MakeLoopSentence(loop, whereBlock);
            }else if(node is IfNode){
                IfNode? _if = node as IfNode;
                MakeIfSentence(_if,whereBlock);
            }else if(node is FunctionNode){
                FunctionNode? function = node as FunctionNode;
                MakeFunction(function, whereBlock);
            }else if(node is UsageVariableNode){
                UsageVariableNode? variable = node as UsageVariableNode;
                MakeUseVariable(variable, whereBlock);
            }else if (node is FunctionCalledNode){
                FunctionCalledNode? call = node as FunctionCalledNode;
                MakeFunctionCall(call, whereBlock);
            }else if(node is FlowNode){
                FlowNode? flow = node as FlowNode;
                MakeFlowInstruction(flow,whereBlock);
            }else if(node is InPutNode){
                InPutNode? inPut = node as InPutNode;
                MakeInPut(inPut,whereBlock);
            }else if(node is OutPutNode){
                OutPutNode? outPut = node as OutPutNode;
                MakeOutPut(outPut,whereBlock);
            }
            // else if(node is EntityNode){
            //     EntityNode? entity = node as EntityNode;
            // }
        }

        
        public void MakeHeader(){
            symbolTable.ForEach(eachField=>{
                if(eachField.Type=="Function"){
                    principalBlock+="void ";
                    principalBlock+=eachField?.Name;
                    List<string> parameterType = [];
                    eachField?.Parameters?.ForEach(parameter=>{
                        //solo agrega tpos para los parametros
                        string type = TypeOfInstruction(parameter.Split(' ')[0]);
                        parameterType.Add(type);
                    });
                    //si contiene parametro de retorno lo hace referencia
                    if(eachField?.DataType!="Void"){
                        parameterType[parameterType.Count-1] +=" &";
                    }
                    string parameters = parameterType != null ? string.Join(", ", parameterType) : "";
                    if(parameters!=""){
                        principalBlock+=$"({parameters});{l}";
                    }else{
                        principalBlock+=$"();{l}";
                    }
                }else if(eachField.Type=="LOOP" && eachField.Name!=""){ //comprobacion extra del name (tecnicamente npo se necesitaria ya que sin nombre el ast no lo agregaria)
                    if(eachField.Parameters?.Count>0){
                        principalBlock+=$"void {eachField.Name}();{l}";
                    }else{
                        principalBlock+=$"void {eachField.Name}();{l}";
                    }
                }else if(eachField.Type=="IF" && eachField.Name!=""){
                    principalBlock+=$"void {eachField.Name}();{l}";
                }
               
            });
        }
       
        public string TypeOfInstruction(string type){
            return type switch
            {
                "STR"=>"string",
                "NUM"=>"int",
                "ANY"=>"string",
                _=>"void"  
            };
        }
        public void MakeIfSentence(IfNode _if, string whereBlock){
            string condition = "";
            _if.Condition.ForEach(eachValue=>{
                    condition+=" "+eachValue.ValueToken;
            });
            if(whereBlock=="function"){
                functions+=$"if({condition}) {{ {l}";
                _if?.IfBlock?.ForEach(block=>{
                    TypeNode(block,"function");
                });
                functions+=$"}} {l}";
            }else{
                if(_if.Name?.ValueToken!=null){
                    System.Console.WriteLine("H1");
                    //make as a function
                    principalBlock+=$"{_if.Name.ValueToken}();{l}";
                    functions+=$"void {_if.Name.ValueToken}() {{ {l}";
                    
                    functions+=$"if({condition}) {{ {l}";
                    _if?.IfBlock?.ForEach(block=>{
                        TypeNode(block,"function");
                    });
                    functions+=$"}} {l}";
                }else{
                    //make a simple if sentence
                    principalBlock+=$"if({condition}) {{ {l}";
                    _if?.IfBlock?.ForEach(block=>{
                        TypeNode(block,whereBlock);
                    });
                    principalBlock+=$"}} {l}";
                }
                
            }



        }
        //ToDo: agregar un MAX al lenguaje para no usar Iter de esta forma
        public void MakeLoopSentence(LoopNode loop, string whereBlock){
            string _for="for(";
            string _while=$"while(true){{ {l} ";
            if(whereBlock!="function"){
                if(loop.Iter?.ValueToken!=null){
                    _for+=$"int Index = {loop.Index?.ValueToken}; Index<{loop.Iter.ValueToken}; Index++) {{ {l}";
                    if(loop?.Name?.ValueToken!=null){
                        System.Console.WriteLine("loop 1");
                        principalBlock+=$"{loop?.Name?.ValueToken}(); {l}";
                        functions+=$"void {loop?.Name?.ValueToken}() {{ {l}";
                        functions+=_for;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,"function");
                        });
                        functions+=$"}} {l} }} {l}";
                    }else{
                        System.Console.WriteLine("loop 2");
                        principalBlock+=_for;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,whereBlock);
                        });
                        principalBlock+=$"}} {l}";
                    }
                }else{
                    if(loop?.Name?.ValueToken!=null){
                        System.Console.WriteLine("loop 3");
                        principalBlock+=$"{loop?.Name?.ValueToken}(); {l}";
                        functions+=$"void {loop?.Name?.ValueToken}() {{ {l}";
                        functions+=_while;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,"function");
                        });
                        functions+=$"}} {l} }} {l}";
                    }else{
                        System.Console.WriteLine("loop 4");
                        principalBlock+=_while;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,whereBlock);
                        });
                        principalBlock+=$"}} {l}";
                    }
                }
            }else {
                _for+=$"int Index = {loop.Index?.ValueToken}; Index<{loop.Iter.ValueToken}; Index++) {{ {l}";

                if(loop.Iter?.ValueToken!=null){
                    functions+=_for;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,whereBlock);
                        });
                        functions+=$"}} {l}";
                }else{
                    functions+=_while;
                        loop?.Block?.ForEach(block=>{
                            TypeNode(block,whereBlock);
                        });
                    functions+=$"}} {l}";
                }

            }
        }
        // public void MakeEntity(){
                    /*
        struct MyStruct {
            int value;

            MyStruct() : value(0) {}
            void display() const {
                std::cout << "Value: " << value << std::endl;
            }
        };
        */

        // }
        public void MakeFunction(FunctionNode function, string whereBlock){
            string parametersFunction ="";
            List<Parameter> parametersWithoutReturn=function?.ParameterList.Take(function.ParameterList.Count-1).ToList();
            if(function?.ReturnValue?.ValueToken!=null){
                if(function?.ParameterList?.Count>0){
                    if(function?.ReturnValue?.ValueToken!=null){
                        parametersFunction=ParametersWithoutReturn(function.ParameterList);
                    }else{
                        parametersFunction=ParametersWithoutReturn(parametersWithoutReturn??[]);
                    }
                    symbolTable.ForEach(eachField=>{
                        if(eachField.Name==function?.ReturnValue.ValueToken && eachField.Scope=="Global"){
                            Console.WriteLine("entro en el return value con: "+function?.ParameterList?.Count);
                            parametersFunction+=$", {TypeOfInstruction(eachField.DataType)} &{function?.ReturnValue.ValueToken}"; //typeOfInstruction(eachField.DataType)


                        }
                    });
                }else {
                    parametersFunction=ParametersWithoutReturn(function?.ParameterList??[]);
                    symbolTable.ForEach(eachField=>{
                        if(eachField.Name==function?.ReturnValue.ValueToken){
                            parametersFunction+=$" {TypeOfInstruction(eachField.DataType??"")} &{function?.ReturnValue.ValueToken}"; //typeOfInstruction(eachField.DataType)

                        }
                    });
                }
            }else{
                if(function?.ParameterList?.Count>0){
                    parametersFunction=ParametersWithoutReturn(function?.ParameterList??[]);
                }
            }

            functions+=$"void {function?.Name?.ValueToken}({parametersFunction}){{ {l}";
            function?.Block?.ForEach(block=>{
                TypeNode(block,"function");
            });
            functions+=$"}} {l}";
        }
        public string ParametersWithoutReturn(List<Parameter> parameters){//FunctionNode function
            string parametersFunction ="";
            string? type="";
            for(int parameter=0; parameter<parameters.Count; parameter++){
                // System.Console.WriteLine("cada parametro: "+function?.ParameterList[parameter].ParameterName?.ValueToken);
                //se debe buscar en la tabla ya que el tipo lo marca como void por ser identifier
                symbolTable.ForEach(eachName=>{
                    if(eachName.Name==parameters[parameter].ParameterName?.ValueToken){
                        type=eachName.DataType;
                    }
                });
                //function?.ParameterList[parameter].Type?.ValueToken
                parametersFunction+=$"{TypeOfInstruction(type??"")} {parameters[parameter].ParameterName?.ValueToken}";
                if(parameter<parameters.Count-1){
                    parametersFunction+=", ";
                }
            }
            return parametersFunction;
        }
        public void MakeInPut(InPutNode inPut, string whereBlock){
            string? value = inPut?.Value?.ValueToken, url=inPut?.Url?.ValueToken;
            if(whereBlock!="function"){
                if(url=="" && value==""){
                    principalBlock+="_Read();";
                }else if(url!=null){
                    principalBlock+=$"_Read({value}, {url});{l}";
                }else if(value!=null){
                    principalBlock+=$"cout<<{value};{l}cin.ignore();{l}cin.get();{l}";
                }
            }else{
                System.Console.WriteLine("url: "+inPut?.Url?.ValueToken + "val: "+inPut?.Value?.ValueToken);
                if(inPut?.Url?.ValueToken=="" && inPut?.Value?.ValueToken==null){
                    
                    functions+="_Read();";
                }else
                 if(url!=null){
                    functions+=$"_Read({value}, {url});{l}";
                }else if(value!=null){
                    functions+=$"cout<<{value};{l}cin.ignore();{l}cin.get();{l}";
                }
            }
        }
        public void MakeOutPut(OutPutNode outPut, string whereBlock){
            string? value = outPut?.Value?.ValueToken, url=outPut?.Url?.ValueToken;
            if(whereBlock!="function"){
                if(url!=null){
                    principalBlock+=$"_Write({url});{l}";
                }else if(value!=null){
                    principalBlock+=$"cout<<{value};{l}";
                }
            }else{
                System.Console.WriteLine("url: "+outPut?.Url?.ValueToken + "val: "+outPut?.Value?.ValueToken);
                if(url!=null){
                    functions+=$"_Write({url});{l}";
                }else if(value!=null){
                    functions+=$"cout<<{value};{l}";
                }
            }
        }
        public void MakeFlowInstruction(FlowNode flow, string whereBlock){
            System.Console.WriteLine(flow?.Identifier?.ValueToken+"hyhuh");
            if(flow?.Identifier?.ValueToken==null){
                System.Console.WriteLine("es solo un return");
                if(whereBlock=="function"){
                    functions+=$"return;{l}";
                }else{
                    principalBlock+=$"return;{l}";
                }
            }else{
                System.Console.WriteLine("es un return con una llamada");
                if(whereBlock=="function"){
                    functions+=$"{flow?.Identifier?.ValueToken}();{l}return;{l}";
                }else{
                    principalBlock+=$"{l}{flow?.Identifier?.ValueToken}();{l}return;{l}";
                }

            }

        }
        public void MakeDeclaration(DeclarationNode declaration, string whereBlock){
            //  declaration
             //global variables
            if(IsGlobalvariable(declaration?.Identifier?.Name?.ValueToken??"")){
                Identifier? aValue;
                Symbol? aSymbol;
                Header+=$"{TypeOfInstruction(declaration?.Type?.TypeToken??"")} {declaration?.Identifier?.Name?.ValueToken};{l}";
                if(declaration?.Value?.Count>0){
                    principalBlock+=$"{declaration?.Identifier?.Name?.ValueToken} =";//Header
                
                    declaration?.Value?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            aValue = eachValue as Identifier;
                            principalBlock+=$" {aValue?.Name?.ValueToken}";
                        }else if(eachValue is Symbol){
                            aSymbol = eachValue as Symbol;
                            principalBlock+=$" {aSymbol?.NameSymbol?.ValueToken}";
                        }else if(eachValue is InPutNode){
                            principalBlock+=$"_Read<{TypeOfInstruction(declaration?.Type?.TypeToken??"")}>()";
                        }
                    });
                    principalBlock+=$";{l}";
                }                    
            }//local declarations
            else{
                //handler in the instruction block
                Identifier? aValue;
                Symbol? aSymbol;
                InPutNode? inPut;
                if(whereBlock!="function"){
                    principalBlock+=$"{TypeOfInstruction(declaration?.Type?.TypeToken??"")} {declaration?.Identifier?.Name?.ValueToken}";
                    if(declaration?.Value?.Count>0){
                        principalBlock+=" =";
                    }
                    declaration?.Value?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            aValue = eachValue as Identifier;
                            principalBlock+=$" {aValue?.Name?.ValueToken}";
                        }else if(eachValue is Symbol){
                            aSymbol = eachValue as Symbol;
                            principalBlock+=$" {aSymbol?.NameSymbol?.ValueToken}";
                        }else if(eachValue is InPutNode){
                            inPut = eachValue as InPutNode;
                            System.Console.WriteLine("entro en el input");
                            principalBlock+=$"_Read<{TypeOfInstruction(declaration?.Type?.TypeToken??"")}>()";
                        }   
                    });
                    principalBlock+=$";{l}"; 
                }else{
                    functions+=$"{TypeOfInstruction(declaration?.Type?.TypeToken??"")} {declaration?.Identifier?.Name?.ValueToken}";
                    if(declaration?.Value?.Count>0){
                        functions+=" =";
                    }
                    declaration?.Value?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            aValue = eachValue as Identifier;
                            functions+=$" {aValue?.Name?.ValueToken}";
                        }else if(eachValue is Symbol){
                            aSymbol = eachValue as Symbol;
                            functions+=$" {aSymbol?.NameSymbol?.ValueToken}";
                        }
                        else if(eachValue is InPutNode){
                            inPut = eachValue as InPutNode;
                            Console.WriteLine("entro en el input de functon");
                            // functions+=$"_Read<{TypeOfInstruction(declaration?.Type?.TypeToken??"")}>()";
                            functions+=$"";
                        }
                    });
                    functions+=$";{l}";
                }
                   
            }
        }
        public bool IsGlobalvariable(string name){
            bool result = false;
            symbolTable.ForEach(eachField=>{
                if(eachField.Name==name && eachField.Type=="IDENTIFIER" && eachField.Scope=="Global"){
                    result = true;
                }
            });
            return result;
        }
        public void MakeUseVariable(UsageVariableNode variable, string whereBlock){
            // FunctionCalledNode functionCall =new();
            Identifier? rightValue;
            Symbol? rightSymbol;
            Identifier? leftValue;
            InPutNode? inputRight;
            if(variable?.LeftValue?.Count==1){
                if(whereBlock=="function"){
                    leftValue = variable?.LeftValue.First() as Identifier;
                    if(variable?.RightValue?.Count>0&& CountReads(variable?.RightValue)==0){
                        functions+=$"{leftValue?.Name?.ValueToken}";
                        if(variable?.RightValue?.Count>0){
                            functions+=" = ";
                        }
                    }
                    variable?.RightValue?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            rightValue = eachValue as Identifier;
                            functions+=$" {rightValue?.Name?.ValueToken}";
                        }else if(eachValue is Symbol){
                            rightSymbol = eachValue as Symbol;
                            functions+=$" {rightSymbol?.NameSymbol?.ValueToken}";
                        }else if(eachValue is InPutNode){
                            inputRight = eachValue as InPutNode;
                            functions+=$"cin>>{leftValue?.Name?.ValueToken}";
                        }
                    });
                    functions+=$";{l}";    
                }else{
                    leftValue = variable?.LeftValue.First() as Identifier;
                    
                    if(variable?.RightValue?.Count>0&& CountReads(variable?.RightValue)==0){
                        principalBlock+=$"{leftValue?.Name?.ValueToken}";
                        if(variable?.RightValue?.Count>0){
                            principalBlock+=" = ";
                        }
                    }
                    variable?.RightValue?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            rightValue = eachValue as Identifier;
                            principalBlock+=$" {rightValue?.Name?.ValueToken}";
                        }else if(eachValue is Symbol){
                            rightSymbol = eachValue as Symbol;
                            principalBlock+=$" {rightSymbol?.NameSymbol?.ValueToken}";
                        }else if(eachValue is InPutNode){
                            inputRight = eachValue as InPutNode;
                            /////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///ToDo: hacer una pila que me guarde en que contexto me encuentro
                            ///comparar el valueToken de leftValue con el name de la tabla de simbolos en una funcion para ver si esta la variable
                            /// ahora se su context para corroborar que sean la miisma variable y dicha funcon devuelve  su tipo y se reemplazara aqui//
                            // principalBlock+=$"_Read<{TypeOfInstruction(leftValue?.Name.TypeToken??"")}>()";

                            principalBlock+=$"cin>>{leftValue?.Name?.ValueToken}";
                        }
                    });
                    principalBlock+=$";{l}";  
                }
            }
        }
        public int CountReads(List<ASTNode> list){
            int countReads=0;
            list.ForEach(eachValue=>{
                if(eachValue is InPutNode){
                    countReads++;
                }
            });
            return countReads;
        }
        public void MakeFunctionCall(FunctionCalledNode call, string whereBlock){
            Identifier? value;
            Symbol? op;
            string? nameOfCall = call?.NameFunctionCall?.ValueToken;
            List<string>? values=[];
            string parametersOfCall="";
             call?.Parameters?.ForEach(eachParameter=>{
                eachParameter.ForEach(valuesOfParameter=>{
                    if(valuesOfParameter is Identifier){
                        value = valuesOfParameter as Identifier;
                        // values.Add(value?.Name?.ValueToken??"");
                        parametersOfCall+=$"{value?.Name?.ValueToken} ";
                    }else if(valuesOfParameter is Symbol){
                        op = valuesOfParameter as Symbol;
                        // values.Add(op?.NameSymbol?.ValueToken??"");
                        parametersOfCall+=$"{op?.NameSymbol?.ValueToken} ";
                    }
                });
                parametersOfCall+=", ";
            });
            // Eliminar la ultima coma y el espacio
            if (parametersOfCall.EndsWith(", "))
            {
                parametersOfCall = parametersOfCall.Substring(0, parametersOfCall.Length - 2);
            }
            Console.WriteLine(parametersOfCall);
            if(whereBlock=="function"){
                functions+=$"{nameOfCall}({parametersOfCall});{l}";
            }else{
                principalBlock+=$"{nameOfCall}({parametersOfCall});{l}";
            }
        }
    }
}