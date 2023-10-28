using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public record CsLoxObject(Object Obj, ValueType? ValueType);
    public class Interpreter : IVisitor<CsLoxObject>
    {
        public void Interpret(Expr expr)
        {
            try
            {
                var result = Evaluate(expr);
                Console.WriteLine(Stringify(result));

            }catch (CsLoxRuntimeError clre) {
                CsLoxErrorHandler.ReportRuntimeError(clre);
            }
        }

        public CsLoxObject VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            var methodName = nameof(VisitBinaryExpr);
            var left = Evaluate(binaryExpr.Left);
            var right = Evaluate(binaryExpr.Right);
            var token = binaryExpr.Operator;
            switch (token.TokenType) {
                case TokenType.GREATER: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString()??"") > double.Parse(right.Obj?.ToString() ?? ""), ValueType.BOOL);
                case TokenType.GREATER_EQUAL: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") >= double.Parse(right.Obj.ToString() ?? ""), ValueType.BOOL);
                case TokenType.LESS: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") < double.Parse(right.Obj.ToString() ?? ""), ValueType.BOOL);
                case TokenType.LESS_EQUAL: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") <= double.Parse(right.Obj.ToString() ?? ""), ValueType.BOOL);
                case TokenType.MINUS: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") - double.Parse(right.Obj.ToString() ?? ""), ValueType.NUMBER);
                case TokenType.STAR: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") * double.Parse(right.Obj.ToString() ?? ""), ValueType.NUMBER);
                case TokenType.SLASH: CheckNumberOperands(token, left, right); return new CsLoxObject(double.Parse(left.Obj.ToString() ?? "") / double.Parse(right.Obj.ToString() ?? ""), ValueType.NUMBER);
                case TokenType.PLUS:
                    var l = 0.0;
                    var r = 0.0;
                    var leftIsParsed = double.TryParse(left.Obj.ToString(), out l);
                    var rightIsParsed = double.TryParse(right.Obj.ToString(), out r);
                    if (leftIsParsed && rightIsParsed)
                    {
                        return new CsLoxObject(l + r, ValueType.NUMBER);    
                    }
                    else if (left.ValueType == ValueType.CHAR_ARRAY && right.ValueType == ValueType.CHAR_ARRAY)
                    {
                        var lca = (char[])left.Obj;
                        var rca = (char[])right.Obj;    
                        var length = lca.Length + rca.Length;
                        var ret = new char[length];
                        Array.Copy(lca, ret, lca.Length);
                        Array.Copy(rca, 0, ret, lca.Length, rca.Length);
                        return new CsLoxObject(ret, ValueType.CHAR_ARRAY);
                    }
                    else if(left.ValueType == ValueType.STRING && right.ValueType == ValueType.STRING)
                    {
                        return new CsLoxObject(left.Obj.ToString() + right.Obj.ToString(), ValueType.STRING);

                    }else if(left.ValueType == ValueType.CHAR_ARRAY) {
                        return new CsLoxObject(new string((char[])left.Obj) + right.Obj.ToString(), ValueType.STRING);
                    }
                    else if (right.ValueType == ValueType.CHAR_ARRAY)
                    {
                        return new CsLoxObject(left.Obj.ToString() + new string((char[])right.Obj), ValueType.STRING);
                    }
                    else if (left.ValueType == ValueType.STRING)
                    {
                        return new CsLoxObject(left.Obj.ToString() + right.Obj.ToString(), ValueType.STRING);
                    }
                    else if (right.ValueType == ValueType.CHAR_ARRAY)
                    {
                        return new CsLoxObject(left.Obj.ToString() +right.Obj.ToString(), ValueType.STRING);
                    }
                    else
                    {
                        throw new CsLoxRuntimeError(token, $"+ is only supported between numbers or strings or string on one side, left={left}, right={right}");
                    }
                case TokenType.BANG_EQUAL: return new CsLoxObject(!IsEqual(left, right), ValueType.BOOL);
                case TokenType.EQUAL: return new CsLoxObject(IsEqual(left, right), ValueType.BOOL);
            }
            throw new CsLoxRuntimeError(token, $"Token {token} is not supported in {methodName}");
        }

        public CsLoxObject VisitExpr(Expr expr)
        {
            return expr.Accept(this);
        }

        public CsLoxObject VisitGroupingExpr(GroupingExpr groupingExpr)
        {
            return Evaluate(groupingExpr.Expression);
        }

        private CsLoxObject Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public CsLoxObject VisitLiteralExpr(LiteralExpr literalExpr)
        {

            ValueType valueType = literalExpr.TokenType switch
            {
                TokenType.FALSE => ValueType.BOOL,
                TokenType.TRUE => ValueType.BOOL,
                TokenType.STRING => ValueType.STRING,
                TokenType.CHAR_ARRAY=> ValueType.CHAR_ARRAY,
                TokenType.NUMBER => ValueType.NUMBER,
                TokenType.NIL => throw new NotImplementedException(),
                _ => throw new CsLoxParseException($"{literalExpr.TokenType} is not supported")

            };
            return new CsLoxObject(literalExpr.Value, valueType);
        }

        public CsLoxObject VisitUnaryExpr(UnaryExpr unaryExpr)
        {
            var result = unaryExpr.Accept(this);

            switch (unaryExpr?.Token?.TokenType)
            {
                case TokenType.BANG: return new CsLoxObject(IsTruthy(result), ValueType.BOOL);
                case TokenType.MINUS: return new CsLoxObject(-(double)result.Obj, ValueType.NUMBER);
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

        private void CheckNumberOperand(Token token, CsLoxObject operand)
        {
            if (operand.ValueType == ValueType.NUMBER) { return; }
            throw new CsLoxRuntimeError(token, $"operand {operand} must be a number");

        }
        private void CheckNumberOperands(Token token, CsLoxObject left, CsLoxObject right)
        {
            if ((left.ValueType == ValueType.NUMBER) && (right.ValueType == ValueType.NUMBER)) { return; }
            throw new CsLoxRuntimeError(token, $"operand {left} and {right} must be a numbers");
        }

        private string Stringify(CsLoxObject v)
        {
            if (v.ValueType == ValueType.NIL) { 
                return "nil"; 
            }
            if (v.ValueType == ValueType.NUMBER) {
                var text = Convert.ToString(v.Obj);
                if(!string.IsNullOrEmpty(text) && text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text??string.Empty;
            }else if (v.ValueType == ValueType.CHAR_ARRAY)
            {
                return new string((char[])v.Obj);
            }
            return v.Obj.ToString() ?? string.Empty;
        }
    }
}
