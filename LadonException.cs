using LadonLang.Data;

namespace LadonLang
{
    public class ParserExeption: Exception
    {
        public Token Token { get; set; }
        public ParserExeption(string message, Token token)
        :base($"{message} (token: {token.TokenType}, lexeme: {token.Lexeme})")
        {
            Token = token;
        }
    }
    public class UnexpectedTokenException : ParserExeption
    {
        public UnexpectedTokenException(string expected, Token found)
            : base($"Expected {expected}, found {found.TokenType}", found)
        {
        }
    }
    public class InvalidTypeException : ParserExeption
    {
        public InvalidTypeException(Token token)
            : base("Invalid type declaration", token)
        {
        }
    }

}