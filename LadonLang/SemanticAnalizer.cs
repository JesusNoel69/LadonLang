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
                eachNode.Print();
                Console.WriteLine();
            }
            foreach(ASTNode eachNode in ast){

                TypeNode(eachNode);
            }
            
            SymbolTable.ShowTable(symbolTable);
        }
        public bool CreatedVariable(ASTNode eachNode) {
            bool existsLeft = false, existsRight = true;
            UsageVariableNode? variable = eachNode as UsageVariableNode;

            // Verificar las variables en el lado izquierdo de la expresión
            variable?.LeftValue?.ForEach(value => {
                if (value is Identifier) {
                    Identifier? nameOfValue = value as Identifier;
                    existsLeft = DeclaratedVariableName.Contains(nameOfValue?.Name?.ValueToken ?? "");
                }
            });

            // Verificar las variables en el lado derecho de la expresión
            variable?.RightValue?.ForEach(value => {
                if (value is Identifier) {
                    Identifier? nameOfValue = value as Identifier;
                    if (nameOfValue?.Name?.TypeToken != "NUMBER" && nameOfValue?.Name?.TypeToken != "PLUS") {
                        if (!DeclaratedVariableName.Contains(nameOfValue?.Name?.ValueToken ?? "")) {
                            existsRight = false;
                        }
                    }
                }
            });

            // System.Console.WriteLine($"existsLeft: {existsLeft}, existsRight: {existsRight}");
            return existsRight && existsLeft;
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

                (List<string>, List<string>) listsTypes = ProcessUsageVariableNode(node as UsageVariableNode);
                if(listsTypes.Item1.Count>0 && listsTypes.Item2.Count>0 && listsTypes.Item2 is not []&& listsTypes.Item1 is not []){
                    if(!SameType(listsTypes)){
                        throw new Exception("Error. tipos de variables distintos");
                    }
                }
               
            }
        }
        public bool SameType((List<string>, List<string>) listsTypes){
            if(CheckEachList(listsTypes.Item1)==true && CheckEachList(listsTypes.Item2)==true){//que ambos en sus evaluacones usen los mismos tipos de datos  
                Console.WriteLine("list 1: "+listsTypes.Item1[0]+" list 2: "+listsTypes.Item2[0]);
                if(listsTypes.Item1[0]!=listsTypes.Item2[0]){
                    return false;
                }
            }else{
                return false;
            }
            return true;
        }

        public bool CheckEachList(List<string> list){
            //num
            //num+num2
            //num+num3+num4
            //num+num2+num4+num3

            for(int i=0; i<list.Count; i+=2){
                if(i%2==0&&i+2<list.Count){
                    System.Console.WriteLine("list i: "+list[i]+" list i+2: "+list[i+2]);
                    if(list[i]!=list[i+2]){
                        return false;
                    } 
                }
            } 
            return true;
        }

        public (List<string>, List<string>) ProcessUsageVariableNode(UsageVariableNode? variable) {
            List<string> listOfTypesRight = [];
            List<string> listOfTypesLeft = [];

            variable?.RightValue?.ForEach(right => {
                Identifier? identifierRight = right as Identifier;
                Symbol? symbolLeft = right as Symbol;

                // Evaluate identifiers
                if (right is Identifier) {
                    string name = identifierRight?.Name?.TypeToken ?? "";
                    List<ASTNode> properties = identifierRight?.Properties ?? [];
                    string type = "";
                    if (properties.Count > 0) {
                        Identifier? lastProperty = properties.Last() as Identifier;
                        type = SelectType(lastProperty);
                    } else {
                        type = SelectType(identifierRight);
                    }
                    listOfTypesRight.Add(type);
                } else if (right is Symbol) {
                    Symbol? op = right as Symbol;
                    listOfTypesRight.Add(op?.NameSymbol?.TypeToken ?? "");
                }
            });
            variable?.LeftValue?.ForEach(left => {
                Identifier? identifierLeft = left as Identifier;
                Symbol? symbolLeft = left as Symbol;

                // Evaluate identifiers
                if (left is Identifier) {
                    string name = identifierLeft?.Name?.TypeToken ?? "";
                    List<ASTNode> properties = identifierLeft?.Properties ?? [];
                    string type = "";
                    if (properties.Count > 0) {
                        Identifier? lastProperty = properties.Last() as Identifier;
                        type = SelectType(lastProperty);
                    } else {
                        type = SelectType(identifierLeft);
                    }
                    listOfTypesLeft.Add(type);
                } else if (left is Symbol) {
                    Symbol? op = left as Symbol;
                    listOfTypesLeft.Add(op?.NameSymbol?.TypeToken ?? "");
                }
            });

            Console.WriteLine("mis tipos left");
            listOfTypesLeft.ForEach(Console.WriteLine);
            Console.WriteLine("mis tipos right");
            listOfTypesRight.ForEach(Console.WriteLine);

            return(listOfTypesLeft, listOfTypesRight);

        }        

        public string SelectType(Identifier identifier){
            return identifier?.Name?.TypeToken switch
            {
                "NUMBER" => "NUM",
                "STRING" => "STR",
                "IDENTIFIER" => TypeInSymbolTable(identifier?.Name?.ValueToken??"No Type"),
                _=>""
            };
        }
        public string TypeInSymbolTable(string type){
            string result="";
            symbolTable.ForEach(eachValue => {
                if(eachValue.Name==type){
                    result=eachValue.DataType??"NO TYPE";
                }
            });
            return result;
        }

       
       
        public void Scope(){}
        
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