
namespace LadonLang.Data{
    
    public abstract class ASTNode{
        protected static int indentLevel = 0;
        protected static string Indent(){
            string temporalIndent="";
            for(int i = 0;i<indentLevel;i++){
                temporalIndent+="    ";
            }
            return temporalIndent;
        }
        
        public abstract void Print();

    }
    public class PrincipalContextNode:ASTNode{
        public List<ASTNode>? PrincipalBlock {get; set;}
        public override void Print()
        {
            PrincipalBlock?.ForEach(eachBlock=>eachBlock?.Print());
        }
    }
    public class NodeToParser{
        public string TypeToken="";
        public string ValueToken="";
    }
    public class EntityNode:ASTNode{
        public NodeToParser Name { get; set; } =new();//
        public ASTNode? Block { get; set; }//ASTNode
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===ENTITY===");
            Console.WriteLine($"{Indent()}Name :{Name}");
            Block?.Print();
            Console.WriteLine($"{Indent()}===Fin ENTITY===");
            indentLevel--;
        }
        
    }
    public class IfNode : ASTNode
    {
        public List<NodeToParser> Condition { get; set; } =[];
        public NodeToParser? Name{ get; set; } //optional
        //ToDo: deberia ser list
        public List<ASTNode>? IfBlock{ get; set; }=[];//NodeToParser
        public List<ASTNode>? ElseBlock { get; set; }=[];//ASTNode
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===IF===");
            Console.WriteLine($"{Indent()}Name: {Name?.TypeToken}");
            Console.Write($"{Indent()}Condition: ");
            Condition.ForEach(eachCondition => Console.Write(eachCondition?.TypeToken+", "));
            Console.WriteLine($"\n{Indent()}If block:");
            
            IfBlock?.ForEach(eachIfBlock => {
                eachIfBlock?.Print();
            });

            Console.WriteLine($"{Indent()}If block fin");
            Console.WriteLine($"{Indent()}Else block");
            ElseBlock?.ForEach(eachElseBlock=>eachElseBlock?.Print());
            // foreach (var eachBlock in IfBlock)
            // {
            //     eachBlock.Print();
            // }
            Console.WriteLine($"{Indent()}Else block fin");
            Console.WriteLine($"{Indent()}===Fin IF===");
            indentLevel--;
        }
    }
    
    public class LoopNode : ASTNode
    {
        public NodeToParser? Name{ get; set; } //optional
        public ASTNode? Block{ get; set; }//NodeToParser
        public NodeToParser? Iter{ get; set; }//
        public NodeToParser? Index{ get; set; }//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===LOOP===");
            Console.WriteLine($"{Indent()}Name: {Name?.TypeToken}\n{Indent()}Iter: {Iter?.TypeToken}\n{Indent()}Index: {Index?.TypeToken}");
            Block?.Print();
            Console.WriteLine($"{Indent()}===Fin LOOP===");
            indentLevel--;
        }
    }
    /********************************Function*****************************************/
    public class Parameter:ASTNode
    {
        public NodeToParser? Type{ get; set; }// el tipo del parametro de tenerlo
        public NodeToParser? ParameterName{ get; set; } //el nombre del parametro de llevar parametros la funcion

        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Parameter===");
            Console.WriteLine($"{Indent()}Type: {Type?.TypeToken}\n{Indent()}Parameter Name: {ParameterName?.TypeToken}");
            Console.WriteLine($"{Indent()}===Fin Parameter===");
            indentLevel--;
        }
    }
    public class FunctionNode : ASTNode
    {
        public NodeToParser Name { get; set; }=new(); //nombre de la funcion//
        public List<Parameter> ParameterList { get; set; } =[]; //lista de parametros//
        public ASTNode? Block { get; set; } //bloque de instrucciones dentro de la funcion
        public NodeToParser? ReturnValue{ get; set; } //identifier del out//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Function===");
            Console.WriteLine($"{Indent()}Return Value: {ReturnValue}");
            Console.WriteLine($"{Indent()}Name: {Name.TypeToken}\nParameters:");
            foreach (var parameter in ParameterList)
            {
                parameter.Print();
            }
            Block?.Print();
            Console.WriteLine($"{Indent()}===Fin Function===");
            indentLevel--;
        }
    }
    /******************************************************************************/
    public class InOutPutNode : ASTNode
    {
        public List<NodeToParser>? Value;
        public NodeToParser? Url;//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Input or Output===");
            Value?.ForEach(value => Console.WriteLine($"{Indent()}{value.TypeToken}"));
            Console.WriteLine($"{Indent()}Url: {Url?.TypeToken}");
            Console.WriteLine($"{Indent()}===Fin Input or Output===");
            indentLevel--;
        }
    }
    public class FlowNode : ASTNode{
        public NodeToParser? Identifier;//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Go===");
            Console.WriteLine($"{Indent()}Go to: {Identifier?.ValueToken}");
            Console.WriteLine($"{Indent()}===Fin Go===");
            indentLevel--;
        }
    }
    public class ExpressionNode : ASTNode
    {
        public List<ExpressionNode> SubExpressions { get; set; } = [];
        public NodeToParser? Operator { get; set; }
        public ExpressionNode? Left { get; set; }
        public ExpressionNode? Right { get; set; }
        public NodeToParser? OParenthesis { get; set; }
        public NodeToParser? CParenthesis { get; set; }
       public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Expression===");
            if (OParenthesis != null)
                Console.Write(OParenthesis.ValueToken);
            Left?.Print();
            if (Operator != null)
                Console.Write(Operator.ValueToken);
            Right?.Print();
            if (CParenthesis != null)
                Console.Write(CParenthesis.ValueToken);
            
            foreach (var subExpr in SubExpressions)
            {
                subExpr.Print();
            }
            Console.WriteLine($"{Indent()}===Fin Expression===");
            indentLevel--;
        }
    }
    public class Identifier: ASTNode{
        public NodeToParser? Name { get; set; } 
        public List<NodeToParser>? Properties {get; set;}=[];//puede tener propiedades cada variable
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Identifier===");
            Console.WriteLine($"{Indent()}Name: {Name?.ValueToken}");
            Properties?.ForEach(property=>Console.WriteLine($"{Indent()}{property.ValueToken}"));
            Console.WriteLine($"{Indent()}===Fin Identifier===");
            indentLevel--;
        }
    }
    public class AssigmentNode:ASTNode
    {
        public Identifier? Identifier { get; set; }//puede ser un array Any, identifier=["element1", "element2"]; //List<>
        //puede ser una functionCall, una expression, un identificador, o un valor, o un valor de entrada
        public NodeToParser? Symbol{get; set;}
        public ExpressionNode? Value{ get; set; }//NodeToParser
        public List<Identifier> Parameters{get; set; }=[];
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Asigment===");
            Identifier?.Print();
            Console.WriteLine($"{Indent()}Symbol: {Symbol?.ValueToken}");
            Value?.Print();
            foreach (var Parameter in Parameters)
            {
                Parameter.Print();
            }
            Console.WriteLine($"{Indent()}===Fin Asigment===");
            indentLevel--;
        }
    }
    public class DeclarationNode : ASTNode
    {
        public NodeToParser? Type { get; set; }
        public Identifier? Identifier { get; set; }//puede ser uno o multiples, long n,m,b; long n; //List<Identifier>
        public AssigmentNode? AssigmentValue{ get; set; }
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Declaration===");
            Console.WriteLine($"{Indent()}Type: {Type?.ValueToken}");
            Identifier?.Print();
            AssigmentValue?.Print();
            Console.WriteLine($"{Indent()}===Fin Declaration===");
            indentLevel--;
        }
    }
}