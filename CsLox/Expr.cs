namespace CsLox
{
    public abstract record Expr() {
        public abstract T Accept<T>(IVisitor<T> visitor);
    };
    public record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr() { 
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
    public record UnaryExpr() : Expr{
        public OperatorRepr? Unary { get; init; }
        public Expr? Right { get; init; }
        private HashSet<string> _allowedSet = new HashSet<string>() { "!","-" };
        public UnaryExpr(string Unary, Expr Right) :this
            (
            )
        {
            if (_allowedSet.Contains(Unary))
            {
                this.Unary = new OperatorRepr(Unary);
                this.Right = Right;
            }
            else
            {
                throw new InvalidOperationException($"{Unary} is not supported.");
            }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    };
    public record GroupingExpr(Expr Expression) : Expr {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    };
    public record LiteralExpr(TokenType TokenType, string Literal) : Expr{
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
    public record OperatorRepr()
    {
        private HashSet<string> _allowedSet = new HashSet<string>()
        {
            "==",
            "!=",
            "<",
            "<=",
            ">",
            ">=",
            "+",
            "-",
            "*",
            "/",
            "!"
        };
        public Token? Token { get; init; }
        public OperatorRepr(string Operator) : this()
        {
            if (_allowedSet.Contains(Operator))
            {
                switch (Operator)
                {
                    case "==": Token = new Token(TokenType.EQUAL_EQUAL, "==", "==", 0); break;
                    case "!=": Token = new Token(TokenType.BANG_EQUAL, "!=", "!=", 0); break;
                    case "<": Token = new Token(TokenType.LESS, "<", "<", 0); break;
                    case "<=": Token = new Token(TokenType.LESS_EQUAL, "<=", "<=", 0); break;
                    case ">": Token = new Token(TokenType.GREATER, ">", ">", 0); break;
                    case ">=": Token = new Token(TokenType.GREATER_EQUAL, ">=", ">=", 0); break;
                    case "+": Token = new Token(TokenType.PLUS, "+", "+", 0); break;
                    case "=": Token = new Token(TokenType.MINUS, "-", "-", 0); break;
                    case "*": Token = new Token(TokenType.STAR, "*", "*", 0); break;
                    case "/": Token = new Token(TokenType.SLASH, "/", "/", 0); break;
                    case "!": Token = new Token(TokenType.BANG, "!", "!", 0); break;

                    default:
                        break;
                }
            }
            throw new InvalidOperationException($"{Operator} is not supported.");
        }
    }

}
