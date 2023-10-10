using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public interface Visitor
    {
        void VisitExpr(Expr expr);
        void VisitBinaryExpr(BinaryExpr binaryExpr);
        void VisitUnaryExpr(UnaryExpr unaryExpr);
        void VisitGroupingExpr(GroupingExpr groupingExpr);
    }
    public abstract record Expr() {
        public abstract void Accept(Visitor visitor);
    };
    public record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr() { 
        public override void Accept(Visitor visitor)
        {
            visitor.VisitBinaryExpr(this);
        }
    }
    public record UnaryExpr(string unary, Expr right) : Expr{
        public override void Accept(Visitor visitor)
        {
            visitor.VisitUnaryExpr(this);
        }
    };
    public record GroupingExpr(Expr expression) : Expr {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitGroupingExpr(this);
        }
    };
    public record Operator()
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
            "/"
        };
        private string? _operator { get; init; }
        public Operator(string op) : this()
        {
            if (_allowedSet.Contains(op))
            {
                _operator = op;
            }
            throw new InvalidOperationException($"{op} is not supported.");

        }

    }

}
