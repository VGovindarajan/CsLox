using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public abstract record Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public record BlockStmt(List<Stmt> Statements) : Stmt()
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    public record ClassStmt(Token Name, VariableExpr SuperClass, List<FunctionStmt> Methods) : Stmt()
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitClassStmt(this);
        }
    }

    public record ExpressionStmt(Expr Expr) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public record FunctionStmt(Token Name, List<Token> Params, List<Stmt> Body) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }

    public record IfStmt(Expr Condition, Stmt ThenBranch, Stmt ElseBranch) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }

    public record PrintStmt(Expr Expr) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public record ReturnStmt(Token Keyword, Expr Value) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }

    public record VarStmt(Token Keyword, Expr Initializer) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }

    public record WhileStmt(Expr Condition, Stmt Body) : Stmt
    {
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}
