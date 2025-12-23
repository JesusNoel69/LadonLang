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
        //Function ::= fn Identifier "(" ParameterList ")" ReturnTypesList? "=<" BlockOfFn "/>"
        public static FunctionStmt? Function()
        {
            if (!Match("FN_KEYWORD"))
            {
                return null;
            }
            if (!Match("IDENTIFIER"))
            {
                return null;
            }
            var functionName = _tokenVector[_index-1];
            if (!Match("OPARENTHESIS"))
            {
                return null;
            }
            var parameters = ParameterList();
            

            if (!Match("CPARENTHESIS"))
            {
                return null;
            }
            var returnTpes = ReturnTypesList();
            if (!MatchDouble("EQUAL","LTHAN"))
            {
                return null;
            }
            var block = BlockOfFn();

            if(!MatchDouble("SLASH", "MTHAN"))
            {
                return null;
            }
            var outParameter = parameters?.FirstOrDefault(x=> x.IsOutParameter==true);
            var parameterWithoutOut = parameters?.Where(x=> x.IsOutParameter==false).ToList();
            return new FunctionStmt
            {
                Block=block,
                Name=functionName,
                OutParameter=outParameter,
                Parameters=parameterWithoutOut,
                ReturnTypes=returnTpes
            };
        }
        
        // ParameterList ::= Parameter ("," Parameter)* | epsilon
        public static List<Parameter>? ParameterList()
        {
            var parameters = new List<Parameter>();
            if (PeekType(0) == "CPARENTHESIS") return [];//maybe this should be null
            var firstParameter = Parameter();
            if (firstParameter==null) return null;
            parameters.Add(firstParameter);
            while (Match("COMMA"))
            {
                var nextParameter = Parameter();
                if (nextParameter==null) return null;
                parameters.Add(nextParameter);
            }
            return parameters;
        }
        // Parameter ::= ("out")? Identifier ExplicitType?
        static Parameter? Parameter()
        {
            bool isOut = Match("OUT_KEYWORD");
            if (!Match("IDENTIFIER")) return null;
            var parameterName = _tokenVector[_index-1];
            Token? type = null;
            // optional explicit type
            if (PeekType(0) == "LTHAN")
            {
                type = ExplicitType();
                if (type==null) return null;
            }
            return new()
            {
                IsOutParameter=isOut,
                Type=type,
                Name=parameterName
            };
        }
        //ReturnTypesList ::= "->" Type ","? Return_types_list|epsilon
        public static List<Token>? ReturnTypesList()
        {
            int start = _index;
            string startToken = token;

            if (!MatchDouble("MINUS", "MTHAN"))
            {
                _index = start; token = startToken;
                return new List<Token>();
            }
            var types = new List<Token>();
            var firstType = TypeValue();
            if (firstType==null) return null;
            types.Add(firstType);
            while (Match("COMMA"))
            {
                var nextType = TypeValue();
                if (nextType==null) return null;
                types.Add(nextType);
            }
            return types;
        }
        public static BlockStmt? BlockOfFn()
        {
            var block = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                if (PeekType(0) == "SLASH" && PeekType(1) == "MTHAN")
                    return block;
                var stmt = Statement();
                if (stmt == null)
                    return null;
                block.Statements.Add(stmt);
            }
            return null;
        }

        //ExplicitType ::= "<" Type ">"
        public static Token? ExplicitType()
        {
            if (!Match("LTHAN"))
            {
                return null;
            }
            var type = TypeValue();
            if(type==null){

                return null;
            }
            if (!Match("MTHAN"))
            {
                return null;
            }
            return type;
        }
        //TypeValue ::= int|float|number|string|bool|char|text
        public static Token? TypeValue()
        {
            //Identifier too because it can be a class
            if(Match("INT_KEYWORD", "BOOL_KEYWORD"
                , "FLOAT_KEYWORD", "CHAR_KEYWORD", "STRING_KEYWORD", "TEXT_KEYWORD", "IDENTIFIER"))
            {
                return _tokenVector[_index-1];
            }
            return null;
        }
        //lambda logic commented at this version
        /*
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
        */
    }
}