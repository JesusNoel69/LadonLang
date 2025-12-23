using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        // Assign ::= AssignTarget "=" Expr ";" ;
        public static AssignStmt? Assign()
        {
            AssignStmt assignStmt= new AssignStmt();
            var variable = AssignTarget();
            if(variable == null) return null;
            assignStmt.Variable = variable;
            if (!MatchSingleAssign())
            {
                Console.WriteLine("Error: falta '=' en asignación.");
                return null;
            }
            Token op = _tokenVector[_index-1];
            assignStmt.AssignOperator = op;

            var expresion = Expr(); 

            if (expresion == null)
            {
                Console.WriteLine("Error: falta expresión en asignación.");
                return null;
            }
            assignStmt.Assignment=expresion;
            if (!Expect("SEMICOLON"))
            {
                Console.WriteLine("Error: falta ';'.");
                return null;
            }
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
        /*
        // Assign ::= AssignTarget "=" Expr ";" ;
        public static Expr? Assign()
        {
            if (!AssignTarget()) return false;

            if (!MatchSingleAssign())
            {
                Console.WriteLine("Error: falta '=' en asignación.");
                return false;
            }

            if (!Expr())
            {
                Console.WriteLine("Error: falta expresión en asignación.");
                return false;
            }

            if (!Expect("SEMICOLON"))
            {
                Console.WriteLine("Error: falta ';'.");
                return false;
            }

            return true;
        }

        // AssignTarget ::= Identifier ;
        public static bool AssignTarget()
        {
            return Match("IDENTIFIER");
        }
        */

    }
}