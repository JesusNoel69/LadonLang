namespace LadonLang.Parser
{
    public partial class Parser 
    {
        //VarDeclStmt ::= VarDecl ";"
        public static bool VarDeclStmt()
        {
            if(!VarDecl())return false;
            if (!Expect("SEMICOLON")) 
            {
                Console.WriteLine("Error: falta ';'.");
                return false;
            }
            return true;;
        }
        //VarDecl ::= VAR_KEYWORD? VarDeclarator ("," VarDeclarator)*
        public static bool VarDecl()
        {
            Match("VAR_KEYWORD");
            if(!VarDeclarator()) return false;
            while (Match("COMMA"))
            {
                if(!VarDeclarator()) return false;
            }
            return true;
        }
      
        //VarDeclarator ::= Identifier TypeArguments? VarInitializer?
        public static bool VarDeclarator()
        {
            if(!Match("IDENTIFIER"))
            {
                return false;
            }
            TypeArguments();
            VarInitializer();
            return true;
        }
        //TypeArguments ::= "<" TypeList ">" ;
        public static bool TypeArguments()
        {
            // if it has no rments returns true because it's optional 
            if(!Match("LTHAN")) return true;
            if(!TypeList()) return false;
            if(!Match("MTHAN")) return false;
            return true;
        }
        //TypeList ::= DataType ("," DataType)* ;
        public static bool TypeList()
        {
            if (!DataType())
            {
                return false;
            }
            while (Match("COMMA"))
            {
                if (!DataType())
                {
                    return false;
                }
            }
            return true;
        }
        //VarInitializer ::= "=" ExpressionList ;
        public static bool VarInitializer()
        {
            if(!MatchSingleAssign()){
                return true;
            }
            if (!ExpressionList())
            {
                System.Console.WriteLine("problema: "+token);

                return false;
            }
            return true;
        }
        //ExpressionList ::= Expression ("," Expression)* ;
        public static bool ExpressionList()
        {
            if (!Expr()) return false;
            while (Match("COMMA"))
            {
                if (!Expr()) return false;
            }
            return true;
        }
        //DataType ::= (INT_KEYWORD|FLOAT_KEYWORD|CHAR_KEYWORD|STRING_KEYWORD|TEXT_KEYWORD) (Val (DOT Val)?)*
        public static bool DataType()
        {
            if (!Match("INT_KEYWORD","FLOAT_KEYWORD","CHAR_KEYWORD","STRING_KEYWORD","TEXT_KEYWORD"))
                return false;

            if (Match("INTEGER_NUMBER", "FLOAT_NUMBER"))
            {
                if (Match("DOT"))
                {
                    if (!Match("INTEGER_NUMBER", "FLOAT_NUMBER")) return false;
                }
            }
            return true;
        }

        //Val ::= "TRUE_KEYWORD" | "FALSE_KEYWORD" | "INTEGER_NUMBER" | "FLOAT_NUMBER" | "CHARACTER" | "STRING"
        public static bool Val()
        {
            return Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING");
        }

    }
}