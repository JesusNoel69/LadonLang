using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        //Select ::= "<" select (Identifier|Value) NammedSentence? ">" OptionList "</" select ">"
        public static bool Select()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("SELECT_KEYWORD")) return false;
            //at this time it no expression use
            if (!Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER"
                , "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"))
            {
                return false;
            }
            NammedSentence();

            if (!Match("MTHAN"))
            {
                return false;
            }
            System.Console.WriteLine(token);

            if (!OptionList())
            {
                return false;
            }

            if (!Match("LTHAN")) return false;
            if (!Match("SLASH")) return false;
            if (!Match("SELECT_KEYWORD")) return false;
            if (!Match("MTHAN")) return false;
            return true;
        }
        // Option ::= "<" option ( "default" | ("value" "=" (Identifier|Value)) ) ">" Block "</" option ">"
        public static bool Option()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("OPTION_KEYWORD")) return false;

            if (Match("DEFAULT_KEYWORD"))
            {
                // ok: <option default>
            }
            else
            {
                // <option value=valor1>
                if (!Match("VALUE_KEYWORD")) return false;
                if (!Match("EQUAL")) return false;

                if (!Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER",
                        "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"))
                    return false;
            }

            if (!Match("MTHAN")) return false;

            if (!BlockOfCodeOption()) return false;

            // </option>
            if (!Match("LTHAN")) return false;
            if (!Match("SLASH")) return false;
            if (!Match("OPTION_KEYWORD")) return false;
            if (!Match("MTHAN")) return false;

            return true;
        }
        // OptionList ::= Option*
        public static bool OptionList()
        {
            while (PeekType(0) == "LTHAN" && PeekType(1) == "OPTION_KEYWORD")
            {
                if (!Option()) return false;
            }
            return true;
        }
        //need finish
        public static bool BlockOfCodeOption()
        {
            while (_index < _tokenVector.Count)
            {
                // stop at </option>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "OPTION_KEYWORD")
                    return true;

                if (!Statement())
                {
                    Console.WriteLine("Error dentro de <option>...</option>.");
                    return false;
                }
            }
            Console.WriteLine("Error: EOF antes de cerrar </option>.");
            return false;
        }
        // If ::= "<" if Expr ";" NammedSentence? ">" Block "</" if ">" (ElseIf | Else)*
        public static bool If()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("IF_KEYWORD")) return false;

            if (!Expr()) return false;

            if (!Expect("SEMICOLON")) return false;

            NammedSentence();

            if (!Expect("MTHAN")) return false;

            if (!BlockOfCodeIf()) return false;

            // </if>
            if (!Match("LTHAN")) return false;
            if (!Match("SLASH")) return false;
            if (!Match("IF_KEYWORD")) return false;
            if (!Expect("MTHAN")) return false;

            while (PeekType(0) == "LTHAN" && PeekType(1) == "ELIF_KEYWORD")
            {
                if (!ElseIf()) return false;
            }

            if (PeekType(0) == "LTHAN" && PeekType(1) == "ELSE_KEYWORD")
            {
                if (!Else()) return false;
            }

            return true;
        }
        // Else ::= "<" else ">" Block "</" else ">"
        public static bool Else()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("ELSE_KEYWORD")) return false;

            if (!Expect("MTHAN")) return false;

            if (!BlockOfCodeElse()) return false;

            // </else>
            if (!Match("LTHAN")) return false;
            if (!Match("SLASH")) return false;
            if (!Match("ELSE_KEYWORD")) return false;
            if (!Expect("MTHAN")) return false;

            return true;
        }

       // ElseIf ::= "<" elif Expr ";" ">" Block "</" elif ">"
        public static bool ElseIf()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("ELIF_KEYWORD")) return false;

            if (!Expr()) return false;
            if (!Expect("SEMICOLON")) return false;

            if (!Expect("MTHAN")) return false;

            if (!BlockOfCodeElif()) return false;

            // </elif>
            if (!Match("LTHAN")) return false;
            if (!Match("SLASH")) return false;
            if (!Match("ELIF_KEYWORD")) return false;
            if (!Expect("MTHAN")) return false;

            return true;
        }
        public static bool BlockOfCodeIf()
        {
            while (_index < _tokenVector.Count)
            {
                // stop at </if>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "IF_KEYWORD")
                    return true;

                if (!Statement())
                {
                    Console.WriteLine("Error dentro de <if>...</if>.");
                    return false;
                }
            }
            Console.WriteLine("Error: EOF antes de cerrar </if>.");
            return false;
        }
        public static bool BlockOfCodeElse()
        {
            while (_index < _tokenVector.Count)
            {
                // stop at </else>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "ELSE_KEYWORD")
                    return true;

                if (!Statement())
                {
                    Console.WriteLine("Error dentro de <else>...</else>.");
                    return false;
                }
            }
            Console.WriteLine("Error: EOF antes de cerrar </else>.");
            return false;
        }
        public static bool BlockOfCodeElif()
        {
            while (_index < _tokenVector.Count)
            {
                // stop at </elif>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "ELIF_KEYWORD")
                    return true;

                if (!Statement())
                {
                    Console.WriteLine("Error dentro de <elif>...</elif>.");
                    return false;
                }
            }
            Console.WriteLine("Error: EOF antes de cerrar </elif>.");
            return false;
        }

    }
}