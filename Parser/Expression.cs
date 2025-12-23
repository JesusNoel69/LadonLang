using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Lexer;
using LadonLang.Parser.Models;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        //check if is not necesary afterward
        // AssignStmt ::= IDENTIFIER TypeArguments? ("=" Expr)? 
        /*public static bool AssignStmt()
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
        }*/
        //Expression ::= AssignmentExpr
        public static Expr? Expr()
        {
            return AssignmentExpr();
        }
        //AssignmentExpr   ::= LogicalOrExpr ( "=" AssignmentExpr )?
        public static Expr? AssignmentExpr()
        {
            int start = _index;
            string st = token;
            if (PeekType(0) == "IDENTIFIER" && PeekType(1) == "EQUAL")
            {
                // left
                if (!Match("IDENTIFIER")) { _index = start; token = st; return null; }
                Token name = _tokenVector[_index - 1];

                if (!MatchSingleAssign()) { _index = start; token = st; return null; }

                var value = AssignmentExpr(); // right-associative: a=b=c
                if (value == null) { _index = start; token = st; return null; }

                return new AssignExpr(name, value);
            }
            return LogicalOrExpr();
        }
        //LogicalOrExpr ::= LogicalAndExpr ( "|" LogicalAndExpr )*
        public static Expr? LogicalOrExpr()
        {

            var left = LogicalAndExpr();
            if (left == null) return null;
            while(Match("OR"))
            {
                Token op = _tokenVector[_index - 1];
                var right = LogicalAndExpr();
                if (right == null) return null;
                left = new BinaryExpr(left, op, right);
            }
            return left;
        }
        //LogicalAndExpr ::= EqualityExpr ( "&" EqualityExpr )*
        public static Expr? LogicalAndExpr()
        {
           var left = EqualityExpr();
            if (left == null) return null;
            while(Match("AND"))
            {
                Token op = _tokenVector[_index - 1];
                var right = EqualityExpr();
                if (right == null) return null;
                left = new BinaryExpr(left, op, right);
            }
            return left;
        }
        //EqualityExpr     ::= ComparisonExpr ( ( "==" | "!=" ) ComparisonExpr )* ;
        public static Expr? EqualityExpr()
        {
            var left = ComparisonExpr();
            if (left == null) return null;

            while (true)
            {
                int start = _index;
                string st = token;
                // ==              !=
                if (MatchDouble("EQUAL", "EQUAL") || MatchDouble("DIFFERENT", "EQUAL"))
                {
                    //At this moment only save first operator to diferenciate
                    Token op1 = _tokenVector[_index - 2];
                    Token op2 = _tokenVector[_index - 1];
                    Token op3 = DoubleToken(op1, op2);
                    var right = ComparisonExpr();
                    if(right==null)return null;
                    left = new BinaryExpr(left, op3, right);
                }
                else
                {
                    _index = start; token = st;
                    break;
                }
            }

            return left;
        }
        //ComparisonExpr   ::= AdditiveExpr ( ( "<" | "<=" | ">" | ">=" ) AdditiveExpr )* ;
        public static Expr? ComparisonExpr()
        {
            var left = AdditiveExpr();
            if (left == null) return null;

            while (true)
            {
                int start = _index;
                string st = token;
                // <=  (< + =)
                // >=  (> + =)
                if(MatchDouble("LTHAN", "EQUAL") || MatchDouble("MTHAN", "EQUAL"))
                {
                    Token op1 = _tokenVector[_index - 2];
                    Token op2 = _tokenVector[_index - 1];
                    Token op3 = DoubleToken(op1,op2);
                    var right = AdditiveExpr();
                    if (right == null) return null;
                    left = new BinaryExpr(left, op3, right);
                }
                // <     >
                else if ( Match("LTHAN") || Match("MTHAN"))
                {
                    Token op = _tokenVector[_index - 1];

                    var right = AdditiveExpr();
                    if (right == null) return null;
                    left = new BinaryExpr(left, op, right);
                }
                else
                {
                    _index = start; token = st;
                    break;
                }
            }

            return left;
        }
        //AdditiveExpr     ::= MultiplicativeExpr ( ("+" | "-") MultiplicativeExpr )* ;
        public static Expr? AdditiveExpr()
        {
            var left = MultiplicativeExpr();
            if(left==null) return null;

            while(Match("PLUS", "MINUS"))
            {
                Token op = _tokenVector[_index-1];
                var right = MultiplicativeExpr();
                if(right==null) return null;
                left = new BinaryExpr(left, op, right);
            }
            return left;
        }
        //MultiplicativeExpr ::= UnaryExpr ( ("*" | "/" | "%") UnaryExpr )* ;
        public static Expr? MultiplicativeExpr()
        {
            var left = UnaryExpr();
            if(left==null) return null;

            while(Match("ASTERISK","SLASH", "PERCENT"))
            {
                Token op = _tokenVector[_index-1];
                var right = UnaryExpr(); 
                if(right == null) return null;
                left = new BinaryExpr(left, op,right);
            }
            return left;
        }
        /*
        UnaryExpr        ::= ( "-" | "!" | "not" ) UnaryExpr
                   | PostfixExpr ;
        */
        public static Expr? UnaryExpr()
        {
            if(Match("MINUS", "DIFFERENT"))
            {
                Token op = _tokenVector[_index-1];
                var right = UnaryExpr();
                if (right == null) return null;
                return new UnaryExpr(op, right);
            }
            return PostfixExpr();
        }
        //PostfixExpr ::= PrimaryExpr PostfixPart* ;
        public static Expr? PostfixExpr()
        {
            return PrimaryExpr();
        }
        
        /*
        PrimaryExpr ::= Literal
              | Identifier
              | LambdaExpr
              | "(" Expression ")" ;
        */
        public static Expr? PrimaryExpr(){
            //check this: NUMBER_KEYWORD", "TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER
            if (Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER", "FLOAT_NUMBER", "CHARACTER", "STRING"))
            {
                var tok = _tokenVector[_index - 1];
                return new LiteralExpr(tok);
            }
            if (Match("IDENTIFIER"))
            {
                var tok = _tokenVector[_index - 1];
                return new VariableExpr(tok);
            }

            if (Match("OPARENTHESIS"))
            {
                var inner = Expr();
                if (inner == null) return null;
                if (!Expect("CPARENTHESIS")) return null;
                return inner;
            }

            if (!_silent)
            {
                Console.WriteLine($"Error: token inesperado |{token}| en Primary {_tokenVector[_index].TokenType}");
            }
            return null;
        }
    }
}