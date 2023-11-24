using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public interface IStmtVisitor<T>
    {
        T VisitBlockStmt(BlockStmt blockStmt);

        T VisitClassStmt(ClassStmt classStmt);

        T VisitExpressionStmt(ExpressionStmt expressionStmt);

        T VisitFunctionStmt(FunctionStmt functionStmt);

        T VisitIfStmt(IfStmt ifStmt);

        T VisitPrintStmt(PrintStmt printStmt);

        T VisitReturnStmt(ReturnStmt returnStmt);

        T VisitVarStmt(VarStmt varStmt);

        T VisitWhileStmt(WhileStmt whileStmt);
    }
}
