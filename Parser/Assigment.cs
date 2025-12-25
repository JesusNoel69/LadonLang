using LadonLang.Data;
using LadonLang.Parser.Models;
namespace LadonLang.Parser
{
    public partial class Parser
    {
        // Assign ::= AssignTarget "=" Expr ";" ;
        public static AssignStmt? Assign()
        {
            int start = _index;
            string st = token;
            AssignStmt assignStmt= new AssignStmt();
            var variable = AssignTarget();
            if(variable == null)
            {
                _index = start; token = st;
                return null;
            }
            assignStmt.Variable = variable;
            if (!MatchSingleAssign())
            {
                throw new UnexpectedTokenException("'=' (assignation)", CurrentToken());
            }
            Token op = _tokenVector[_index-1];
            assignStmt.AssignOperator = op;
            var expresion = Expr(); 
            if (expresion == null)
            {
                throw new UnexpectedTokenException("Expression after '='", CurrentToken());
            }
            assignStmt.Assignment=expresion;
            Expect("SEMICOLON");
            return assignStmt;
        }
        // AssignTarget ::= Identifier ;
        public static Token? AssignTarget()
        {

            if (Match("IDENTIFIER"))
            {
                return _tokenVector[_index-1];
            }
            return null;
        }
    }
}