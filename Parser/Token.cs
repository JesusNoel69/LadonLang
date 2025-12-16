using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Lexer;

namespace LadonLang.Parser
{
    public class Token
    {
        public DelimiterSymbols Type { get; }
        public string Lexeme { get; }
        public object? Literal { get; }
        public int Line { get; }

        public Token(DelimiterSymbols type, string lexeme, object? literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }
    }
}