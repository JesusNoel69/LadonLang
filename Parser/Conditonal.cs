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
                throw new UnexpectedTokenException(
                    "Value or identifier after '<select'",
                    CurrentToken()
                );
            }
            var sentinelValue = _tokenVector[_index-1];
            var sentenceName = NammedSentence();

            if (!Expect("MTHAN"))
                throw new UnexpectedTokenException("MTHAN", CurrentToken());
            var options = OptionList();
            if (options==null)
            {
                throw new ParserExeption("Select: Invalid option list", CurrentToken());
            }
            if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN ('</select>')", CurrentToken());
            if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH ('</select>')", CurrentToken());
            if (!Match("SELECT_KEYWORD")) throw new UnexpectedTokenException("SELECT_KEYWORD", CurrentToken());
            if (!Expect("MTHAN")) throw new UnexpectedTokenException("MTHAN", CurrentToken());
            return new SelectStmt()
            {
                Name=sentenceName,
                Options=options,
                Value=sentinelValue
            };
        }
        // Option ::= "<" option ( "default" | ("value" "=" (Identifier|Value)) ) ">" Block "</" option ">"
        public static OptionStmt? Option()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("OPTION_KEYWORD")) return null;
            bool isDefault = false;
            Token? tokenValue = null;
            if (Match("DEFAULT_KEYWORD"))
            {
                // ok: <option default>
                isDefault=true;
            }
            else
            {
                // <option value=valor1>
                if (!Match("VALUE_KEYWORD"))
                {
                    throw new UnexpectedTokenException("DEFAULT_KEYWORD or VALUE_KEYWORD", CurrentToken());
                }
                if (!Match("EQUAL"))
                {
                    throw new UnexpectedTokenException("EQUAL", CurrentToken());
                }

                if (!Match("TRUE_KEYWORD", "FALSE_KEYWORD", "INTEGER_NUMBER",
                        "FLOAT_NUMBER", "CHARACTER", "STRING", "IDENTIFIER"))
                {
                    throw new UnexpectedTokenException(
                        "Value or identifie after 'value='",
                        CurrentToken()
                    );
                }
                tokenValue=_tokenVector[_index-1];
            }
            Expect("MTHAN");
            var blockOfCode = BlockOfCodeOption(); 
            if (blockOfCode == null)
            {
                throw new ParserExeption("Option: Invalid block", CurrentToken());
            }
            // </option>
            if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN ('</option>')", CurrentToken());
            if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH ('</option>')", CurrentToken());
            if (!Match("OPTION_KEYWORD")) throw new UnexpectedTokenException("OPTION_KEYWORD", CurrentToken());
            if (!Expect("MTHAN")) throw new UnexpectedTokenException("MTHAN", CurrentToken());


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
                if (option == null)
                {
                    throw new ParserExeption("OptionList: Invalid option", CurrentToken());
                }
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
                    throw new ParserExeption("Error before to close <option>...</option> block", CurrentToken());
                }
                statements.Add(stmt);
            }
            throw new ParserExeption("EOF before to close </option> block", CurrentToken());
        }
        // If ::= "<" if Expr ";" NammedSentence? ">" Block "</" if ">" (ElseIf | Else)*
        public static IfStmt? If()
        {
            if (!Match("LTHAN")) return null;
            if (!Match("IF_KEYWORD")) return null;
            var condition = Expr();
            if (condition == null)
            {
                throw new ParserExeption("If: Invalid condition", CurrentToken());
            }
            Expect("SEMICOLON");
            var sentenceName = NammedSentence();
            Expect("MTHAN");
            var blockOfCode = BlockOfCodeIf();
            if (blockOfCode == null)
            {
                throw new ParserExeption("If: Invalid condition", CurrentToken());
            }

            // </if>
            if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN ('</if>')", CurrentToken());
            if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH ('</if>')", CurrentToken());
            if (!Match("IF_KEYWORD")) throw new UnexpectedTokenException("IF_KEYWORD", CurrentToken());        
            Expect("MTHAN");
            List<ElifStmt> elifStatements =[];
            while (PeekType(0) == "LTHAN" && PeekType(1) == "ELIF_KEYWORD")
            {
                var elif=ElseIf(); 
                if (elif == null)
                {
                    throw new ParserExeption("If: Invalid elif block", CurrentToken());
                }
                elifStatements.Add(elif);
            }

            ElseStmt? elseStmt = null;
            if (PeekType(0) == "LTHAN" && PeekType(1) == "ELSE_KEYWORD")
            {
                elseStmt = Else();
                if (elseStmt == null)
                {
                    throw new ParserExeption("If: Invalid else block.", CurrentToken());
                }
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
            Expect("MTHAN");
            var blockOfCode = BlockOfCodeElse();
            if (blockOfCode == null)
            {
                throw new ParserExeption("Else: Invalid block", CurrentToken());
            }
            // </else>
            if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN ('</else>')", CurrentToken());
            if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH ('</else>')", CurrentToken());
            if (!Match("ELSE_KEYWORD")) throw new UnexpectedTokenException("ELSE_KEYWORD", CurrentToken());
            Expect("MTHAN");
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
            if (condition == null)
            {
                throw new ParserExeption("Elif: Invalid condition", CurrentToken());
            }
            Expect("SEMICOLON");
            Expect("MTHAN");
            var block = BlockOfCodeElif();
            if (block == null)
            {
                throw new ParserExeption("Elif: Invalid block", CurrentToken());
            }
            // </elif>
            if (!Match("LTHAN")) throw new UnexpectedTokenException("LTHAN ('</elif>')", CurrentToken());
            if (!Match("SLASH")) throw new UnexpectedTokenException("SLASH ('</elif>')", CurrentToken());
            if (!Match("ELIF_KEYWORD")) throw new UnexpectedTokenException("ELIF_KEYWORD", CurrentToken());
            Expect("MTHAN");

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
                    throw new ParserExeption("Error into <if>...</if> block", CurrentToken());
                }
                block.Statements.Add(statement);
            }
            throw new ParserExeption("EOF before to close </if>.", CurrentToken());
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
                    throw new ParserExeption("Error into <else>...</else> block", CurrentToken());
                }
                block.Statements.Add(statement);
            }
            throw new ParserExeption("EOF before to close </else>.", CurrentToken());
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
                    throw new ParserExeption("Error into <elif>...</elif> block", CurrentToken());
                }
                block.Statements.Add(statement);
            }
            throw new ParserExeption("EOF before to close </elif>.", CurrentToken());
        }
    }
}