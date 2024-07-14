
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
        public List<ASTNode>? Block { get; set; }=[];
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===ENTITY===");
            Console.WriteLine($"{Indent()}Name :{Name.TypeToken}");
            Console.WriteLine($"{Indent()}Entity Block");
            Block?.ForEach(eachBlock=>eachBlock?.Print());
            Console.WriteLine($"{Indent()}Entity Block fin");
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
            // Console.WriteLine($"{Indent()}Else block");
            // ElseBlock?.ForEach(eachElseBlock=>eachElseBlock?.Print());
            // foreach (var eachBlock in IfBlock)
            // {
            //     eachBlock.Print();
            // }
            // Console.WriteLine($"{Indent()}Else block fin");
            Console.WriteLine($"{Indent()}===Fin IF===");
            indentLevel--;
        }
    }
    
    public class LoopNode : ASTNode
    {
        public NodeToParser? Name{ get; set; } //optional
        public List<ASTNode>? Block{ get; set; }=[];//NodeToParser
        public NodeToParser? Iter{ get; set; }//
        public NodeToParser? Index{ get; set; }//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===LOOP===");
            Console.WriteLine($"{Indent()}Name: {Name?.TypeToken}\n{Indent()}Iter: {Iter?.TypeToken}\n{Indent()}Index: {Index?.TypeToken}");
            Console.WriteLine($"\n{Indent()}LOOP Block:");
            Block?.ForEach(eachBlock=>{
                eachBlock.Print();
                });
            Console.WriteLine($"{Indent()}LOOP Block fin");
            Console.WriteLine($"{Indent()}===Fin LOOP===");
            indentLevel--;
        }
    }
    /********************************Function*****************************************/
    
    public class FunctionNode : ASTNode
    {
        public NodeToParser Name { get; set; }=new(); //nombre de la funcion//
        public List<Parameter> ParameterList { get; set; } =[]; //lista de parametros//
        public List<ASTNode>? Block { get; set; } =[];//bloque de instrucciones dentro de la funcion
        public NodeToParser? ReturnValue{ get; set; } //identifier del out//
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Function===");
            Console.WriteLine($"{Indent()}Return Value: {ReturnValue?.TypeToken}");
            Console.WriteLine($"{Indent()}Name: {Name.TypeToken}\n{Indent()}Parameters:");
            foreach (var parameter in ParameterList)
            {
                parameter.Print();
            }
            Console.WriteLine($"{Indent()}FN Block");
            Block?.ForEach(eachBlock=>eachBlock?.Print());
            Console.WriteLine($"{Indent()}FN Block fin");
            Console.WriteLine($"{Indent()}===Fin Function===");
            indentLevel--;
        }
    }
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
    /******************************************************************************/
    public class OutPutNode : ASTNode
    {
        public NodeToParser? Value;
        public NodeToParser? Url;
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Output===");
            Console.WriteLine($"{Indent()}Value: {Value?.TypeToken}");
            Console.WriteLine($"{Indent()}Url: {Url?.TypeToken}");
            Console.WriteLine($"{Indent()}===Fin Output===");
            indentLevel--;
        }
    }
    public class InPutNode : ASTNode
    {
        public NodeToParser? Value;
        public NodeToParser? Url;
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Input===");
            Console.WriteLine($"{Indent()}Value: {Value?.TypeToken}");
            Console.WriteLine($"{Indent()}Url: {Url?.TypeToken}");
            Console.WriteLine($"{Indent()}===Fin Input===");
            indentLevel--;
        }
    }
    public class FlowNode : ASTNode{
        public NodeToParser? Identifier;//
        public bool Exist=true;
        public override void Print()
        {
            indentLevel++;
            Console.WriteLine($"{Indent()}===Go===");
            if(Identifier?.TypeToken!=null){//si no tiene identifier los 
                Console.WriteLine($"{Indent()}Go to: {Identifier?.ValueToken}");
            }
            Console.WriteLine($"{Indent()}===Fin Go===");
            indentLevel--;
        }
    }
   
}