public abstract class ASTNode
    {
        public abstract void Print(); // Método para imprimir el nodo (para depuración)
    }
    

    public class ExpressionOperation : ASTNode
    {
        public ASTNode Left { get; set; }
        public string Operator { get; set; }
        public ASTNode Right { get; set; }

        public ExpressionOperation(ASTNode left, string op, ASTNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override void Print()
        {
            Console.Write("(");
            Left.Print();
            Console.Write(" " + Operator + " ");
            Right.Print();
            Console.Write(")");
        }
    }

    public class ValueNode : ASTNode
    {
        public string TokenType { get; set; }
        public string Value { get; set; }

        public ValueNode(string tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public override void Print()
        {
            Console.Write(Value);
        }
    }

    public class DeclarationNode : ASTNode
    {
        public ASTNode TypeNode { get; set; }
        public string Identifier { get; set; }
        public ASTNode AssignNode { get; set; }

        public DeclarationNode(ASTNode typeNode, string identifier, ASTNode assignNode)
        {
            TypeNode = typeNode;
            Identifier = identifier;
            AssignNode = assignNode;
        }

        public override void Print()
        {
            TypeNode.Print();
            Console.Write(" " + Identifier);
            if (AssignNode != null)
            {
                Console.Write(" = ");
                AssignNode.Print();
            }
            Console.Write(";");
        }
    }

    public class AssignNode : ASTNode
    {
        public string Operator { get; set; }
        public ASTNode Value { get; set; }

        public AssignNode(string op, ASTNode value)
        {
            Operator = op;
            Value = value;
        }

        public override void Print()
        {
            Console.Write(Operator + " ");
            Value.Print();
        }
    }

    public class FunctionNode : ASTNode
    {
        public string Identifier { get; set; }
        public ASTNode Parameters { get; set; }
        public string OutIdentifier { get; set; }
        public ASTNode Body { get; set; }

        public FunctionNode(string identifier, ASTNode parameters, string outIdentifier, ASTNode body)
        {
            Identifier = identifier;
            Parameters = parameters;
            OutIdentifier = outIdentifier;
            Body = body;
        }

        public override void Print()
        {
            Console.Write("fn " + Identifier + "(");
            Parameters.Print();
            Console.Write(") out " + OutIdentifier + " {");
            Body.Print();
            Console.Write("}");
        }
    }

    public class ParameterNode : ASTNode
    {
        public ASTNode TypeNode { get; set; }
        public string Identifier { get; set; }

        public ParameterNode(ASTNode typeNode, string identifier)
        {
            TypeNode = typeNode;
            Identifier = identifier;
        }

        public override void Print()
        {
            TypeNode.Print();
            Console.Write(" " + Identifier);
        }
    }

    public class ParameterListNode : ASTNode
    {
        public ASTNode Parameter { get; set; }
        public ASTNode NextParameter { get; set; }

        public ParameterListNode(ASTNode parameter, ASTNode nextParameter)
        {
            Parameter = parameter;
            NextParameter = nextParameter;
        }

        public override void Print()
        {
            Parameter.Print();
            if (NextParameter != null)
            {
                Console.Write(", ");
                NextParameter.Print();
            }
        }
    }

    public class TypeNode : ASTNode
    {
        public string Type { get; set; }

        public TypeNode(string type)
        {
            Type = type;
        }

        public override void Print()
        {
            Console.Write(Type);
        }
    }