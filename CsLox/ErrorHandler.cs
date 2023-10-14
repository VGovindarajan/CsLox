namespace CsLox;

public static class ErrorHandler
{
    public static bool HadError = false;

    public static void Error(int line, string message)
    {
        Report(line, string.Empty, message);
    }

    public static void Error(Token token, string message)
    {
        var lexWhere = (token.TokenType == TokenType.EOF) ? " at end" : $" at {token.Lexeme}";
        Report(token.Line, lexWhere, message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line : {line}] Error : {where} : {message}");
        HadError = true;
    }
}
