using LadonLang.Data;
using LadonLang.Parser.Models;
namespace LadonLang.Parser
{
    public partial class Parser 
    {
        //VarDeclStmt ::= VarDecl ";"
        public static VarDeclStmt? VarDeclStmt()
        {
            var declaration = VarDecl();
            if(declaration==null)return null;
            Expect("SEMICOLON");
            return new()
            {
                Decl= declaration
            };
        }
        //VarDecl ::= VAR_KEYWORD? VarDeclarator ("," VarDeclarator)*
        public static VarDecl? VarDecl()
        {
            //in this version var will be required
            //Match("VAR_KEYWORD");
            if (!Match("VAR_KEYWORD"))
            {
                return null;
            }
            var varTok = _tokenVector[_index-1];
            var declaration = new VarDecl{VarKeyword=varTok};
            //maybe manage null in var4decl isn't necesary, because throw excption yet
            var firstDeclaration = VarDeclarator();
            if (firstDeclaration == null)
            {
                throw new UnexpectedTokenException("IDENTIFIER (declarator after 'var')", CurrentToken());
            }
            declaration.Declarators.Add(firstDeclaration);
            while (Match("COMMA"))
            {
                var nextDeclaration = VarDeclarator();
                if (nextDeclaration == null)
                {
                    throw new UnexpectedTokenException("IDENTIFIER (declarator expected after ',')", CurrentToken());
                }
                declaration.Declarators.Add(nextDeclaration);
            }
            return declaration;
        }
        //VarDeclarator ::= Identifier TypeArguments? VarInitializer?
        public static VarDeclarator? VarDeclarator()
        {
            if(!Match("IDENTIFIER"))
            {
                throw new UnexpectedTokenException("IDENTIFIER", CurrentToken());
            }
            var identifierToken = _tokenVector[_index-1];
            var typeArgs = TypeArguments();
            var Initializer = VarInitializer();
            return new VarDeclarator
            {
                Identifier=identifierToken,
                Initializer = Initializer,
                TypeArguments = typeArgs
            };
        }
        //TypeArguments ::= "<" TypeList ">" ;
        public static List<DataTypeNode> TypeArguments()
        {
            // if it has no arguments returns true because it's optional 
            if(!Match("LTHAN")) return new List<DataTypeNode>();
            var list = TypeList();
            //these values has errors if no manage 
            if (list == null)
                throw new InvalidTypeException(CurrentToken());
            Expect("MTHAN");
            return list;
        }
        //TypeList ::= DataType ("," DataType)* ;
        public static List<DataTypeNode>? TypeList()
        {
            var list = new List<DataTypeNode>();
            var firstType = DataType();
            if (firstType==null)
            {
                throw new InvalidTypeException(CurrentToken());
            }
            list.Add(firstType);
            while (Match("COMMA"))
            {  
                var nextType = DataType();
                if (nextType == null)
                {
                    throw new InvalidTypeException(CurrentToken());
                }
                list.Add(nextType);
            }
            return list;
        }
        //VarInitializer ::= "=" ExpressionList ;
        public static VarInitializerNode? VarInitializer()
        {
            if(!MatchSingleAssign()){ // if is'nt initilized yet
                return null;
            }
            //keep the assign op, maybe isn't necesary
            var op = _tokenVector[_index-1];
            var expressions = ExpressionList();
            if (expressions==null)
            {
                throw new UnexpectedTokenException("Expression (expression list after '=')", CurrentToken());
            }
            var init = new VarInitializerNode()
            {
                AssignOperator=op
            };
            init.Expressions.AddRange(expressions);
            return init;
        }
        //ExpressionList ::= Expression ("," Expression)* ;
        public static List<Expr>? ExpressionList()
        {
            var list = new List<Expr>();
            var expression = Expr();
            if (expression == null)
            {
                throw new UnexpectedTokenException("Expression", CurrentToken());
            }
            list.Add(expression);
            while (Match("COMMA"))
            {
                var nextExpression = Expr();
                if (nextExpression == null)
                {
                    throw new UnexpectedTokenException("Expression (after ',')", CurrentToken());
                }
                list.Add(nextExpression);
            }
            return list;
        }
        //DataType ::= (INT_KEYWORD|FLOAT_KEYWORD|CHAR_KEYWORD|STRING_KEYWORD|TEXT_KEYWORD) (Val (DOT Val)?)?
        public static DataTypeNode? DataType()
        {
            if (!Match("INT_KEYWORD","FLOAT_KEYWORD","CHAR_KEYWORD","STRING_KEYWORD","TEXT_KEYWORD"))
                return null;
            var baseType = _tokenVector[_index-1];
            Token? arg = null;
            Token? argAfterDot = null;

            if (Match("INTEGER_NUMBER", "FLOAT_NUMBER"))
            {
                arg = _tokenVector[_index-1];
                if (Match("DOT"))
                {
                    if (!Match("INTEGER_NUMBER", "FLOAT_NUMBER"))
                    {
                        throw new UnexpectedTokenException("INTEGER_NUMBER or FLOAT_NUMBER", CurrentToken());
                    }
                    argAfterDot = _tokenVector[_index-1];
                }
            }
            return new DataTypeNode
            {
                BaseTypeKeyword=baseType,
                SizeOrArg1=arg,
                SizeOrArg2=argAfterDot
            };
        }
        //Val ::= "TRUE_KEYWORD" | "FALSE_KEYWORD" | "INTEGER_NUMBER" | "FLOAT_NUMBER" | "CHARACTER" | "STRING"
        public static bool Val()
        {
            return Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING");
        }
    }
}