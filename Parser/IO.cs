using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        // Input ::= "<" input InputOptions? "/>"
        public static bool Input()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("INPUT_KEYWORD")) return false;

            InputOptions(); // opcional
            System.Console.WriteLine("este: "+token);
            if (!Match("SLASH")) return false;
            if (!Match("MTHAN")) return false;

            return true;
        }
        // InputOptions ::= SetInput | KeyTypeCombo
        // SetInput ::= "SET_KEYWORD" "[" IDENTIFIER "]"
        // KeyTypeCombo ::= (KeyAttr TypeAttr?) | (TypeAttr KeyAttr?)
        public static bool InputOptions()
        {
            //_skipSpace();
            // <input/>  → there is no options
            if (_index >= _tokenVector.Count)
                return true;

            var t = _tokenVector[_index].TokenType;

            // <input set[ident]/>
            if (t=="SET_KEYWORD")
            {
                return SetInputAttr();
            }

            // <input key='A' ...> o <input type=[key] ...>
            bool hasKey = false;
            bool hasType = false;

            // try key first
            if (t=="KEY_KEYWORD")
            {
                if (!KeyInputAttr()) return false;
                hasKey = true;
                if(!TypeInputAttr()) return false;
                //_skipSpace();
                t = _tokenVector[_index].TokenType;
            }

            // try type if get
            if (t=="TYPE_KEYWORD")
            {
                //System.Console.WriteLine("debereia entrar aqui");
                if (!TypeInputAttr()) return false;
                KeyInputAttr();
                hasType = true;
            }
            else if (!hasKey)
            {
                // if has no options 
                return true;
            }

            return hasKey || hasType;
        }

       //KeyInputAttr ::= key='A'
        public static bool KeyInputAttr()
        {
            if (!Match("KEY_KEYWORD")) return false;
            if(!Match("EQUAL")) return false;
            if (!Match("CHARACTER"))
                return false;
            return true;
        }

        //TypeInputAttr ::= type=[key]
        public static bool TypeInputAttr()
        {
            if (!Match("TYPE_KEYWORD")) return false;

            if (!Match("EQUAL")) return false;
            if (!Match("OCORCHETES")) return false;

            if (!Match("KEY_KEYWORD")) return false;

            if (!Match("CCORCHETES")) return false;
            return true;
        }

        // set[nombre_variable]
        public static bool SetInputAttr()
        {
            if (!Match("SET_KEYWORD")) return false;

            if (!Match("OCORCHETES")) return false;

            if (!Match("IDENTIFIER")) return false;

            if (!Match("CCORCHETES")) return false;

            return true;
        }


        // Output ::= "<" output ">" PrintList "</" output ">" | OutputGet
        public static bool Output()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("OUTPUT_KEYWORD")) return false;

            //_skipSpace();

            //<output> ... </output>
            if (Match("MTHAN"))
            {
                if (!PrintList()) return false;

                if (!Match("LTHAN")) return false;
                if (!Match("SLASH")) return false;
                if (!Match("OUTPUT_KEYWORD")) return false;
                if (!Match("MTHAN")) return false;

                return true;
            }

            // <output get[ident]/>  (or get[ident]/ if is added)
            if (!OutputInlineAttrs()) return false;

            if (!Match("SLASH")) return false;
            if (!Match("MTHAN")) return false;

            return true;
        }

        // OutputInlineAttrs ::=  "GET_KEYWORD" "[" IDENTIFIER "]"
        public static bool OutputInlineAttrs()
        {
            if (Match("GET_KEYWORD"))
            {
                if (!Match("OCORCHETES")) return false;
                if (!Match("IDENTIFIER")) return false;
                if (!Match("CCORCHETES")) return false;
                return true;
            }
            return false;
        }
        //PrintList ::= Expr ("," Expr)*
        public static bool PrintList()
        {
            // </output>
            if (PeekType(0) == "LTHAN" &&
                PeekType(1) == "SLASH" &&
                PeekType(2) == "OUTPUT_KEYWORD")
            {
                return true;
            }

            // PrintList ::= Expr ("," Expr)* ;
            if (!Expr()) return false;

            while (Match("COMMA"))
            {
                if (!Expr()) return false;
            }
            Match("SEMICOLON");
            return true;
        }

    }
}