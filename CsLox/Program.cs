namespace CsLox;

public class Program
{
    private static int Main(string[] args)
    {
        CsLox csLox = new CsLox();
        csLox.Run(args);

        if (ErrorHandler.HadError)
        {
            Environment.Exit(65);
        }
        return 0;
    }
}

