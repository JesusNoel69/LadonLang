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
    public class DuplicateSymbolException : ParserExeption
    {
        public DuplicateSymbolException(string name, Token token)
            : base($"Duplicated symbol: '{name}'", token) { }
    }
    public class UndeclaredSymbolException : ParserExeption
    {
        public UndeclaredSymbolException(Token token)
            : base("Undeclared identifier", token) { }
    }
    public class TypeMismatchException : ParserExeption
    {
        public TypeMismatchException(string message, Token token)
            : base(message, token) { }
    }
}