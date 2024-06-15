using LadonLang.Data;
namespace LadonLang
{
    public class AstConstructor
    {
        public static int _index = 0;
        public static NodeToParser token =new();
        // public static string beforeToken =null;

        public static List<Node> _tokenVector = [];
        public static List<ASTNode> root = [];
        public static ASTNode? blocks;

        public static List<ASTNode> getAst(List<Node> tokenVector){
            return root;
        }
        public static void Advance()
        {
            _index++;
            if (_index < _tokenVector.Count)
            {
                Console.WriteLine(token);
                token.TypeToken = _tokenVector[_index].tipoToken;
                token.ValueToken = _tokenVector[_index].token;
                // if(_index>=1){
                //     beforeToken = _tokenVector[_index-1].tipoToken;
                // }

            }
            else
            {
                System.Console.WriteLine(token);
            }
        }
        public static ASTNode Statements(){

            while (_index < _tokenVector.Count){

                if(token.TypeToken=="OPEN_CORCHETES"){
                    blocks=null;
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
                }else {
                    blocks=Declare();
                }
                root.Add(blocks);
                return blocks;
            }
            return null;
            

        }

        
        public static IfNode If(){
            //ToDo: ver si necesita borrarse cada vez antes de enviar
            IfNode _if = new();
            // _if=new();
            Advance(); //skip IF
            Advance();//skip :
            Advance();//skip (
            while(token.TypeToken!="CLOSE_CORCHETES" && token.TypeToken!="SHARP"){//condition
                _if.Condition.Add(token);
                Advance();
            }
            if(token.TypeToken=="SHARP"){
                Advance();
                _if.Name=token;
            }
            Advance();//skip ]
            Advance();//skip ---
            _if.IfBlock = Statements(); 
            Advance();//skip ---
            Advance();//skip [
            Advance();//skip :
            Advance();//skip ]
            Advance();//skip ---
            _if.ElseBlock = Statements();
            Advance();//skip ---
            return _if;
        }
        public static LoopNode Loop(){
            LoopNode loop = new();
            Advance();//skip Loop
            if(token.TypeToken=="DOUBLE_DOT"){
                Advance(); //skip :
                Advance(); //skip INDEX
                Advance(); //skip =
                loop.Index=token;
                Advance(); //skip ,
                Advance(); //skip ITER
                Advance(); //skip =
                loop.Iter=token;
            }
            if(token.TypeToken=="SHARP"){
                Advance();//skip #
                loop.Name=token;
            }
            Advance();//skip ---
            loop.Block=Statements();
            Advance();//skip ---
            return loop;
        }
        public static FunctionNode Function(){
            FunctionNode function = new();
            Advance();//skip FN
            function.Name=token;
            Advance();//skip (
            Parameter parameter = new();
            while(token.TypeToken!="OUT"&&token.TypeToken!="CLOSE_CORCHETES"){
                parameter.Type=token;
                Advance();
                parameter.ParameterName=token;
                Advance();
                function.ParameterList.Add(parameter);
            }
            Advance();//skip )
            Advance();//skip ---
            function.Block=Statements();
            Advance();//skip ---
            return function;
        }
        public static InOutPutNode InPut(){
            //[Read(“entrada”):Url=docUrl];
            //[Read(“entrada”)];
            InOutPutNode input=new();
            Advance();//skip READ        
            Advance();//skip (
            while(token.TypeToken!="CLOSE_PARENTHESIS"){
                input.Value.Add(token);
                Advance();
            }
            Advance();//skip )
            if(token.TypeToken=="DOUBLE_DOT"){
                Advance(); //skip :
                Advance(); //skip URL
                Advance(); //skip =
                input.Url=token;
                Advance();
            }            
            Advance();//skip ]    
            Advance();//skip ;    
            return input;
        }
        public static InOutPutNode OutPut(){
            //[Write(“message”)];
            //[Write : Url=docUrl];
            InOutPutNode output=new();
            Advance(); //skip WRITE
            if(token.TypeToken=="OPEN_PARENTHESIS"){
                Advance();//skip (
                output.Value.Add(token);
                Advance();
                Advance();//skip )
            }
            if(token.TypeToken=="DOUBLE_DOT"){
                Advance(); //skip :
                Advance(); //skip URL
                Advance(); //skip =
                output.Url=token;
                Advance();
            }
            Advance(); //skip ]
            Advance(); //skip ;
            return output;
        }
        public static EntityNode Entity(){
            //[Entity #name]
            // ---
            //                 Str identifier;
            // ---
            EntityNode entity = new();
            Advance(); //skip ENTITY
            Advance(); //skip #
            entity.Name=token;
            Advance();
            Advance();//skip ]
            Advance(); // skip ---
            entity.Block = Statements();
            Advance(); // skip ---
            return entity;
        }
        public static FlowNode Flow(){
            //Go;
            //Go # name;
            Advance();//skip GO
            FlowNode flow = new();
            if(token.TypeToken=="SHARP"){
                Advance();//skip #
                flow.Identifier = token;
                Advance();
            }
            Advance();//skip ;
            return flow;
        }

        ////
       public static bool ParenthesisCompare(ExpressionNode expression)
        {
            if (token.TypeToken == "OPEN_PARENTHESIS")
            {
                expression.OParenthesis = token;
                Advance(); // skip '('
                expression.Left = Expression();
                if (token.TypeToken == "CLOSE_PARENTHESIS")
                {
                    expression.CParenthesis = token;
                    Advance(); // skip ')'
                }
                return true;
            }
            return false;
        }
        public static ExpressionNode Expression()
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
                    ExpressionNode leftNode = new ExpressionNode { Operator = token };
                    Advance();
                    if (token != null && !Array.Exists(delimiters, element => element == token.TypeToken))
                    {
                        // Operator
                        NodeToParser operatorToken = token;
                        Advance();
                        // Right operand
                        ExpressionNode rightNode = new ExpressionNode { Operator = token};
                        Advance();
                        // Create a subexpression node
                        ExpressionNode subExpression = new ExpressionNode
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
        public static DeclarationNode Declare(){
            DeclarationNode declaration = new DeclarationNode(); 
            string[] asignSymbols = [ "DOUBLE_ASTERISK", "DOUBLE_MINUS", "DOUBLE_PLUS", "EQUAL_PLUS",
                                    "EQUAL_SLASH", "EQUAL_ASTERISK"];
            if(token.TypeToken=="LONG"||token.TypeToken=="ANY"||token.TypeToken=="DEFAULT"||token.TypeToken=="NUM"
            ||token.TypeToken=="STR")
            {
                declaration.Type=token;
                Advance();
            }
            while(token.TypeToken=="IDENTIFIER"){
                AssigmentNode assigment = new();
                Identifier identifier = new(){
                    Name = token
                };
                Advance();
                //NUM n.index.iter
                //NUM n.n.iter=0,m,r
                AddPropertieToIdentifier(ref identifier);
                if(Array.Exists(asignSymbols, symbol => symbol == token.TypeToken)){
                    declaration.Identifier = identifier;
                    assigment = new();
                    identifier = new(){
                        Name = token
                    };
                    assigment.Symbol=token;
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
        public static void AddPropertieToIdentifier(ref Identifier identifier){
            while(token.TypeToken=="DOT"){
                Advance();
                identifier.Properties.Add(token);
                Advance();
            }
        }
        public static void AssigmentOfFunctionCall(ref AssigmentNode assigment){
            if(token.TypeToken=="OPEN_PARENTHESIS"){
                Advance();
                if(token.TypeToken!="OPEN_PARENTHESIS"){
                    assigment.Parameters.Add(new (){
                        Name=token
                    });
                    Advance();
                    while(token.TypeToken=="COMMA"){
                        assigment.Parameters.Add(new(){Name = token});
                        Advance();
                    }
                }
                Advance(); //skip )
            }
        }
    }
}





/*
Num n.Index=2, r.n, n=r;
Str d=n(2,3,n);
n=2;
n;
n();
r=d();
*/