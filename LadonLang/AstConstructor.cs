using System.Linq;
using System.Runtime.CompilerServices;
using LadonLang.Data;
// using LadonLang;
namespace LadonLang//LadonLangAST
{
    public class AstConstructor(List<Node> tokenVector)
    {
        public  int _index = 0;
        public NodeToParser token =new();     
        public  List<Node> _tokenVector = tokenVector;
        public  List<ASTNode> root = [];
        public List<SymbolTable> _table=[];
        public int global=0;
        public List<string> context=[];
        public  List<ASTNode> GetAst(){
            return root;
        }
        public  void Advance()
        {
            if (_index < _tokenVector.Count)
            {
                
                token.TypeToken = _tokenVector[_index].tipoToken;
                token.ValueToken = _tokenVector[_index].token;
                _index++;
            }
        }
        public void Start(ref List<SymbolTable> Table)
        {
            _table=Table;
               
            if (_tokenVector.Count > 0)
            {
                context.Add("GLobal");
                Statements();
            }
        }
        public  ASTNode? Statements(){
            _tokenVector.Insert(0,new(-1,"@","PRINCIPAL_CONTEXT",1));
            ASTNode? blocks=null;
            while (_index < _tokenVector.Count){

                blocks=null;
                if(token.TypeToken=="OPEN_CORCHETES"){
                    Advance();//skip [
                    if(token.TypeToken=="IF"){
                        blocks=If("");
                    }
                    else  if(token.TypeToken=="LOOP"){
                        blocks=Loop("");
                    }else  if(token.TypeToken=="READ"){
                        blocks=InPut();
                    }else  if(token.TypeToken=="WRITE"){
                        blocks=OutPut();
                    }else  if(token.TypeToken=="ENTITY"){
                        blocks=Entity("");
                    }
                }else  if(token.TypeToken=="FN"){
                    blocks=Function("");
                }else if(token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"||token.TypeToken=="STR"){
                    blocks = Declaration();
                }else if(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                    token.TypeToken=="STRING"||token.TypeToken=="IDENTIFIER"){
                    blocks=UseVariable();
                }else if(token.TypeToken=="GO"){
                    blocks = Flow();
                }else if(token.TypeToken=="@"){//caso que sirve como raiz del ast
                    blocks=RootCase();
                }
                if(blocks!=null){
                    root.Add(blocks);
                    blocks=null;
                }
                //al ejecutar una instruccion este salta el context-token o el semicolon y le permite avanzar
                Advance();
            }

            return blocks;
        }
        public  ASTNode? ReturnBlock(string name){
            ASTNode? blocks=null;
            if(token.TypeToken=="OPEN_CORCHETES"){
                Advance();//skip [
                if(token.TypeToken=="IF"){
                    blocks=If(name);
                }
                else  if(token.TypeToken=="LOOP"){
                    blocks=Loop(name);
                }else  if(token.TypeToken=="READ"){
                    blocks=InPut();
                }else  if(token.TypeToken=="WRITE"){
                    blocks=OutPut();
                }else  if(token.TypeToken=="ENTITY"){
                    blocks=Entity(name);
                }
            }else  if(token.TypeToken=="FN"){
                blocks=Function(name);
            }else if(token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"||token.TypeToken=="STR"){
                    blocks = Declaration();
            }
            else if(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                    token.TypeToken=="STRING"||token.TypeToken=="IDENTIFIER"){
                blocks=UseVariable();
            }
            else if(token.TypeToken=="GO"){
                blocks = Flow();
            }
            return blocks;
        }
        public PrincipalContextNode RootCase()
        {
            PrincipalContextNode principalContextNode=new();
            Advance();//skip @
            principalContextNode.PrincipalBlock?.Add(ReturnBlock("MAIN"));
            return principalContextNode;
        }
        public  IfNode If(string name){
            IfNode _if = new();
            string scopeIf = "Local";
            string identifierOfStructure="";
            if(global==0){
                scopeIf="Global";
            }
            global++;
            Advance(); //skip IF
            Advance();//skip :
            Advance();//skip (
            while(token.TypeToken!="CLOSE_PARENTHESIS" && token.TypeToken!="SHARP"){//condition
                _if.Condition.Add(new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    });
                Advance();
                
            }
            Advance();//skip )
            if(token.TypeToken=="SHARP"){
                Advance();//skip #
                _if.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                    identifierOfStructure=token.TypeToken;
                Advance();
            }
            Advance();//skip ]
            Advance();//skip ---
            if(name!=""){
                context.Add(name);
            }
            while(token.TypeToken!="CONTEXT_TOKEN"){
                _if.IfBlock?.Add(ReturnBlock("IF"));
                Advance();//skip ---
            }
            global--;
            _table.Add(new SymbolTable{
               Name="IF",
               Type="Structure control",
               Scope=scopeIf,
               Context=new List<string>(context),
               NameOfStructure=identifierOfStructure
            });
            if(name!=""){
                context.RemoveAt(-1);
            }
            return _if;
        }
        public  LoopNode Loop(string name){
            LoopNode loop = new();
            string scopeLoop = "Local";
            string identifierOfStructure="";
            if(global==0){
                scopeLoop="Global";
            }
            global++;
            Advance();//skip Loop
            if(token.TypeToken=="DOUBLE_DOT"){
                Advance(); //skip :
                Advance(); //skip INDEX
                Advance(); //skip =
                loop.Index=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();
                Advance(); //skip ,
                Advance(); //skip ITER
                Advance(); //skip =
                loop.Iter=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();
            }
            if(token.TypeToken=="SHARP"){
                Advance();//skip #
                loop.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                identifierOfStructure=token.ValueToken;
                Advance();
                
            }

            Advance();//skip ]
            Advance();//skip ---
            context.Add(name);
            while(token.TypeToken!="CONTEXT_TOKEN"){
                loop.Block?.Add(ReturnBlock("LOOP"));
                Advance();//skip ---
            }
            _table.Add(new SymbolTable{
               Name="LOOP",
               Type="Structure control",
               Scope=scopeLoop,
               Context=new List<string>(context),
               NameOfStructure=identifierOfStructure
            });
            global--;
            context.RemoveAt(context.Count-1);
            return loop;
        }
        public  FunctionNode Function(string name){
            string parameterToSymbolTable = "";
            List<string> ParametersToSymbolTable = [];
            string scopeFunction = "Local";
            if(global==0){
                scopeFunction="Global";
            }
            global++;
            string type ="Void";

            FunctionNode function = new();
            Advance();//skip FN
            function.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
            string nameFunctionToSymbolTable = token.ValueToken;
            Advance();
            Advance();//skip (
            Parameter parameter = new();
            while(token.TypeToken!="OUT"&&token.TypeToken!="CLOSE_PARENTHESIS"){
                parameterToSymbolTable="";
                parameter.Type=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                parameterToSymbolTable+=token.ValueToken;//concat type
                Advance();//skip type
                parameter.ParameterName=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                parameterToSymbolTable+=" "+token.ValueToken;//concat name of parameter
                Advance();//skip name
                if(token.TypeToken=="COMMA"){
                    Advance();//skip ,
                }
                //parameter
                ParametersToSymbolTable.Add(parameterToSymbolTable);
                function.ParameterList.Add(new Parameter{
                    ParameterName=parameter.ParameterName,
                    Type=parameter.Type
                });
            }
            if(token.TypeToken=="OUT"){
                Advance();//skip OUT
                function.ReturnValue=new(){
                    TypeToken=token.TypeToken,
                    ValueToken = token.ValueToken
                };

                _table.ForEach(tableField=>{
                    if(tableField.Name==token.ValueToken){
                        type= tableField.DataType ?? "Void";
                    }
                });
                Advance();//skip name
            }
            Advance();//skip )
            Advance();//skip ---
            if(name!=""){
                context.Add(name);
            }
            ContainsDefinitionFor(nameFunctionToSymbolTable);
            _table.Add(new SymbolTable{
               Name=nameFunctionToSymbolTable,
               Type="Function",
               DataType=type,
               Scope=scopeFunction,
               Parameters=ParametersToSymbolTable,
                Context = new List<string>(context)
            });
            while(token.TypeToken!="CONTEXT_TOKEN"){
                function.Block?.Add(ReturnBlock("FN"));
                Advance();//skip ---
            }
            global--;
            if(name!=""){
                context.RemoveAt(context.Count-1);
            }
            return function;
        }
        public void ContainsDefinitionFor(string name){
            _table.ForEach(tableField=>{
                if(tableField.Name==name){
                    throw new Exception("Error. ya existe una definicion para "+name);
                }
            });
        }
        public  InPutNode InPut(){
            string scopeInput = "Local";
            if(global==0){
                scopeInput="Global";
            }
            string parameterToSymbolTable ="-";
            InPutNode input=new();
            Advance();//skip READ  
            if(token.TypeToken=="OPEN_PARENTHESIS"){
                 Advance();//skip (
                input.Value=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();
                Advance();//skip )
                if(token.TypeToken=="DOUBLE_DOT"){
                    Advance(); //skip :
                    Advance(); //skip URL
                    Advance(); //skip =
                    if(input!=null)
                        input.Url=new NodeToParser
                        {
                            TypeToken = token.TypeToken,
                            ValueToken = token.ValueToken
                        };
                       parameterToSymbolTable  = token.ValueToken;
                    Advance();
                }            
                Advance();//skip ]   
            }    
            _table.Add(new SymbolTable{
               Name="INPUT",
               Type="Data",
               Scope=scopeInput,
               Parameters=[parameterToSymbolTable],
               Context=new List<string>(context)
            });
            // Advance();//skip ;    
            return input;
        }
        public  OutPutNode OutPut(){
            string scopeOutPut = "Local";
            if(global==0){
                scopeOutPut="Global";
            }
            string parameterToSymbolTable ="-";
            OutPutNode output=new();
            Advance(); //skip WRITE
            if(token.TypeToken=="OPEN_PARENTHESIS"){
                Advance();//skip (
                output.Value=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();
                Advance();//skip )
            }
            if(token.TypeToken=="DOUBLE_DOT"){
                Advance(); //skip :
                Advance(); //skip URL
                Advance(); //skip =
                if(output!=null)
                output.Url=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                    parameterToSymbolTable =token.ValueToken;
                Advance();
            }
            Advance(); //skip ]
            // Advance(); //skip ;
            _table.Add(new SymbolTable{
               Name="OUTPUT",
               Type="Data",
               Scope=scopeOutPut,
               Parameters=[parameterToSymbolTable],
               Context=new List<string>(context)
            });
            return output;
        }
        public  EntityNode Entity(string name){
            EntityNode entity = new();
            string scopeEntity = "Local";
            if(global==0){
                scopeEntity="Global";
            }
            global++;
            Advance(); //skip ENTITY
            Advance(); //skip #
            entity.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
            string identifierOfStructure=token.ValueToken;
            Advance();
            Advance();//skip ]
            Advance(); // skip ---
            if(name!=""){
                context.Add(name);
            }
            while(token.TypeToken!="CONTEXT_TOKEN"){
                entity.Block?.Add(ReturnBlock("ENTITY"));
                Advance(); // skip ---
            }
            _table.Add(new SymbolTable{
               Name="ENTITY",
               Type="Data",
               Scope=scopeEntity,
               NameOfStructure=identifierOfStructure
            });
            global--;
            if(name!=""){
                context.RemoveAt(context.Count-1);
            }
            return entity;
        }
        public  FlowNode Flow(){
            string scopeFlow = "Local";
            if(global==0){
                scopeFlow="Global";
            }
            string parameterToSymbolTable ="-";
            Advance();//skip GO
            FlowNode flow = new();
            if(token.TypeToken=="SHARP"){
                Advance();//skip #
                flow.Identifier = new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                    flow.Exist=false;
                    parameterToSymbolTable=token.ValueToken;
                Advance();//skip name
            }
            _table.Add(new SymbolTable{
               Name="FLOW",
               Type="Control structure",
               Scope=scopeFlow,
               Parameters=[parameterToSymbolTable],
               Context=new List<string>(context)
            });
            // Advance();//skip ;
            return flow;
        }
        public DeclarationNode Declaration(){
            //type identifier = value;
            string scopeFlow = "Local";
            if(global==0){
                scopeFlow="Global";
            }
            DeclarationNode declaration = new()
            {
                Type = new()
                {
                    TypeToken = token.TypeToken,
                    ValueToken = token.ValueToken
                }
            };
            Advance();//skip type
            declaration.Identifier= new Identifier{
                Name = new NodeToParser{
                    TypeToken = token.TypeToken,
                    ValueToken= token.ValueToken
                }
            };
            Advance();//skip identifier
            ContainsDefinitionFor(declaration.Identifier.Name.ValueToken);
            _table.Add(new SymbolTable{
                Name=declaration.Identifier.Name.ValueToken,
                Type="IDENTIFIER",
                DataType=declaration.Type.TypeToken,
                Scope=scopeFlow,
                Context=new List<string>(context)
            });

        
            if(token.TypeToken=="EQUAL"){
                Advance();//skip = 
                //
                while(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                    token.TypeToken=="STRING"||token.TypeToken=="IDENTIFIER"){

                    if(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                        token.TypeToken=="STRING"){
                        declaration.Value?.Add(
                            new Identifier{
                                Name = new NodeToParser{
                                    TypeToken = token.TypeToken,
                                    ValueToken= token.ValueToken
                                }
                        });
                        Advance();//skip value
                    }else if(token.TypeToken=="IDENTIFIER"){
                        Identifier identifier = new(){
                            Name = new NodeToParser{
                                TypeToken = token.TypeToken,
                                ValueToken= token.ValueToken
                            }
                        };
                        Advance(); //skip identifier
                        if(token.TypeToken=="DOT"){
                            while(token.TypeToken=="DOT"){
                                Advance();//skip .
                                identifier.Properties?.Add(new Identifier{
                                    Name = new NodeToParser{
                                        TypeToken = identifier.Name.TypeToken,
                                        ValueToken = identifier.Name.ValueToken,
                                    }
                                });
                                Advance();//skip identifier 
                                declaration.Value?.Add(identifier);
                            }
                        }
                    }
                    if(token.TypeToken=="SLASH"||token.TypeToken=="PLUS"||token.TypeToken=="MINUS"
                    ||token.TypeToken=="ASTERISK"){
                        declaration.Value?.Add(new Symbol{
                            NameSymbol=new(){
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }
                        });
                        Advance();
                    }
                }
                //
            }    
            
            return declaration;
        }
        public ASTNode UseVariable(){
            UsageVariableNode variable = new();
                while(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                    token.TypeToken=="STRING"||token.TypeToken=="IDENTIFIER"){
                    if(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                        token.TypeToken=="STRING"){
                        variable.LeftValue?.Add(
                            new Identifier{
                            Name = new NodeToParser{
                                TypeToken = token.TypeToken,
                                ValueToken= token.ValueToken
                            }
                        });
                        Advance();//skip value


                    }else if(token.TypeToken=="IDENTIFIER"){
                        Identifier identifier = new(){
                            Name = new NodeToParser{
                                TypeToken = token.TypeToken,
                                ValueToken= token.ValueToken
                            }
                        };
                        Advance(); //skip identifier
                        if(token.TypeToken=="DOT"){
                            while(token.TypeToken=="DOT"){
                                Advance();//skip .
                                identifier.Properties?.Add(new Identifier{
                                    Name = new NodeToParser{
                                        TypeToken = identifier.Name.TypeToken,
                                        ValueToken = identifier.Name.ValueToken,
                                    }
                                });
                                Advance();//skip identifier 
                            }
                        }
                        //function call case
                        else if(token.TypeToken=="OPEN_PARENTHESIS"){
                            FunctionCalledNode functionCalled=new(){
                                NameFunctionCall=new NodeToParser{
                                    TypeToken=identifier.Name.TypeToken,
                                    ValueToken=identifier.Name.ValueToken,
                                }
                            };
                            
                            Advance();//skip (
                            while(token.TypeToken!="CLOSE_PARENTHESIS"){
                                UsageVariableNode parameters =new();
                                Identifier parameter = new(){
                                    Name = new NodeToParser{
                                        TypeToken = token.TypeToken,
                                        ValueToken= token.ValueToken
                                    }
                                };
                                Symbol op =new();
                                parameters.LeftValue?.Add(new Identifier(){
                                    Name = new NodeToParser{
                                        TypeToken=parameter.Name.TypeToken,
                                        ValueToken=parameter.Name.ValueToken,
                                    }
                                });
                                Advance();//skip value
                                if(token.TypeToken=="SLASH"||token.TypeToken=="PLUS"||token.TypeToken=="MINUS"
                                    ||token.TypeToken=="ASTERISK"){
                                    while(token.TypeToken!="COMMA"&&token.TypeToken!="CLOSE_PARENTHESIS"&&token.TypeToken!="SEMICOLON"){
                                        op.NameSymbol=new(){
                                            TypeToken = token.TypeToken,
                                            ValueToken= token.ValueToken
                                        };
                                        parameters.LeftValue?.Add(new Symbol(){
                                            NameSymbol=new NodeToParser{
                                                TypeToken=op.NameSymbol.TypeToken,
                                                ValueToken=op.NameSymbol.ValueToken
                                            }
                                        });
                                        Advance();//skip operator
                                        parameter.Name=new(){
                                            TypeToken = token.TypeToken,
                                            ValueToken= token.ValueToken
                                        };
                                        System.Console.WriteLine("identificador es "+token.ValueToken);
                                        Console.ReadKey();
                                        parameters.LeftValue?.Add(new Identifier(){
                                            Name = new NodeToParser{
                                                TypeToken=parameter.Name.TypeToken,
                                                ValueToken=parameter.Name.ValueToken,
                                            }
                                        });
                                        Advance(); //skip value
                                        
                                    }
                                }
                                if(token.TypeToken=="COMMA"){
                                    Advance();//skip ,
                                }
                                functionCalled.Parameters.Add(parameters.LeftValue);
                            }
                            Advance();//skip )
                            return functionCalled;
                        }
                        ///function called case fin

                        variable.LeftValue?.Add(identifier);
                    }
                    if(token.TypeToken=="SLASH"||token.TypeToken=="PLUS"||token.TypeToken=="MINUS"
                    ||token.TypeToken=="ASTERISK"){
                        variable.LeftValue?.Add(new Symbol{
                            NameSymbol=new(){
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }
                        });
                        Advance();//
                    }
                }


            if(token.TypeToken=="EQUAL"){
                Advance();//skip = 
                while(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                    token.TypeToken=="STRING"||token.TypeToken=="IDENTIFIER"){

                    if(token.TypeToken=="NUMBER"||token.TypeToken=="DECIMAL_NUMBER"||
                        token.TypeToken=="STRING"){
                        variable.RightValue?.Add(
                            new Identifier{
                            Name = new NodeToParser{
                                TypeToken = token.TypeToken,
                                ValueToken= token.ValueToken
                            }
                        });
                        Advance();//skip value
                    }else if(token.TypeToken=="IDENTIFIER"){
                        Identifier identifier = new(){
                            Name = new NodeToParser{
                                TypeToken = token.TypeToken,
                                ValueToken= token.ValueToken
                            }
                        };
                        Advance(); //skip identifier
                    

                        if(token.TypeToken=="DOT"){
                            
                            while(token.TypeToken=="DOT"){
                                Advance();//skip .
                                identifier.Properties?.Add(new Identifier{
                                    Name = new NodeToParser{
                                        TypeToken = identifier.Name.TypeToken,
                                        ValueToken = identifier.Name.ValueToken,
                                    }
                                });
                                Advance();//skip identifier 
                                variable.RightValue?.Add(identifier);
                            }
                        }
                    }
                    if(token.TypeToken=="SLASH"||token.TypeToken=="PLUS"||token.TypeToken=="MINUS"
                    ||token.TypeToken=="ASTERISK"){
                        variable.RightValue?.Add(new Symbol{
                            NameSymbol=new(){
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }
                        });
                        Advance();
                    }
                }
            }
            return variable;
        }         
    }
}