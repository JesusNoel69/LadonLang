using LadonLang.Data;
namespace LadonLang
{
    public class CodeGenerator(ref AstConstructor ast, ref List<SymbolTable> symbolTable)
    {
        private readonly List<ASTNode> ast = ast.root;
        private List<SymbolTable> symbolTable = symbolTable; 
        public void TraverseAst(){
            foreach(ASTNode eachNode in ast){
                TypeNode(eachNode);
            }
        }
        public void TypeNode(ASTNode node){
            if(node is DeclarationNode){
                DeclarationNode? declaration = node as DeclarationNode;
            }else if(node is LoopNode){
                LoopNode? loop = node as LoopNode;
                loop?.Block?.ForEach(block => {
                    TypeNode(block);
                });
            }else if(node is IfNode){
                IfNode? _if = node as IfNode;
                _if?.IfBlock?.ForEach(block => {
                    TypeNode(block);
                });
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
                UsageVariableNode? use = node as UsageVariableNode;
               
            }else if (node is FunctionCalledNode){
                FunctionCalledNode? call = node as FunctionCalledNode;
            }
        }

        public void MakeIfSentence(){}
        public void MakeLoopSentence(){}
        public void MakeFunction(){}
        public void MakeInPutInstruction(){}
        public void MakeOutPut(){}
        public void MakeFlowInstruction(){}
        public void MakeDeclaration(){}
        public void MakeUseVariable(){}
        public void MakeFunctionCall(){}
    }
}