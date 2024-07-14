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
        // public  ASTNode? blocks;

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
                // System.Console.WriteLine(token.TypeToken+"el valor del advance");
            }
        }
        public  void Retreat()
        {
            if (_index > 0)
            {
                _index--;//

                token.TypeToken = _tokenVector[_index].tipoToken;
                token.ValueToken = _tokenVector[_index].token;


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
                }else if(token.TypeToken=="IDENTIFIER"||token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"||token.TypeToken=="STR"){
                    blocks=Declare();
                }else if(token.TypeToken=="GO"){
                    blocks = Flow();
                }
                else if(token.TypeToken=="@"){
                    blocks=RootCase();
                }
                if(blocks!=null){
                    root.Add(blocks);
                    blocks=null;
                }
                //al ejecutar una instruccion este salta el context-token y le permite avanzar
                Advance();
            }
            return blocks;
        }
        // public int countGlobal=0;
        public  ASTNode? ReturnBlock(){
            ASTNode? blocks=null;

            if(token.TypeToken=="OPEN_CORCHETES"){
                Advance();//skip [
                if(token.TypeToken=="IF"){
                    blocks=If();
                }
                else  if(token.TypeToken=="LOOP"){
                    System.Console.WriteLine("--------------------- "+ token.ValueToken);
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
            }else if(token.TypeToken=="IDENTIFIER"||token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"||token.TypeToken=="STR"){
                blocks=Declare();
            }else if(token.TypeToken=="GO"){
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
            //[Entity #name]
            // ---
            //                 Str identifier;
            // ---
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

        ////
       public  bool ParenthesisCompare(ExpressionNode expression)
        {
            if (token.TypeToken == "OPEN_PARENTHESIS")
            {
                expression.OParenthesis = new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance(); // skip '('
                expression.Left = Expression();
                if (token.TypeToken == "CLOSE_PARENTHESIS")
                {
                    expression.CParenthesis = new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                    Advance(); // skip ')'
                }
                return true;
            }
            return false;
        }
        public  ExpressionNode Expression()
        {
            ExpressionNode expression = new ExpressionNode();
            string[] delimiters = [ "DOUBLE_ASTERISK", "DOUBLE_EQUAL", "DOUBLE_MINUS", "DOUBLE_PLUS", "EQUAL_PLUS",
                                    "EQUAL_MINUS", "EQUAL_SLASH", "EQUAL_ASTERISK", "MORE_THAN_EQUAL", "LESS_THAN_EQUAL",
                                    "DIFFERENT_EQUAL", "SEMICOLON", "CLOSE_CORCHETES" ];

            while (token != null && !Array.Exists(delimiters, element => element == token.TypeToken))
            {
                if (!ParenthesisCompare(expression))
                {
                    // Left operand
                    ExpressionNode leftNode = new ExpressionNode { Operator = new NodeToParser
                        {
                            TypeToken = token.TypeToken,
                            ValueToken = token.ValueToken
                        }};
                    Advance();
                    if (token != null && !Array.Exists(delimiters, element => element == token.TypeToken))
                    {
                        // Operator
                        NodeToParser operatorToken = new()
                        {
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            };
                        Advance();
                        // Right operand
                        ExpressionNode rightNode = new ExpressionNode { Operator = new NodeToParser
                            {
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }};
                        Advance();
                        // Create a subexpression node
                        ExpressionNode subExpression = new()
                        {
                            Left = leftNode,
                            Operator = operatorToken,
                            Right = rightNode
                        };
                        expression.SubExpressions.Add(subExpression);
                    }
                    else
                    {
                        // If we don't have a complete subexpression, add just the left node
                        expression.SubExpressions.Add(leftNode);
                    }
                }
            }
            return expression;
        }
        //////
        public  DeclarationNode Declare(){
            DeclarationNode declaration = new DeclarationNode(); 
            string[] asignSymbols = [ "DOUBLE_ASTERISK", "DOUBLE_MINUS", "DOUBLE_PLUS", "EQUAL_PLUS",
                                    "EQUAL_SLASH", "EQUAL_ASTERISK"];
            if(token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"
            ||token.TypeToken=="STR")
            {
                declaration.Type=new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    };
                Advance();
            }
            while(token.TypeToken=="IDENTIFIER"){
                AssigmentNode assigment = new();
                Identifier identifier = new()
                {
                    Name = new NodeToParser
                        {
                            TypeToken = token.TypeToken,
                            ValueToken = token.ValueToken
                        }
                };
                Advance();
                //NUM n.index.iter
                //NUM n.n.iter=0,m,r
                AddPropertieToIdentifier(ref identifier);
                if(Array.Exists(asignSymbols, symbol => symbol == token.TypeToken)){
                    declaration.Identifier = identifier;
                    assigment = new()
                    {
                        // identifier = new(){
                        //     Name = token
                        // };
                        Symbol = new NodeToParser
                            {
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }
                    };
                    Advance();
                    if(token.TypeToken=="IDENTIFIER"){
                        AddPropertieToIdentifier(ref identifier );
                        AssigmentOfFunctionCall(ref assigment);
                    }
                    assigment.Value=Expression();
                }
                if(token.TypeToken=="COMMA"){
                    Advance();//skip ,
                }
                declaration.Identifier = identifier;
                declaration.AssigmentValue = assigment;   
            }
            Advance(); //skip ;
            return declaration;
        }
        public  void AddPropertieToIdentifier(ref Identifier identifier){
            while(token.TypeToken=="DOT"){
                Advance();
                identifier?.Properties?.Add(new NodeToParser
                    {
                        TypeToken = token.TypeToken,
                        ValueToken = token.ValueToken
                    });
                Advance();
            }
        }
        public  void AssigmentOfFunctionCall(ref AssigmentNode assigment){
            if(token.TypeToken=="OPEN_PARENTHESIS"){
                Advance();
                if(token.TypeToken!="OPEN_PARENTHESIS"){
                    assigment.Parameters.Add(new (){
                        Name=new NodeToParser
                            {
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }
                    });
                    Advance();
                    while(token.TypeToken=="COMMA"){
                        assigment.Parameters.Add(new(){Name = new NodeToParser
                            {
                                TypeToken = token.TypeToken,
                                ValueToken = token.ValueToken
                            }});
                        Advance();
                    }
                }
                Advance(); //skip )
            }
        }
    }
}