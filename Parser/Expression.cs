using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Lexer;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        
        // AssignStmt ::= IDENTIFIER TypeArguments? ("=" Expr)? 
        public static bool AssignStmt()
        {
            if (!Match("IDENTIFIER")){
                return false;
            }
            TypeArguments();

            if (MatchSingleAssign())
            {
                if (!Expr())
                {
                    Console.WriteLine("Error: expresión inválida en asignación.");
                    return false;
                }
            }
            return true;
        }
        //Expression ::= AssignmentExpr
        public static bool Expr()
        {
            return AssignmentExpr();
        }
        //AssignmentExpr   ::= LogicalOrExpr ( "=" AssignmentExpr )?
        public static bool AssignmentExpr()
        {
            if(!LogicalOrExpr()) return false;
            if(MatchSingleAssign())
            {
                if(!AssignmentExpr()) return false;
            }
            return true;
        }
        //LogicalOrExpr ::= LogicalAndExpr ( "|" LogicalAndExpr )*
        public static bool LogicalOrExpr()
        {
            if(!LogicalAndExpr()) return false;
            while(Match("OR"))
            {
                if(!LogicalAndExpr()) return false;
            }
            return true;
        }
        //LogicalAndExpr ::= EqualityExpr ( "&" EqualityExpr )*
        public static bool LogicalAndExpr()
        {
            if(!EqualityExpr()) return false;
            while(Match("AND"))
            {
                if(!EqualityExpr()) return false;
            }
            return true;
        }
        //EqualityExpr     ::= ComparisonExpr ( ( "==" | "!=" ) ComparisonExpr )* ;
        public static bool EqualityExpr()
        {
            if (!ComparisonExpr()) return false;

            while (true)
            {
                // ==
                if (MatchDouble("EQUAL", "EQUAL"))
                {
                    if (!ComparisonExpr()) return false;
                }
                // !=
                else if (MatchDouble("DIFFERENT", "EQUAL"))
                {
                    if (!ComparisonExpr()) return false;
                }
                else
                {
                    break;
                }
            }

            return true;
        }
        //ComparisonExpr   ::= AdditiveExpr ( ( "<" | "<=" | ">" | ">=" ) AdditiveExpr )* ;
        public static bool ComparisonExpr()
        {
            if (!AdditiveExpr()) return false;

            while (true)
            {
                // <=  (< + =)
                if (MatchDouble("LTHAN", "EQUAL"))
                {
                    if (!AdditiveExpr()) return false;
                }
                // >=  (> + =)
                else if (MatchDouble("MTHAN", "EQUAL"))
                {
                    if (!AdditiveExpr()) return false;
                }
                // <
                else if ( Match("LTHAN"))
                {
                    if (!AdditiveExpr()) return false;
                }
                // > 
                else if ( Match("MTHAN"))
                {
                    if (!AdditiveExpr()) return false;
                }
                else
                {
                    break;
                }
            }

            return true;
        }
        //AdditiveExpr     ::= MultiplicativeExpr ( ("+" | "-") MultiplicativeExpr )* ;
        public static bool AdditiveExpr()
        {
            if(!MultiplicativeExpr()) return false;
            while(Match("PLUS", "MINUS"))
            {
                if(!MultiplicativeExpr()) return false;
            }
            return true;
        }
        //MultiplicativeExpr ::= UnaryExpr ( ("*" | "/" | "%") UnaryExpr )* ;
        public static bool MultiplicativeExpr()
        {
            if(!UnaryExpr()) return false;
            System.Console.WriteLine("viene de la multipicative");
            while(Match("ASTERISK","SLASH", "PERCENT"))
            {
                if(!UnaryExpr()) return false;
            }
            return true;
        }
        /*
        UnaryExpr        ::= ( "-" | "!" | "not" ) UnaryExpr
                   | PostfixExpr ;
        */
        public static bool UnaryExpr()
        {
            if(Match("MINUS", "DIFFERENT"))
            {
                return UnaryExpr();
            }
            return PostfixExpr();
        }
        //PostfixExpr ::= PrimaryExpr PostfixPart* ;
        public static bool PostfixExpr()
        {
            return PrimaryExpr();
        }
        
        /*
        PrimaryExpr ::= Literal
              | Identifier
              | LambdaExpr
              | "(" Expression ")" ;
        */
        public static bool PrimaryExpr(){
            //check this: NUMBER_KEYWORD", "TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER
            if (Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING"))
            {
                System.Console.WriteLine("matcheo tru");
                return true;
            }
            if (Match("IDENTIFIER"))
            {
                return true;
            }

            if (Match("OPARENTHESIS"))
            {
                if (!Expr()) return false;
                if (!Expect("CPARENTHESIS")) return false;
                return true;
            }
            if (!_silent)
            {
                Console.WriteLine($"Error: token inesperado |{token}| en Primary {_tokenVector[_index].TokenType}");
            }
            return false;
        }
    }
}