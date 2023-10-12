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
        public Token Token { get; init; }
        public Expr Right { get; init; }
        private HashSet<string> _allowedSet = new HashSet<string>() { "!","-" };
        public UnaryExpr(Token Token, Expr Right) :this
            (
            )
        {
            if (_allowedSet.Contains(Token.Lexeme ?? string.Empty))
            {
                this.Right = Right;
                this.Token = Token;
            }
            else
            {
                throw new InvalidOperationException($"{Token} is not supported.");
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
            "!",
            ":",
        };
        public Token Operator { get; init; }
        public OperatorRepr(Token Operator) : this()
        {
            if (Operator!= null && _allowedSet.Contains(Operator.Lexeme??string.Empty))
            {
                this.Operator = Operator;
            }
            throw new InvalidOperationException($"{Operator} is not supported.");
        }
    }

}
