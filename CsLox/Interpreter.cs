using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public class Interpreter : IVisitor<object>
    {
        public void Interpret(Expr expr)
        {
            try
            {
                object value = Evaluate(expr);
                Console.WriteLine(Stringify(value));

            }catch (CsLoxRuntimeError clre) {
                CsLoxErrorHandler.ReportRuntimeError(clre);
            }
        }

        public object VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            var methodName = nameof(VisitBinaryExpr);
            object left = Evaluate(binaryExpr.Left);
            object right = Evaluate(binaryExpr.Right);
            var token = binaryExpr.Operator;
            switch (token.TokenType) {
                case TokenType.GREATER: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString()??"") > double.Parse(right?.ToString() ?? "");
                case TokenType.GREATER_EQUAL: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") >= double.Parse(right?.ToString() ?? "");
                case TokenType.LESS: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") < double.Parse(right?.ToString() ?? "");
                case TokenType.LESS_EQUAL: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") <= double.Parse(right?.ToString() ?? "");
                case TokenType.MINUS: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") - double.Parse(right?.ToString() ?? "");
                case TokenType.STAR: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") * double.Parse(right?.ToString() ?? "");
                case TokenType.SLASH: CheckNumberOperands(token, left, right); return double.Parse(left?.ToString() ?? "") / double.Parse(right?.ToString() ?? "");
                case TokenType.PLUS:
                    var l = 0.0;
                    var r = 0.0;
                    var leftIsParsed = double.TryParse(left.ToString(), out l);
                    var rightIsParsed = double.TryParse(right.ToString(), out r);
                    if (leftIsParsed && rightIsParsed)
                    {
                        return l + r;    
                    }
                    else if (left is char[] @lca && right is char[] @rca)
                    {
                        var length = lca.Length + rca.Length;
                        var ret = new char[length];
                        Array.Copy(lca, ret, lca.Length);
                        Array.Copy(rca, 0, ret, lca.Length, rca.Length);
                        return ret;
                    }
                    else if(left is string @ls && right is string @rs)
                    {
                        return @ls + @rs;

                    }else if(left is char[] @lcaa) {
                        return new string(lcaa) + right.ToString();
                    }
                    else if (right is char[] @rcaa)
                    {
                        return left.ToString() + new string(rcaa);
                    }
                    else
                    {
                        throw new CsLoxRuntimeError(token, $"+ is only supported between numbers or strings or string on one side, left={left}, right={right}");
                    }
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL: return IsEqual(left, right);
            }
            throw new CsLoxRuntimeError(token, $"Token {token} is not supported in {methodName}");
        }

        public object VisitExpr(Expr expr)
        {
            return expr.Accept(this);
        }

        public object VisitGroupingExpr(GroupingExpr groupingExpr)
        {
            return Evaluate(groupingExpr.Expression);
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public object VisitLiteralExpr(LiteralExpr literalExpr)
        {
            return literalExpr.Value;
        }

        public object VisitUnaryExpr(UnaryExpr unaryExpr)
        {
            object result = unaryExpr.Accept(this);

            switch (unaryExpr?.Token?.TokenType)
            {
                case TokenType.BANG: return IsTruthy(result);
                case TokenType.MINUS: return -(double)(result);
                case default(TokenType): throw new InvalidOperationException($"{unaryExpr.Token} is not a supported operator.");
            }
            return result;
        }

        private bool IsTruthy(object result)
        {
            if (result == null) { 
                return false;
            }
            if (result is bool) {
                return (bool)result;
            }
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null)
            {
                return false;
            }
            return left.Equals(right);
        }

        private void CheckNumberOperand(Token token, object operand)
        {
            if (double.TryParse(operand.ToString(), out var _)) { return; }
            throw new CsLoxRuntimeError(token, $"operand {operand} must be a number");

        }
        private void CheckNumberOperands(Token token, object left, object right)
        {
            if (double.TryParse(left.ToString(), out var _) && double.TryParse(right.ToString(), out var _)) { return; }
            throw new CsLoxRuntimeError(token, $"operand {left} and {right} must be a numbers");
        }

        private string Stringify(object obj)
        {
            if (obj is null) { 
                return "nil"; 
            }
            if (obj is double) {
                var text = Convert.ToString(obj);
                if(!string.IsNullOrEmpty(text) && text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text??string.Empty;
            }else if (obj is char[] @ca)
            {
                return new string(ca);
            }
            return obj.ToString() ?? string.Empty;
        }
    }
}
