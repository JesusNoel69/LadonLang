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
        public void TraverseAst(){
           
            MakeHeader();

            foreach(ASTNode eachNode in ast){
                TypeNode(eachNode);
            }
            Console.WriteLine(Header);
            System.Console.WriteLine("+++++++++++++");
            System.Console.WriteLine(principalBlock);

        }
        public void TypeNode(ASTNode node){
            if(node is DeclarationNode){
                DeclarationNode? declaration = node as DeclarationNode;

                MakeDeclaration(declaration);
            }else if(node is LoopNode){
                LoopNode? loop = node as LoopNode;
                loop?.Block?.ForEach(TypeNode);
            }else if(node is IfNode){
                IfNode? _if = node as IfNode;
                /*
                _if?.IfBlock?.ForEach(block => {
                    TypeNode(block);
                });*/
                _if?.IfBlock?.ForEach(TypeNode);
            }else if(node is FunctionNode){
                FunctionNode? function = node as FunctionNode;
                function?.Block?.ForEach(TypeNode);
            }else if(node is EntityNode){
                EntityNode? entity = node as EntityNode;
                entity?.Block?.ForEach(TypeNode);
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
                        Header+=$"void {eachField.Name}(int, int);{l}";
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
        public void MakeIfSentence(){}
        public void MakeLoopSentence(){}
        public void MakeFunction(){}
        public void MakeInPutInstruction(){}
        public void MakeOutPut(){}
        public void MakeFlowInstruction(){}
        public void MakeDeclaration(DeclarationNode declaration){
             //global variables
            //  declaration
        
                
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
                            Console.WriteLine( aValue?.Name?.ValueToken);
                        }else if(eachValue is Symbol){
                            aSymbol = eachValue as Symbol;
                            Header+=$" {aSymbol?.NameSymbol?.ValueToken}";
                            Console.WriteLine( aSymbol?.NameSymbol?.ValueToken);
                        }
                    });
                    Header+=$";{l}";                    
                }else{
                    //handler in the instruction block
                    Identifier? aValue;
                    Symbol? aSymbol;
                    principalBlock+=$"{TypeOfInstruction(declaration?.Type?.TypeToken??"")} {declaration?.Identifier?.Name?.ValueToken}";
                    if(declaration?.Value?.Count>0){
                        principalBlock+=" =";
                    }
                    declaration?.Value?.ForEach(eachValue=>{
                        if(eachValue is Identifier){
                            aValue = eachValue as Identifier;
                            principalBlock+=$" {aValue?.Name?.ValueToken}";
                            Console.WriteLine( aValue?.Name?.ValueToken);
                        }else if(eachValue is Symbol){
                            aSymbol = eachValue as Symbol;
                            principalBlock+=$" {aSymbol?.NameSymbol?.ValueToken}";
                            Console.WriteLine( aSymbol?.NameSymbol?.ValueToken);
                        }
                    });
                    principalBlock+=$";{l}";    
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