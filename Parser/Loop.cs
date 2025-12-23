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
        // Loop ::= "<" "loop" (LoopHeader)? ">" Block "</" "loop" ">"
        public static LoopStmt? Loop()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("LOOP_KEYWORD")) return null;
            LoopHeader? header = null;
            Token? nameTok = null;
            // <loop>
            if (PeekType(0) == "MTHAN")
            {
                Match("MTHAN");
            }
            else
            {
                header = LoopHeader();
                if (header == null) return null;

                // optional name: @id or id=foo
                nameTok = NammedSentence(); // Token? (optional)
                if (!Expect("MTHAN")) return null;
            }

            var block = BlockCodeLoop();
            if (block == null) return null;

            if (!CloseLoopTag()) return null;

            return new LoopStmt
            {
                Name = nameTok?.Lexeme ?? string.Empty,   // ajusta Lexeme/Value según tu Token
                LoopHeader = header ?? new LoopHeader(),  // o null si permites sin header
                Block = block
            };
        }
        
        static bool CloseLoopTag()
        {
            if (!MatchDouble("LTHAN", "SLASH")) return false;
            if (!Match("LOOP_KEYWORD")) return false;
            if (!Expect("MTHAN")) return false;
            return true;
        }
        // LoopHeader ::= LoopIn | LoopOf | LoopIt | LoopGeneral
        public static LoopHeader? LoopHeader()
        {
            int start = _index;
            string startToken = token;

            _silent = true;
            var headerIn = LoopIn();
            if (headerIn!=null) { _silent = false; return headerIn; }
            _index = start; token = startToken;
            var headerOf=LoopOf();
            if (headerOf!=null) { _silent = false; return headerOf; }
            _index = start; token = startToken;
            if (HeaderLooksLikeLoopIt())
            {
                var headerIt = LoopIt();
                if (headerIt != null) { _silent = false; return headerIt; }
                _index = start; token = startToken;
            }

            // general: Expr ";" ("pass")?
            if (HeaderLooksLikeLoopDo())
            {
                var header = LoopGeneral();
                if (header != null) { _silent = false; return header; }
                _index = start; token = startToken;
            }

            _silent = false;
            return null;
        }
        // LoopGeneral ::= Expr ";" ("pass")?
        public static LoopDo? LoopGeneral()
        {
            if (!CanStartExpr(PeekType(0))) return null;
            var condition= Expr();
            if (condition == null) return null;

            if (!Expect("SEMICOLON")) return null;

            bool havePass = false;
            if (Match("PASS_KEYWORD")) // optional
            {
                havePass=true;
            }
            return new()
            {
                Condition=condition,
                IsPostTest=havePass
            };
        }

        //Nammed_sentence ::= ("@" Identifier | id "=" Identifier)
        public static Token? NammedSentence()
        {
            Token name = null;
            if (Match("ARROBA"))
            {
                if (!Match("IDENTIFIER"))
                {
                    return null;
                }
                name = _tokenVector[_index-1];
            }else if (Match("ID_KEYWORD"))
            {
                if (!Match("EQUAL"))
                {
                    return null;
                }
                if (!Match("IDENTIFIER"))
                {
                    return null;
                }
                name = _tokenVector[_index-1];
            }
            return name;
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
            //while (i + 1 < _tokenVector.Count && _tokenVector[i].TokenType != "MTHAN")
            while (i < _tokenVector.Count && (i+2) < _tokenVector.Count)
            {
                if (_tokenVector[i].TokenType == "SEMICOLON" 
                && (_tokenVector[i+1].TokenType == "PASS_KEYWORD" || _tokenVector[i+1].TokenType == "MTHAN")) return true;
                i++;
            }
            return false;
        }
        public static BlockStmt? BlockCodeLoop()
        {
            var block = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                if (PeekType(0) == "LTHAN" &&
                    PeekType(1) == "SLASH" &&
                    PeekType(2) == "LOOP_KEYWORD")
                {
                    return block;
                }
                var stmt = Statement();
                if (stmt==null)
                {
                    Console.WriteLine("Error dentro del bloque <loop> ... </loop>.");
                    return null;
                }
                block.Statements.Add(stmt);
            }

            Console.WriteLine("Error: EOF antes de cerrar </loop>.");
            return null;
        }

        //Loop_in ::= Identifier in identifier
        public static LoopIn? LoopIn()
        {
            if (!Match("IDENTIFIER"))
            {
                return null;
            } 
            var iterator = _tokenVector[_index - 1];
            if (!Match("IN_KEYWORD"))
            {
                return null;
            }
            //operate over all properties of an object, so it well fine at this moment (check for complexity at the future) 
            if (!Match("IDENTIFIER"))
            {
                return null;
            }
            var iterable = _tokenVector[_index - 1];
            return new LoopIn{
                    IteratorName=iterator, 
                    Iterable=iterable
                };

        }
        //Loop_of ::= Identifier of identifier
        public static LoopOf? LoopOf()
        {
            if (!Match("IDENTIFIER"))
            {
                return null;
            }
            var each = _tokenVector[_index - 1];
            if (!Match("OF_KEYWORD"))
            {
                return null;
            }
            //operate over an array, so it well fine at this moment (check for complexity at the future) 
            if (!Match("IDENTIFIER"))
            {
                return null;
            }
            var iterable = _tokenVector[_index - 1];
            return new LoopOf { EachName = each, Iterable = iterable };
        }
        // LoopIt := Initialization ";" Condition ";" Iteration ";"
        public static LoopIt? LoopIt()
        {
            if (!CanStartExpr(PeekType(0))) return null;

            int start = _index;
            string startToken = token;
            var assign = AssignmentExpr();
            if (assign == null) { _index = start; token = startToken; return null; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return null; }
            var condition = Expr();
            if (condition==null) { _index = start; token = startToken; return null; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return null; }
            var iterable = Expr();
            if (iterable == null) { _index = start; token = startToken; return null; }
            if (!Match("SEMICOLON")) { _index = start; token = startToken; return null; }

            return new LoopIt()
            {
                Init = assign,
                Condition=condition,
                Step=iterable
            };
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