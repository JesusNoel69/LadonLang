using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using LadonLang.Data;
using LadonLang.Parser.Models;

namespace LadonLang.Parser
{
    public partial class Parser
    {
        //Select ::= "<" select (Identifier|Value) NammedSentence? ">" OptionList "</" select ">"
        public static SelectStmt? Select()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("SELECT_KEYWORD")) return null;
            //at this time it no expression use
            if (!Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER"
                , "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"))
            {
                return null;
            }
            var sentinelValue = _tokenVector[_index-1];
            var sentenceName = NammedSentence();

            if (!Match("MTHAN"))
            {
                return null;
            }
            var options = OptionList();
            if (options==null)
            {
                return null;
            }

            if (!Match("LTHAN")) return null;
            if (!Match("SLASH")) return null;
            if (!Match("SELECT_KEYWORD")) return null;
            if (!Match("MTHAN")) return null;
            return new SelectStmt()
            {
                Name=sentenceName,
                Options=options,
                Value=sentinelValue
            };
        }
        // Option ::= "<" option ( "default" | ("value" "=" (Identifier|Value)) ) ">" Block "</" option ">"
        public static OptionStmt Option()
        {
            bool isDefault = false;
            Token? tokenValue = null;
            if (!Match("LTHAN")) return null;
            if (!Match("OPTION_KEYWORD")) return null;

            if (Match("DEFAULT_KEYWORD"))
            {
                // ok: <option default>
                isDefault=true;
            }
            else
            {
                // <option value=valor1>
                if (!Match("VALUE_KEYWORD")) return null;
                if (!Match("EQUAL")) return null;

                if (!Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER",
                        "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"))
                    return null;
                tokenValue=_tokenVector[_index-1];
            }

            if (!Match("MTHAN")) return null;
            var blockOfCode = BlockOfCodeOption(); 
            if (blockOfCode==null) return null;

            // </option>
            if (!Match("LTHAN")) return null;
            if (!Match("SLASH")) return null;
            if (!Match("OPTION_KEYWORD")) return null;
            if (!Match("MTHAN")) return null;

            return new OptionStmt()
            {
                Block=new BlockStmt(){
                        Statements = blockOfCode
                    },
                IsDefault=isDefault,
                Value=tokenValue
            };
        }
        // OptionList ::= Option*
        public static List<OptionStmt>? OptionList()
        {
            List<OptionStmt> options = [];
            while (PeekType(0) == "LTHAN" && PeekType(1) == "OPTION_KEYWORD")
            {
                var option = Option(); 
                if (option==null) return null;
                options.Add(option);
            }
            return options;
        }
        //need finish
        public static List<StmtNode>? BlockOfCodeOption()
        {
            var statements = new List<StmtNode>();
            while (_index < _tokenVector.Count)
            {
                // stop at </option>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "OPTION_KEYWORD")
                    return statements;
                var stmt = Statement();
                if (stmt==null)
                {
                    Console.WriteLine("Error dentro de <option>...</option>.");
                    return null;
                }
                statements.Add(stmt);
            }
            Console.WriteLine("Error: EOF antes de cerrar </option>.");
            return null;
        }
        // If ::= "<" if Expr ";" NammedSentence? ">" Block "</" if ">" (ElseIf | Else)*
        public static IfStmt? If()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("IF_KEYWORD")) return null;
            var condition = Expr();
            if (condition == null) return null;

            if (!Expect("SEMICOLON")) return null;

            var sentenceName = NammedSentence();

            if (!Expect("MTHAN")) return null;
            var blockOfCode = BlockOfCodeIf();
            if (blockOfCode == null) return null;

            // </if>
            if (!Match("LTHAN")) return null;
            if (!Match("SLASH")) return null;
            if (!Match("IF_KEYWORD")) return null;
            if (!Expect("MTHAN")) return null;
            List<ElifStmt> elifStatements =[];
            while (PeekType(0) == "LTHAN" && PeekType(1) == "ELIF_KEYWORD")
            {
                var elif=ElseIf(); 
                if (elif==null) return null;
                elifStatements.Add(elif);
            }

            ElseStmt? elseStmt = null;
            if (PeekType(0) == "LTHAN" && PeekType(1) == "ELSE_KEYWORD")
            {
                elseStmt = Else();
                if (elseStmt == null) return null;
            }


            return new IfStmt()
            {
                Condition=condition,
                ElifStmtList=elifStatements,
                ElseStmt=elseStmt,
                Name=sentenceName,
                ThenBlock=blockOfCode
            };
        }
        // Else ::= "<" else ">" Block "</" else ">"
        public static ElseStmt? Else()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("ELSE_KEYWORD")) return null;

            if (!Expect("MTHAN")) return null;
            var blockOfCode = BlockOfCodeElse();
            if (blockOfCode==null) return null;

            // </else>
            if (!Match("LTHAN")) return null;
            if (!Match("SLASH")) return null;
            if (!Match("ELSE_KEYWORD")) return null;
            if (!Expect("MTHAN")) return null;

            return new ElseStmt(){
                Block=blockOfCode
            };
        }

       // ElseIf ::= "<" elif Expr ";" ">" Block "</" elif ">"
        public static ElifStmt? ElseIf()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("ELIF_KEYWORD")) return null;
            var condition = Expr();
            if (condition==null) return null;
            if (!Expect("SEMICOLON")) return null;

            if (!Expect("MTHAN")) return null;
            var block = BlockOfCodeElif();
            if (block==null) return null;

            // </elif>
            if (!Match("LTHAN")) return null;
            if (!Match("SLASH")) return null;
            if (!Match("ELIF_KEYWORD")) return null;
            if (!Expect("MTHAN")) return null;

            return new ElifStmt()
            {
                Block=block,
                Condition=condition
            };
        }
        public static BlockStmt? BlockOfCodeIf()
        {
            var block = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                // stop at </if>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "IF_KEYWORD")
                    return block;
                var statement = Statement();
                if (statement==null)
                {
                    Console.WriteLine("Error dentro de <if>...</if>.");
                    return null;
                }
                block.Statements.Add(statement);
            }
            Console.WriteLine("Error: EOF antes de cerrar </if>.");
            return null;
        }
        public static BlockStmt? BlockOfCodeElse()
        {
            var block = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                // stop at </else>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "ELSE_KEYWORD")
                    return block;
                var statement = Statement();
                if (statement==null)
                {
                    Console.WriteLine("Error dentro de <else>...</else>.");
                    return null;
                }
                block.Statements.Add(statement);
            }
            Console.WriteLine("Error: EOF antes de cerrar </else>.");
            return null;
        }
        public static BlockStmt? BlockOfCodeElif()
        {
            var block = new BlockStmt();
            while (_index < _tokenVector.Count)
            {
                // stop at </elif>
                if (PeekType(0) == "LTHAN" && PeekType(1) == "SLASH" && PeekType(2) == "ELIF_KEYWORD")
                    return block;
                var statement = Statement();
                if (statement==null)
                {
                    Console.WriteLine("Error dentro de <elif>...</elif>.");
                    return null;
                }
                block.Statements.Add(statement);
            }
            Console.WriteLine("Error: EOF antes de cerrar </elif>.");
            return null;
        }
        
        /*
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
        }*/

    }
}