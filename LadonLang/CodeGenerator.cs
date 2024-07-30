using System.Reflection;
using LadonLang.Data;
namespace LadonLang
{
    public class CodeGenerator(ref AstConstructor ast, ref List<SymbolTable> symbolTable)
    {
        private readonly List<ASTNode> ast = ast.root;
        private List<SymbolTable> symbolTable = symbolTable;
        public const string l ="\n";
        private string directives=$"#include <iostream>{l}using namespace std;{l}";
        public string Header ="";
        public string principalBlock="";
        public string functions="";
        public void TraverseAst(){
           
            MakeHeader();
            principalBlock+=$"int main() {{ {l}";

            foreach(ASTNode eachNode in ast){
                TypeNode(eachNode,"");
            }
            principalBlock+=$"return 0;{l} }}";
            Console.WriteLine(Header);
            System.Console.WriteLine();
            System.Console.WriteLine(principalBlock);
            System.Console.WriteLine();
            System.Console.WriteLine(functions);
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
            }else if(node is EntityNode){
                EntityNode? entity = node as EntityNode;
            }else if(node is UsageVariableNode){
                UsageVariableNode? use = node as UsageVariableNode;
               
            }else if (node is FunctionCalledNode){
                FunctionCalledNode? call = node as FunctionCalledNode;
            }
        }
        
        public void MakeHeader(){
            symbolTable.ForEach(eachField=>{
                //prototype function
                //type name (type ,...);
                if(eachField.Type=="Function"){
                    Header+="void ";
                    Header+=eachField?.Name;
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
                        Header+=$"({parameters});{l}";
                    }else{
                        Header+=$"();{l}";
                    }
                }else if(eachField.Type=="LOOP" && eachField.Name!=""){ //comprobacion extra del name (tecnicamente npo se necesitaria ya que sin nombre el ast no lo agregaria)
                    if(eachField.Parameters?.Count>0){
                        Header+=$"void {eachField.Name}();{l}";
                    }else{
                        Header+=$"void {eachField.Name}();{l}";
                    }
                }else if(eachField.Type=="IF" && eachField.Name!=""){
                    Header+=$"void {eachField.Name}();{l}";
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
                System.Console.WriteLine("H2");
                //make a simple if sentence
                principalBlock+=$"if({condition}) {{ {l}";
                _if?.IfBlock?.ForEach(block=>{
                    TypeNode(block,whereBlock);
                });
                principalBlock+=$"}} {l}";
            }
        }
        //ToDo: agregar un MAX al lenguaje para no usar Iter de esta forma
        public void MakeLoopSentence(LoopNode loop, string whereBlock){
            string _for="for(";
            string _while="while() {\n";
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

        }
        public void MakeFunction(){}
        public void MakeInPutInstruction(){}
        public void MakeOutPut(){}
        public void MakeFlowInstruction(){}
        public void MakeDeclaration(DeclarationNode declaration, string whereBlock){
            //  declaration
             //global variables
            if(IsGlobalvariable(declaration?.Identifier?.Name?.ValueToken??"")){
                Identifier? aValue;
                Symbol? aSymbol;
                Header+=$"{TypeOfInstruction(declaration?.Type?.TypeToken??"")} {declaration?.Identifier?.Name?.ValueToken}";
                if(declaration?.Value?.Count>0){
                    Header+=" =";
                }
                declaration?.Value?.ForEach(eachValue=>{
                    if(eachValue is Identifier){
                        aValue = eachValue as Identifier;
                        Header+=$" {aValue?.Name?.ValueToken}";
                    }else if(eachValue is Symbol){
                        aSymbol = eachValue as Symbol;
                        Header+=$" {aSymbol?.NameSymbol?.ValueToken}";
                    }
                });
                Header+=$";{l}";                    
            }//local declarations
            else{
                //handler in the instruction block
                Identifier? aValue;
                Symbol? aSymbol;
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
        public void MakeUseVariable(){}
        public void MakeFunctionCall(){}
    }
}