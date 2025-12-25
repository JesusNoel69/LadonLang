using LadonLang.Data;
using LadonLang.Parser.Models;
namespace LadonLang.Parser
{
    public partial class Parser
    {
        // Input ::= "<" input InputOptions? "/>"
        public static StmtNode? Input()
        {
            int start = _index;
            string st = token;

            if (!Match("LTHAN")) return null;
            if (!Match("INPUT_KEYWORD")) { _index = start; token = st; return null; }
            InputStmt? input = null;
            input = InputOptions(); // optional
            if (!Match("SLASH")) 
                throw new UnexpectedTokenException("SLASH '/' (needed to close <input/>)", CurrentToken());
            if (!Match("MTHAN")) 
                throw new UnexpectedTokenException("MTHAN '>' (needed to close <input/>)", CurrentToken());
            return input;
        }
        // InputOptions ::= SetInput | KeyTypeCombo
        // SetInput ::= "SET_KEYWORD" "[" IDENTIFIER "]"
        // KeyTypeCombo ::= (KeyAttr TypeAttr?) | (TypeAttr KeyAttr?)
        public static InputStmt? InputOptions()
        {
            // <input/>  → there is no options
            if (_index >= _tokenVector.Count)
                return new InputStmt();

            var t = _tokenVector[_index].TokenType;

            // <input set[ident]/>
            if (t=="SET_KEYWORD")
            {
                var set = SetInputAttr();
                if (set == null)
                    throw new UnexpectedTokenException("set[IDENTIFIER]", CurrentToken());
                return set;
            }

            // <input type=[key] key='A' ...> o <input type=[key] ...>
            bool hasKey = false;
            //bool hasType = false;
            Token? keyInput = null;
            bool typeInput=false;

            // try key first
            if (t=="KEY_KEYWORD")
            {
                keyInput=KeyInputAttr();
                if (keyInput==null)
                    throw new UnexpectedTokenException("key='CHARACTER'", CurrentToken());
                hasKey = true;
                typeInput=TypeInputAttr();
                if (!typeInput)
                {
                    throw new UnexpectedTokenException("type=[key]'", CurrentToken());
                }
                t = _tokenVector[_index].TokenType;
            }

            // try type if get
            if (t=="TYPE_KEYWORD")
            {
                typeInput=TypeInputAttr();
                if (!typeInput)
                {
                    throw new UnexpectedTokenException("type=[key]'", CurrentToken());
                }
                keyInput=KeyInputAttr();
                //hasType = true;
            }
            else if (!hasKey)
            {
                // if has no options
                return new InputStmt();//verify if there is not null needed 
            }
            var input = new InputStmt()
            {
              Key=keyInput,
              SetVariable=null,
              UseType=typeInput  
            };

            return input;
        }
       //KeyInputAttr ::= key='A'
        public static Token? KeyInputAttr()
        {
            if (!Match("KEY_KEYWORD")) return null;
            if (!Match("EQUAL")) throw new UnexpectedTokenException("EQUAL '=' after key", CurrentToken());
            if (!Match("CHARACTER")) throw new UnexpectedTokenException("CHARACTER after 'key='", CurrentToken());
            var token = _tokenVector[_index-1];
            return token;
        }
        //TypeInputAttr ::= type=[key]
        public static bool TypeInputAttr()
        {
            if (!Match("TYPE_KEYWORD")) return false;
            if (!Match("EQUAL")) throw new UnexpectedTokenException("EQUAL '=' after type", CurrentToken());
            if (!Match("OCORCHETES")) throw new UnexpectedTokenException("OCORCHETES '[' after 'type='", CurrentToken());
            if (!Match("KEY_KEYWORD")) throw new UnexpectedTokenException("KEY_KEYWORD into type=[...]", CurrentToken());
            if (!Match("CCORCHETES")) throw new UnexpectedTokenException("CCORCHETES ']' to close type=[key]", CurrentToken());
            return true;
        }

        // set[nombre_variable]
        public static InputStmt? SetInputAttr()
        {
            if (!Match("SET_KEYWORD")) return null;

            if (!Match("OCORCHETES")) throw new UnexpectedTokenException("OCORCHETES '[' after set", CurrentToken());
            if (!Match("IDENTIFIER")) throw new UnexpectedTokenException("IDENTIFIER into set[...]", CurrentToken());
            var variable = _tokenVector[_index - 1];
            if (!Match("CCORCHETES")) throw new UnexpectedTokenException("CCORCHETES ']' to close set[ident]", CurrentToken());
            var inputStmt = new InputStmt()
            {
                Key=null,
                SetVariable=variable,
                UseType=false
            };
            return inputStmt;
        }
        // Output ::= "<" output ">" PrintList "</" output ">" | OutputGet
        public static OutputStmt? Output()
        {
            int start = _index;
            string st = token;
            if (!Match("LTHAN")) return null;
            if (!Match("OUTPUT_KEYWORD")) {_index = start; token = st; return null;}
            //<output> ... </output>
            if (Match("MTHAN"))
            {
                var items = PrintList(); //should be throw an error if it fail
                if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN '<' to close </output>", CurrentToken());
                if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH '/' to close </output>", CurrentToken());
                if (!Match("OUTPUT_KEYWORD")) throw new UnexpectedTokenException("OUTPUT_KEYWORD in </output>", CurrentToken());
                if (!Match("MTHAN")) throw new UnexpectedTokenException("MTHAN '>' at the end of </output>", CurrentToken());
                var stmt = new OutputStmt();
                stmt.PrintList=items;
                return stmt;
            }

            // <output get[ident]/>  (or get[ident]/ if is added)
            Token? getVar = OutputInlineAttrs();
            if (getVar == null) { 
                _index = start; 
                token = st; 
                throw new UnexpectedTokenException("get[IDENTIFIER] or '>'", CurrentToken()); 
            }
            if (!Match("SLASH")) { 
                _index = start; 
                token = st; 
                throw new UnexpectedTokenException("SLASH '/' to close <output .../>", CurrentToken()); 
            }
            if (!Match("MTHAN")) { 
                _index = start; 
                token = st; 
                if (!Match("MTHAN")) throw new UnexpectedTokenException("MTHAN '>' to close <output .../>", CurrentToken()); 
            }
            return new OutputStmt { GetVariable = getVar };
        }

        // OutputInlineAttrs ::=  "GET_KEYWORD" "[" IDENTIFIER "]"
        public static Token? OutputInlineAttrs()
        {
            if (!Match("GET_KEYWORD")) return null;

            if (!Match("OCORCHETES")) throw new UnexpectedTokenException("OCORCHETES '[' after get", CurrentToken());
            if (!Match("IDENTIFIER")) throw new UnexpectedTokenException("IDENTIFIER into get[...]", CurrentToken());
            var id = _tokenVector[_index - 1];
            if (!Match("CCORCHETES")) throw new UnexpectedTokenException("CCORCHETES ']' to close get[ident]", CurrentToken());
            return id;
        }
        //PrintList ::= Expr ("," Expr)*
        public static List<Expr> PrintList()
        {
            // </output>
            if (PeekType(0) == "LTHAN" &&
                PeekType(1) == "SLASH" &&
                PeekType(2) == "OUTPUT_KEYWORD")
            {
                return new List<Expr>();
            }
            var list = new List<Expr>();
            var expr = Expr();
            // PrintList ::= Expr ("," Expr)* ;
            if (expr == null)
                throw new UnexpectedTokenException("Expr in PrintList <output>...</output>", CurrentToken());
            list.Add(expr);
            while (Match("COMMA"))
            {
                expr=Expr();
                if (expr == null)
                {
                    throw new UnexpectedTokenException("Expr after ',' in PrintList", CurrentToken());
                }
                list.Add(expr);
            }
            //verify if smicolon is neeed, because tag close using </output> but has special tokens
            //Match("SEMICOLON");
            return list;
        }
    }
}