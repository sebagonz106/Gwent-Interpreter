using System;
using System.Collections;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;
using System.Text;

namespace Gwent_Interpreter
{
    class Parser
    {
       TokenEnumerator tokens;

        public Parser(List<Token> tokens)
        {
            this.tokens = new TokenEnumerator(tokens);
            this.tokens.MoveNext();
        }

        public List<IExpression> Parse()
        {
            List<IExpression> expressions = new List<IExpression>();

            expressions.Add(Comparison());

            return expressions;
        }

        #region Expression Builders
        IExpression Comparison()
        {
            IExpression expr = ValueExpression();

            if (MatchAndMove(new List<TokenType> { TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual, TokenType.Equals, TokenType.NotEquals }))
            {
                expr = new ComparingOperation(tokens.Previous, expr, ValueExpression());
            }

            return expr;
        }

        IExpression ValueExpression()
        {
            return Term();
        }

        IExpression Term()
        {
            IExpression expr = Factor();

            while (MatchAndMove(new List<TokenType> { TokenType.Plus, TokenType.Minus}))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Factor());
            }

            return expr;
        }

        IExpression Factor()
        {
            IExpression expr = Power();

            if (MatchAndMove(new List<TokenType> { TokenType.Multiply, TokenType.Divide }))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Power());
            }

            return expr;
        }

        IExpression Power()
        {
            IExpression expr = Unary();

            if (MatchAndMove(new List<TokenType> { TokenType.PowerTo}))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Unary());
            }

            return expr;
        }

        IExpression Unary()
        {
            if (MatchAndMove(new List<TokenType> { TokenType.Minus, TokenType.Not }))
            {
                return new UnaryOperation(tokens.Previous, Primary());
            }

            return Primary();
        }

        IExpression Primary()
        {
            IExpression expr;

            if (tokens.Current.Type == TokenType.OpenParen)
            {
                tokens.MoveNext();
                expr = ValueExpression();
                if (tokens.Current.Type != TokenType.CloseParen) throw new NotImplementedException("unclosed parenthesis");
            }
            else
            {
                expr = new Atom(tokens.Current);
            }

            tokens.MoveNext();
            return expr;
        }
        #endregion

        #region Utils
        class TokenEnumerator : IEnumerator<Token>
        {
            List<Token> tokens;
            int current = -1;

            public TokenEnumerator(List<Token> tokens) { this.tokens = tokens; }

            public Token Current => current >= 0 ? tokens[current] : null;

            public Token Previous => current >= 1 ? tokens[current - 1] : null;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                if (current < -1 || current == tokens.Count - 1)
                {
                    current = -2;
                    return false;
                }
                current++;
                return true;
            }

            public void Reset()
            {
                current = -1;
                this.MoveNext();
            }
        }

        bool MatchAndMove(List<TokenType> typesToMatch)
        {
            if (typesToMatch.Contains(tokens.Current.Type))
            {
                tokens.MoveNext();
                return true;
            }
            return false;
        }
        #endregion
    }
}
