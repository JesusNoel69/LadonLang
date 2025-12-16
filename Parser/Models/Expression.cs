using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser.Models
{
    public class Expression
    {
        public abstract class Expr { }

        public class BinaryExpr : Expr
        {
            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

            public BinaryExpr(Expr left, Token op, Expr right)
            {
                Left = left;
                Operator = op;
                Right = right;
            }
        }

        public class UnaryExpr : Expr
        {
            public Token Operator { get; }
            public Expr Right { get; }

            public UnaryExpr(Token op, Expr right)
            {
                Operator = op;
                Right = right;
            }
        }

        public class LiteralExpr : Expr
        {
            public object? Value { get; }
            public LiteralExpr(object? value) => Value = value;
        }

        public class VariableExpr : Expr
        {
            public Token Name { get; }
            public VariableExpr(Token name) => Name = name;
        }

        public class AssignExpr : Expr
        {
            public Token Name { get; }
            public Expr Value { get; }

            public AssignExpr(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}