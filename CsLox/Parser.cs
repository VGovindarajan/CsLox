using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    public class Parser
    {
        private List<Token> tokens{ get; init; }
        private int currentIndex = 0;
        public Parser(List<Token> tokens) {
            this.tokens = tokens;
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();
            while (Match(TokenType.GREATER, TokenType.GREATER, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;

        }

        private bool Match(params TokenType[] tokenTypes)
        {
            foreach(var tokenType in tokenTypes)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type) {
            if (IsAtEnd())
            {
                return false;
            }
            return Peek().TokenType == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                currentIndex++;
            }
            return Previous();
        }

        private bool IsAtEnd() {
            return Peek().TokenType == TokenType.EOF;
        }
        private Token Peek()
        {
            return tokens[currentIndex];
        }

        private Token Previous()
        {
            return tokens[currentIndex - 1];
        }
    }
}
