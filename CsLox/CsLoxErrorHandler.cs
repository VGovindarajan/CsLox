namespace CsLox;

public static class CsLoxErrorHandler
{
    public static bool HadParseError = false;
    public static bool HadRuntimeError = false;

    public static void ParseError(int line, string message)
    {
        ReportParseError(line, string.Empty, message);
    }

    public static void ParseError(Token token, string message)
    {
        var lexWhere = (token.TokenType == TokenType.EOF) ? " at end" : $" at {token.Lexeme}";
        ReportParseError(token.Line, lexWhere, message);
    }

    public static void ReportParseError(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line : {line}] ParseError : {where} : {message}");
        HadParseError = true;
    }

    public static void ReportRuntimeError(CsLoxRuntimeError clre)
    {
        Console.Error.WriteLine($"[line : {clre.Token.Line}] RuntimeError : {clre.Message}");
        HadRuntimeError = true;
    }
}
