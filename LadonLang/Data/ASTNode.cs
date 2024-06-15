namespace LadonLang.Data{
    public abstract class ASTNode{
        public abstract void Print();
    }
    public class NodeToParser{
        public string TypeToken="";
        public string ValueToken="";
    }
    public class EntityNode:ASTNode{
        public NodeToParser Name { get; set; } =new();//
        public ASTNode Block { get; set; }//ASTNode
        public override void Print()
        {
            // Console.WriteLine($"Entidad-Name: {Name}\nEntidad-Bloque:{Block.ForEach(element=>element.Print())}");
            throw new NotImplementedException();
        }
        
    }
    public class IfNode : ASTNode
    {
        public List<NodeToParser> Condition { get; set; } =[];
        public NodeToParser? Name{ get; set; } //optional
        public ASTNode IfBlock{ get; set; }//NodeToParser
        public ASTNode? ElseBlock { get; set; }//ASTNode
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class LoopNode : ASTNode
    {
        public NodeToParser? Name{ get; set; } //optional
        public ASTNode Block{ get; set; }//NodeToParser
        public NodeToParser? Iter{ get; set; }//
        public NodeToParser? Index{ get; set; }//
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    /********************************Function*****************************************/
    public class Parameter:ASTNode
    {
        public NodeToParser? Type{ get; set; }// el tipo del parametro de tenerlo
        public NodeToParser? ParameterName{ get; set; } //el nombre del parametro de llevar parametros la funcion
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class FunctionNode : ASTNode
    {
        public NodeToParser Name { get; set; }=new(); //nombre de la funcion//
        public List<Parameter> ParameterList { get; set; } =[]; //lista de parametros//
        public ASTNode Block { get; set; } //bloque de instrucciones dentro de la funcion
        public NodeToParser? ReturnValue{ get; set; } //identifier del out//
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    /******************************************************************************/
    public class InOutPutNode : ASTNode
    {
        public List<NodeToParser>? Value;
        public NodeToParser? Url;//
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class FlowNode : ASTNode{
        public NodeToParser? Identifier;//
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
   
    // public class ExpressionNode : ASTNode{
    //     public List<ExpressionNode> Value { get; set; } =[]; //NodeToParser
    //     public NodeToParser? Operator { get; set; }
    //     public NodeToParser? Left { get; set; }
    //     public NodeToParser? Right { get; set; }
    //     public NodeToParser? OParenthesis { get; set; }
    //     public NodeToParser? CParenthesis { get; set; }
    // }
    public class ExpressionNode : ASTNode
    {
        public List<ExpressionNode> SubExpressions { get; set; } = [];
        public NodeToParser Operator { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public NodeToParser OParenthesis { get; set; }
        public NodeToParser CParenthesis { get; set; }
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class Identifier: ASTNode{
        public NodeToParser Name { get; set; } 
        public List<NodeToParser>? Properties {get; set;}=[];//puede tener propiedades cada variable
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class AssigmentNode:ASTNode
    {
        public Identifier Identifier { get; set; }//puede ser un array Any, identifier=["element1", "element2"]; //List<>
        //puede ser una functionCall, una expression, un identificador, o un valor, o un valor de entrada
        public NodeToParser? Symbol{get; set;}
        public ASTNode? Value{ get; set; }//ExpressionNode
        public List<Identifier> Parameters{get; set; }=[];
        public override void Print()
        {
            throw new NotImplementedException();
        }

    }
    public class DeclarationNode : ASTNode
    {
        public NodeToParser? Type { get; set; }
        public Identifier Identifier { get; set; }//puede ser uno o multiples, long n,m,b; long n; //List<Identifier>
        public AssigmentNode? AssigmentValue{ get; set; }
        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
}