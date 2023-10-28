namespace CsLox;

public class Token
{

    public TokenType TokenType { get; init; }
    public string? Lexeme { get; init; }
    public char[]? Literal { get; init; }
    public int Line { get; init; }

    public Token(TokenType tokenType, string? lexeme, char[]? literal, int line)
    {
        TokenType = tokenType;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return $"{TokenType.ToString()} : {Lexeme} : {new string(Literal)} : {Line}";
    }
}