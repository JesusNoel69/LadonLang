using System.Dynamic;
using LadonLang.Data;

namespace LadonLang
{
    public class SemanticAnalizer
    {
        private List<SymbolTable> symbolTable;
        private List<ASTNode> ast;
        public List<string> DeclaratedVariableName=[];
        public SemanticAnalizer(ref List<SymbolTable> symbolTable, ref AstConstructor ast){
            this.symbolTable = symbolTable;
            this.ast = ast.root;
        }
        public void Analize(){
            foreach(ASTNode eachNode in ast){
                // eachNode.Print();
                // Console.WriteLine();
                TypeNode(eachNode);
            }
            SymbolTable.ShowTable(symbolTable);
        }
        public bool CreatedVariable(ASTNode eachNode){
            bool exists = false;
            UsageVariableNode? variable = eachNode as UsageVariableNode;
            variable?.LeftValue?.ForEach(value=>{
                if(value is Identifier){
                    Identifier? nameOfValue = value as Identifier;
                    DeclaratedVariableName.ForEach(eachVariable=>{
                        if(nameOfValue?.Name?.ValueToken==eachVariable){
                            exists = true;
                        }
                    });
                }
            });
            variable?.LeftValue?.ForEach(value=>{
                if(value is Identifier){
                    Identifier? nameOfValue = value as Identifier;
                    DeclaratedVariableName.ForEach(eachVariable=>{
                        if(nameOfValue?.Name?.ValueToken==eachVariable){
                            exists = true;
                        }
                    });
                }
            });
            return exists;
        }
        public void TypeNode(ASTNode node){
            if(node is DeclarationNode){
                DeclarationNode? declaration = node as DeclarationNode;
                DeclaratedVariableName.Add(declaration?.Identifier?.Name?.ValueToken??"");//
            }else if(node is LoopNode){
                LoopNode? loop = node as LoopNode;
                if(loop?.Name?.ValueToken!=""){
                    DeclaratedVariableName.Add(loop?.Name?.ValueToken??"");//
                }
                loop?.Block?.ForEach(block => {
                    TypeNode(block);
                });
            }else if(node is IfNode){
                IfNode? _if = node as IfNode;
                _if?.IfBlock?.ForEach(block => {
                    TypeNode(block);
                });
                if(_if?.Name?.ValueToken!=""){
                    DeclaratedVariableName.Add(_if?.Name?.ValueToken??"");//
                }
            }else if(node is FunctionNode){
                FunctionNode? function = node as FunctionNode;
                function?.Block?.ForEach(block => {
                    TypeNode(block);
                });
            }else if(node is EntityNode){
                EntityNode? entity = node as EntityNode;
                entity?.Block?.ForEach(block => {
                    TypeNode(block);
                });
            }else if(node is UsageVariableNode){
                if(!CreatedVariable(node)){
                    throw new Exception("error. Variable no declarada previamente");
                }
            }
        }

       
        public void Scope(){}
        public bool SameType(){
            return true;
        }
        public bool FunctionParameterVerification(){
            return true;
        }
        public bool FunctionReturnType(){
            return true;
        }
        public bool OperandsOfCompatibleTypes(){
            return true;
        }
        public bool VerifyCondition(){
            return true;
        }


    }
}