using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public Expr Parse()
        {
            try
            {
                return Expression();

            }catch (CsLoxParseException pe) {
                Console.WriteLine($"{pe.Message}");
                throw;
            }
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
        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.PLUS, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();
            while (Match(TokenType.STAR, TokenType.SLASH))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new UnaryExpr(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if(Match(TokenType.FALSE))
            {
                return new LiteralExpr(TokenType.FALSE, "false".ToCharArray());
            }
            if (Match(TokenType.TRUE))
            {
                return new LiteralExpr(TokenType.TRUE, "true".ToCharArray());
            }
            if (Match(TokenType.NIL))
            {
                return new LiteralExpr(TokenType.NIL, "nil".ToCharArray());
            }

            if(Match(TokenType.NUMBER))
            {
                var p = Previous();
                return new LiteralExpr(p.TokenType, p.Literal);
            }
            if (Match(TokenType.CHAR_ARRAY))
            {
                var p = Previous();
                return new LiteralExpr(p.TokenType, p.Literal);
            }

            if (Match(TokenType.STRING))
            {
                var p = Previous();
                return new LiteralExpr(p.TokenType, p.Literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expect Expression");
        }

        private Token Consume(TokenType tokenType, string message) {
            if (Check(tokenType))
            {
                return Advance();
            }
            throw Error(Peek(), message);
        }

        private CsLoxParseException Error(Token token, string message)
        {
            CsLoxErrorHandler.ParseError(token, message);
            return new CsLoxParseException($"Line:{token.Line}, Lexeme:{token.Lexeme ?? string.Empty}, message:{message}");
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

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().TokenType == TokenType.SEMICOLON) {
                    return;
                }

                switch (Peek().TokenType) {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }
    }
}
