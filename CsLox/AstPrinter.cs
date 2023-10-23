using System.Text;

namespace CsLox
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept<string>(this);
        }

        private string Parenthesize(string name, params Expr[] expressions)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            
            sb.Append(name);
            if (expressions != null)
            {
                foreach (Expr e in expressions)
                {
                    sb.Append(" ");
                    if (e != null)
                    {
                        sb.Append(e.Accept(this));
                    }
                    sb.Append(" ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }


        public string VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            return Parenthesize(binaryExpr.Operator.Lexeme??string.Empty, binaryExpr.Left, binaryExpr.Right);
        }

        public string VisitExpr(Expr expr)
        {
            return string.Empty;
        }

        public string VisitGroupingExpr(GroupingExpr groupingExpr)
        {
            return Parenthesize("group", groupingExpr.Expression );
        }

        public string VisitLiteralExpr(LiteralExpr literalExpr)
        {
            if (literalExpr.TokenType == TokenType.NIL) {
                return "nil";
            }
            return $"{literalExpr.Value}";
        }

        public string VisitUnaryExpr(UnaryExpr unaryExpr)
        {
            var right = unaryExpr != null ? unaryExpr.Right : null;
            if(right!= null)
            {
                return Parenthesize(unaryExpr?.Token?.Lexeme ?? string.Empty, right);
            }
            return Parenthesize(unaryExpr?.Token?.Lexeme ?? string.Empty);
        }
    }
}
