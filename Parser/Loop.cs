using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        // Loop ::= "<" "loop" (LoopHeader)? ">" Block "</" "loop" ">"
        public static bool Loop()
        {
            if (!Match("LTHAN")) return false;
            if (!Match("LOOP_KEYWORD")) return false;

            // <loop>
            if (PeekType(0) == "MTHAN")
            {
                Match("MTHAN");
                BlockCodeLoop();
                return CloseLoopTag();
            }

            // heaer with content
            if (!LoopHeader()) return false;
            NammedSentence();
            if (!Expect("MTHAN")) return false;
            BlockCodeLoop();
            return CloseLoopTag();
        }

        static bool CloseLoopTag()
        {
            if (!MatchDouble("LTHAN", "SLASH")) return false;
            if (!Match("LOOP_KEYWORD")) return false;
            if (!Expect("MTHAN")) return false;
            return true;
        }
        // LoopHeader ::= LoopIn | LoopOf | LoopIt | LoopGeneral
        public static bool LoopHeader()
        {
            int start = _index;
            string startToken = token;

            _silent = true;

            if (LoopIn()) { _silent = false; return true; }
            _index = start; token = startToken;

            if (LoopOf()) { _silent = false; return true; }
            _index = start; token = startToken;

            // loop-it solo si se parece a it: al menos 3 ';' antes de '>'
            if (HeaderLooksLikeLoopIt() && LoopIt()) { _silent = false; return true; }
            _index = start; token = startToken;

            // general: Expr ";" ("pass")?
            if (HeaderLooksLikeLoopDo() && LoopGeneral()) { _silent = false; return true; }

            _index = start; token = startToken;
            _silent = false;
            return false;
        }
        // LoopGeneral ::= Expr ";" ("pass")?
        public static bool LoopGeneral()
        {
            if (!CanStartExpr(PeekType(0))) return false;

            if (!Expr()) return false;

            if (!Expect("SEMICOLON")) return false;

            Match("PASS_KEYWORD"); // optional
            return true;
        }

        //Nammed_sentence ::= ("@" Identifier | id "=" Identifier)
        public static bool NammedSentence()
        {
            if (Match("ARROBA"))
            {
                if (!Match("IDENTIFIER"))
                {
                    return false;
                }
            }else if (Match("ID_KEYWORD"))
            {
                if (!Match("EQUAL"))
                {
                    return false;
                }
                if (!Match("IDENTIFIER"))
                {
                    return false;
                }
            }
            return true;
        }
        static bool HeaderLooksLikeLoopIt()
        {
            int i = _index;
            int semis = 0;

            while (i < _tokenVector.Count && _tokenVector[i].TokenType != "MTHAN")
            {
                if (_tokenVector[i].TokenType == "SEMICOLON") semis++;
                i++;
            }
            return semis >= 3;
        }

        static bool HeaderLooksLikeLoopDo()
        {
            int i = _index;
            while (i < _tokenVector.Count && (i+2) < _tokenVector.Count)
            {
                if (_tokenVector[i].TokenType == "SEMICOLON" 
                && (_tokenVector[i+1].TokenType == "PASS_KEYWORD" || _tokenVector[i+1].TokenType == "MTHAN")) return true;
                i++;
            }
            return false;
        }
        public static bool BlockCodeLoop()
        {
            while (_index < _tokenVector.Count)
            {
                if (PeekType(0) == "LTHAN" &&
                    PeekType(1) == "SLASH" &&
                    PeekType(2) == "LOOP_KEYWORD")
                {
                    return true;
                }

                if (!Statement())
                {
                    Console.WriteLine("Error dentro del bloque <loop> ... </loop>.");
                    return false;
                }
            }

            Console.WriteLine("Error: EOF antes de cerrar </loop>.");
            return false;
        }

        //Loop_in ::= Identifier in identifier
        public static bool LoopIn()
        {
            if (!Match("IDENTIFIER"))
            {
                return false;
            }
            if (!Match("IN_KEYWORD"))
            {
                return false;
            }
            //operate over all properties of an object, so it well fine at this moment (check for complexity at the future) 
            if (!Match("IDENTIFIER"))
            {
                return false;
            }
            return true;
        }
        //Loop_of ::= Identifier of identifier
        public static bool LoopOf()
        {
            if (!Match("IDENTIFIER"))
            {
                return false;
            }
            if (!Match("OF_KEYWORD"))
            {
                return false;
            }
            //operate over an array, so it well fine at this moment (check for complexity at the future) 
            if (!Match("IDENTIFIER"))
            {
                return false;
            }
            return true;
        }
        // LoopIt := Initialization ";" Condition ";" Iteration ";"
        public static bool LoopIt()
        {
            if (!CanStartExpr(PeekType(0))) return false;

            int start = _index;
            string startToken = token;

            if (!AssignmentExpr()) { _index = start; token = startToken; return false; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return false; }

            if (!Expr()) { _index = start; token = startToken; return false; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return false; }

            if (!Expr()) { _index = start; token = startToken; return false; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return false; }

            return true;
        }


        //Loop_do ::= Condition (pass)? ";"
        public static bool LoopDo()
        {
            if (!CanStartExpr(token)) return false;
            bool ok = Expr();
            if (!ok) return false;
            /*if (!Match("SEMICOLON"))
            {
                return false;
            }*/
            /*Match("PASS_KEYWORD");
            if (!Match("MTHAN"))
            {
                return false;
            }*/
            return true;
        }
        static bool CanStartExpr(string t)
        {
            return t == "IDENTIFIER"
                || t == "TRUE_KEYWORD" || t == "FALSE_KEYWORD"
                || t == "INTEGER_NUMBER" || t == "FLOAT_NUMBER"
                || t == "CHARACTER" || t == "STRING"
                || t == "OPARENTHESIS"
                || t == "MINUS" || t == "DIFFERENT";
        }
    }
}