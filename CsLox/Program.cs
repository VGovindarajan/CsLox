namespace CsLox;

public class Program
{
    private static int Main(string[] args)
    {
        CsLox csLox = new CsLox();

        csLox.Run(args);

        if (CsLoxErrorHandler.HadParseError)
        {
            Environment.Exit(65);
        }
        if (CsLoxErrorHandler.HadRuntimeError)
        {
            Environment.Exit(70);
        }
        return 0;
    }

    private static int PrintAst()
    {
        Expr expr = new BinaryExpr(
            new UnaryExpr(new Token(TokenType.MINUS, "-", "-", 1), new LiteralExpr(TokenType.NUMBER, 123)),
            new Token(TokenType.STAR, "*", "*", 1),
            new GroupingExpr(new LiteralExpr(TokenType.NUMBER, 45.67))
            );
        Console.WriteLine(new AstPrinter().Print(expr));
        return 0;
    }
}

