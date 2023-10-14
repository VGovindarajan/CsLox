﻿using System;
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

            }catch (ParseException pe) {
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
                return new LiteralExpr(TokenType.FALSE, "false");
            }
            if (Match(TokenType.TRUE))
            {
                return new LiteralExpr(TokenType.TRUE, "true");
            }
            if (Match(TokenType.NIL))
            {
                return new LiteralExpr(TokenType.NIL, "nil");
            }

            if(Match(TokenType.NUMBER, TokenType.STRING))
            {
                var p = Previous();
                return new LiteralExpr(p.TokenType, p.Lexeme??string.Empty);
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

        private ParseException Error(Token token, string message)
        {
            ErrorHandler.Error(token, message);
            return new ParseException($"Line:{token.Line}, Lexeme:{token.Lexeme ?? string.Empty}, message:{message}");
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
