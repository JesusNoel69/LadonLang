using LadonLang.Data;

namespace LadonLang
{
    public class SemanticAnalizer
    {
        private List<SymbolTable> symbolTable;
        private List<ASTNode> ast;
        public SemanticAnalizer(ref List<SymbolTable> symbolTable, ref AstConstructor ast){
            this.symbolTable = symbolTable;
            this.ast = ast.root;
        }
        public void Analize(){
            foreach(ASTNode eachNode in ast){
                eachNode.Print();
                Console.WriteLine();
            }
        }

        public bool DecratedVariable(){
            return true;
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