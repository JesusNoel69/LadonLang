using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        //Function ::= (fn)? Identifier "(" ParameterList ")" ReturnTypesList? "=<" BlockOfFn "/>"
        public static bool Function()
        {
            Match("FN_KEYWORD");
            if (!Match("IDENTIFIER"))
            {
                return false;
            }
            if (!Match("OPARENTHESIS"))
            {
                return false;
            }
            ParameterList();
            if (!Match("CPARENTHESIS"))
            {
                return false;
            }
            System.Console.WriteLine("a"+token);
            ReturnTypesList();
            if (!MatchDouble("EQUAL","LTHAN"))
            {

                return false;
            }
            BlockOfFn();

            if(!MatchDouble("SLASH", "MTHAN"))
            {
            System.Console.WriteLine("llega");

                return false;
            }
            return true;
        }
        
        // ParameterList ::= Parameter ("," Parameter)* | epsilon
        public static bool ParameterList()
        {
            if (PeekType(0) == "CPARENTHESIS") return true;

            if (!Parameter()) return false;
            while (Match("COMMA"))
            {
                if (!Parameter()) return false;
            }
            return true;
        }
        // Parameter ::= ("out")? Identifier ExplicitType?
        static bool Parameter()
        {
            Match("OUT_KEYWORD"); // opcional
            if (!Match("IDENTIFIER")) return false;

            // tipo explícito opcional
            if (PeekType(0) == "LTHAN")
            {
                if (!ExplicitType()) return false;
            }
            return true;
        }
        //ReturnTypesList ::= "->" Type ","? Return_types_list|epsilon
        public static bool ReturnTypesList()
        {
            int start = _index;
            string startToken = token;

            if (!MatchDouble("MINUS", "MTHAN"))
            {
                _index = start; token = startToken;
                return true;
            }

            if (!TypeValue()) return false;
            while (Match("COMMA"))
            {
                if (!TypeValue()) return false;
            }
            return true;
        }
        public static bool BlockOfFn()
        {
            while (_index < _tokenVector.Count)
            {
                if (PeekType(0) == "SLASH" && PeekType(1) == "MTHAN")
                    return true;

                if (!Statement())
                    return false;
            }
            return false;
        }

        //ExplicitType ::= "<" Type ">"
        public static bool ExplicitType()
        {
            if (!Match("LTHAN"))
            {
                return false;
            }
            if(!TypeValue()){

                return false;
            }
            if (!Match("MTHAN"))
            {
                return false;
            }
            return true;
        }
        //TypeValue ::= int|float|number|string|bool|char|text
        public static bool TypeValue()
        {
            //Identifier too because it can be a class
            return Match("INT_KEYWORD", "BOOL_KEYWORD"
                , "FLOAT_KEYWORD", "CHAR_KEYWORD", "STRING_KEYWORD", "TEXT_KEYWORD", "IDENTIFIER");
        }


        static bool MatchFatArrow()
        {
            return MatchDouble("EQUAL", "MTHAN");
        }

        static bool CanStartLambdaBody(string t)
        {
            // starts with < then < ... />
            if (t == "LTHAN") return true;

            // else can be start using expression
            return CanStartExpr(t);
        }
        // LambdaAssign ::= Identifier ExplicitType? "=" LambdaExpr
        public static bool LambdaAssign()
        {
            int start = _index;
            string startToken = token;

            if (!Match("IDENTIFIER"))
            {
                _index = start; token = startToken;
                return false;
            }

            // mul<int>
            if (PeekType(0) == "LTHAN")
            {
                if (!ExplicitType())
                {
                    _index = start; token = startToken;
                    return false;
                }
            }

            if (!MatchSingleAssign())
            {
                _index = start; token = startToken;
                return false;
            }

            if (!LambdaExpr())
            {
                _index = start; token = startToken;
                return false;
            }

            return true;
        }
        // LambdaExpr ::= Identifier "=>" LambdaBody | "(" LambdaParameterList? ")" "=>" LambdaBody
        public static bool LambdaExpr()
        {
            int start = _index;
            string startToken = token;

            // ( ... ) => ...
            if (Match("OPARENTHESIS"))
            {
                LambdaParameterList();
                if (!Expect("CPARENTHESIS")) { _index = start; token = startToken; return false; }

                if (!MatchFatArrow()) { _index = start; token = startToken; return false; }

                if (!LambdaBody()) { _index = start; token = startToken; return false; }

                return true;
            }

            // e => ...
            if (Match("IDENTIFIER"))
            {
                if (!MatchFatArrow()) { _index = start; token = startToken; return false; }

                if (!LambdaBody()) { _index = start; token = startToken; return false; }

                return true;
            }

            _index = start; token = startToken;
            return false;
        }
        // LambdaParameterList ::= LambdaParameter ("," LambdaParameter)*
        public static bool LambdaParameterList()
        {
            // vacío
            if (PeekType(0) == "CPARENTHESIS") return true;

            if (!LambdaParameter()) return false;

            while (Match("COMMA"))
            {
                if (!LambdaParameter()) return false;
            }

            return true;
        }

        // LambdaParameter ::= Identifier ExplicitType?
        public static bool LambdaParameter()
        {
            if (!Match("IDENTIFIER")) return false;

            if (PeekType(0) == "LTHAN")
            {
                if (!ExplicitType()) return false;
            }

            return true;
        }
        // LambdaBody ::= "<" BlockOfCodeLambda "/>" | Expr
        public static bool LambdaBody()
        {
            // for check maybe can be removed
            if (!CanStartLambdaBody(PeekType(0))) return false;
            //block
            if (PeekType(0) == "LTHAN")
            {
                Match("LTHAN");
                if (!BlockOfCodeLambda()) return false;
                if (!MatchDouble("SLASH", "MTHAN")) return false;
                return true;
            }
            if (!CanStartExpr(PeekType(0))) return false;

            return Expr();
        }

        public static bool BlockOfCodeLambda()
        {
            while (_index < _tokenVector.Count)
            {
                if (PeekType(0) == "SLASH" && PeekType(1) == "MTHAN")
                    return true;

                if (!Statement())
                {
                    Console.WriteLine("Error dentro de bloque de lambda.");
                    return false;
                }
            }
            Console.WriteLine("Error: EOF antes de cerrar '/>' en lambda.");
            return false;
        }
    }
}