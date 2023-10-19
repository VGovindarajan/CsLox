using System.Linq.Expressions;

namespace CsLox;

public class CsLox
{
    public CsLox() { }

    public void Run(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage : CsLox [Filename]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Missing File : {args[0]}");
                Environment.Exit(127);
            }
            RunFileScan(args[0]);
        }
        else
        {
            Console.WriteLine("Enter Lox expression ...");
            RunPromptScan();
        }
    }

    void RunFileScan(string path)
    {
        var text = File.ReadAllText(path);
        RunScan(text);
    }

    void RunPromptScan()
    {
        while (true)
        {
            Console.Write(">");
            var line = Console.ReadLine();
            if (line == null || line.Length == 0)
            {
                break;
            }
            RunScan(line);
            ErrorHandler.HadError = false;
            Console.Write(">");
        }
        return;
    }

    void RunScan(string line)
    {
        Scanner scanner = new Scanner(line.ToCharArray());
        IEnumerable<Token> tokens = scanner.Scan();
        //foreach (var token in tokens)
        //{
        //    Console.WriteLine(token);
        //}
        Parser parser = new Parser(tokens.ToList());
        Expr expr = parser.Parse();
        if (expr != null)
        {
            Console.WriteLine(new PrintVisitor().Print(expr));
        }

        return;
    }
}