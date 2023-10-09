using System.Globalization;
using System.Reflection.Metadata;

namespace CsLox;

public class Scanner
{
    public string Source { get; init; }
    private int currentIndex = 0;
    private int startIndex = 0;
    private int lineNumber = 1;

    private List<Token> tokens = new();

    private readonly Dictionary<string, TokenType> keyWords = new Dictionary<string, TokenType>
    {
        ["and"] = TokenType.AND,
        ["class"] = TokenType.CLASS,
        ["else"] = TokenType.ELSE,
        ["false"] = TokenType.FALSE,
        ["for"] = TokenType.FOR,
        ["fun"] = TokenType.FUN,
        ["if"] = TokenType.IF,
        ["nil"] = TokenType.NIL,
        ["or"] = TokenType.OR,
        ["print"] = TokenType.PRINT,
        ["return"] = TokenType.RETURN,
        ["super"] = TokenType.SUPER,
        ["this"] = TokenType.THIS,
        ["true"] = TokenType.TRUE,
        ["var"] = TokenType.VAR,
        ["while"] = TokenType.WHILE,
    };

    public Scanner(string source)
    {
        Source = source;
    }

    public IEnumerable<Token> Scan()
    {
        while (!IsAtEnd())
        {
            startIndex = currentIndex;
            ScanToken();
        }
        tokens.Add(new Token(TokenType.EOF, "", null, lineNumber));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            //Single character operators
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;

            //Single or double character operators
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;

            //Slash or Comments
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;

            //Ignore white space
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                lineNumber++;
                break;
            //strings
            case '"':
                HandleString();
                break;

            default:
                if (IsDigit(c))
                {
                    HandleNumber();
                }
                else if (IsAlpha(c))
                {
                    HandleIdentifier();
                }
                else
                {
                    ErrorHandler.Report(lineNumber, c.ToString(), $"Unexpected character {c}");
                }
                break;
        }
        ;
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private void HandleNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        //Look for the fractional part

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            //Consume the "."
            Advance();
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(TokenType.NUMBER, Source.Substring(startIndex, currentIndex - startIndex));
    }

    public void HandleIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }
        string text = Source.Substring(startIndex, currentIndex - startIndex);
        if (keyWords.ContainsKey(text))
        {
            AddToken(keyWords[text]);
            return;
        }
        AddToken(TokenType.IDENTIFIER);
    }

    private char PeekNext()
    {
        if ((currentIndex + 1) >= Source.Length)
        {
            return '\0';
        }
        return Source[currentIndex + 1];
    }

    private void HandleString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                lineNumber++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            ErrorHandler.Error(lineNumber, "Unterminated string");
            return;
        }

        //The closing ".
        Advance();

        //Trim the surrounding quotes.
        string value = Source.Substring(startIndex + 1, currentIndex - startIndex - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }
        return Source[currentIndex];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }
        if (Source[currentIndex] != expected)
        {
            return false;
        }
        currentIndex++;
        return true;
    }

    private bool IsAtEnd()
    {
        return currentIndex >= Source.Length;
    }

    private char Advance()
    {
        return Source[currentIndex++];
    }

    private void AddToken(TokenType tokenType)
    {
        AddToken(tokenType, null);
    }

    private void AddToken(TokenType tokenType, object? literal)
    {
        var text = Source.Substring(startIndex, currentIndex - startIndex);
        tokens.Add(new Token(tokenType, text, literal, lineNumber));
    }
}