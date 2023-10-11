namespace CsLox
{
    public interface IVisitor<T>
    {
        T VisitExpr(Expr expr);
        T VisitBinaryExpr(BinaryExpr binaryExpr);
        T VisitUnaryExpr(UnaryExpr unaryExpr);
        T VisitGroupingExpr(GroupingExpr groupingExpr);
        T VisitLiteralExpr(LiteralExpr literalExpr);
    }

}
