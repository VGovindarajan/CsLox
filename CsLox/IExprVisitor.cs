namespace CsLox
{
    public interface IExprVisitor<T>
    {
        T VisitExpr(Expr expr);
        T VisitBinaryExpr(BinaryExpr binaryExpr);
        T VisitUnaryExpr(UnaryExpr unaryExpr);
        T VisitGroupingExpr(GroupingExpr groupingExpr);
        T VisitLiteralExpr(LiteralExpr literalExpr);

        T VisitVariableExpr(VariableExpr variableExpr);

       
    }

}
