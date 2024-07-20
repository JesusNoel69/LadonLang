using LadonLang.Data;
using LadonLang;
//Todo: todos los bloques de cada contexto deberia ser una lista, para guardar instrucciones al mismo nivel
namespace LadonLangAST
{
    public class AstConstructor(List<Node> tokenVector)
    {
        public  int _index = 0;
        public NodeToParser token =new();     
        public  List<Node> _tokenVector = tokenVector;
        public  List<ASTNode> root = [];
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
        public void Start(List<Node> tokenVector)
        {
            if (_tokenVector.Count > 0)
            {
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
                        blocks=If();
                    }
                    else  if(token.TypeToken=="LOOP"){
                        blocks=Loop();
                    }else  if(token.TypeToken=="READ"){
                        blocks=InPut();
                    }else  if(token.TypeToken=="WRITE"){
                        blocks=OutPut();
                    }else  if(token.TypeToken=="ENTITY"){
                        blocks=Entity();
                    }
                }else  if(token.TypeToken=="FN"){
                    blocks=Function();
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
        public  ASTNode? ReturnBlock(){
            ASTNode? blocks=null;
            if(token.TypeToken=="OPEN_CORCHETES"){
                Advance();//skip [
                if(token.TypeToken=="IF"){
                    blocks=If();
                }
                else  if(token.TypeToken=="LOOP"){
                    blocks=Loop();
                }else  if(token.TypeToken=="READ"){
                    blocks=InPut();
                }else  if(token.TypeToken=="WRITE"){
                    blocks=OutPut();
                }else  if(token.TypeToken=="ENTITY"){
                    blocks=Entity();
                }
            }else  if(token.TypeToken=="FN"){
                blocks=Function();
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
            principalContextNode.PrincipalBlock?.Add(ReturnBlock());
            return principalContextNode;
        }
        public  IfNode If(){
            IfNode _if = new();
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
                Advance();
            }
            Advance();//skip ]
            Advance();//skip ---
            while(token.TypeToken!="CONTEXT_TOKEN"){
                _if.IfBlock?.Add(ReturnBlock());
                Advance();//skip ---
            }
            return _if;
        }
        public  LoopNode Loop(){
            LoopNode loop = new();
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
                    Advance();
            }

            Advance();//skip ]
            Advance();//skip ---
            while(token.TypeToken!="CONTEXT_TOKEN"){
                loop.Block?.Add(ReturnBlock());
                Advance();//skip ---
            }
            return loop;
        }
        public  FunctionNode Function(){
            FunctionNode function = new();
            Advance();//skip FN
            function.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
            Advance();
            Advance();//skip (
            Parameter parameter = new();
            while(token.TypeToken!="OUT"&&token.TypeToken!="CLOSE_PARENTHESIS"){
                parameter.Type=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();//skip type
                parameter.ParameterName=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();//skip name
                if(token.TypeToken=="COMMA"){
                    Advance();//skip ,
                }
                //parameter
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
                Advance();//skip name
            }
            Advance();//skip )
            Advance();//skip ---
            while(token.TypeToken!="CONTEXT_TOKEN"){
                function.Block?.Add(ReturnBlock());
                Advance();//skip ---
            }
            return function;
        }
        public  InPutNode InPut(){
            //[Read];   //[Read:identifier]
            //[Read(“entrada”):Url=docUrl];
            //[Read(“entrada”)];
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
                    Advance();
                }            
                Advance();//skip ]   
            }     
            
            // Advance();//skip ;    
            return input;
        }
        public  OutPutNode OutPut(){
            //[Write(“message”)];
            //[Write : Url=docUrl];
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
                Advance();
            }
            Advance(); //skip ]
            // Advance(); //skip ;
            return output;
        }
        public  EntityNode Entity(){
            EntityNode entity = new();
            Advance(); //skip ENTITY
            Advance(); //skip #
            entity.Name=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
            Advance();
            Advance();//skip ]
            Advance(); // skip ---
            while(token.TypeToken!="CONTEXT_TOKEN"){
                entity.Block?.Add(ReturnBlock());
                Advance(); // skip ---
            }
            return entity;
        }
        public  FlowNode Flow(){
            //Go;
            //Go # name;
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
                Advance();//skip name
            }
            // Advance();//skip ;
            return flow;
        }

        public DeclarationNode Declaration(){
            //type identifier = value;
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
        public UsageVariableNode UseVariable(){
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
                        Advance();
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