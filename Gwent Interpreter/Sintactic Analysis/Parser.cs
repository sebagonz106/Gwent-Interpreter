using System;
using System.Collections;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;
using Gwent_Interpreter.Statements;

namespace Gwent_Interpreter
{
    class Parser
    {
        TokenEnumerator tokens;
        List<Environment> environments;
        public List<string> Errors { get; private set; }

        public Parser(List<Token> tokens)
        {
            this.tokens = new TokenEnumerator(tokens);
            this.tokens.MoveNext();
            this.Errors = new List<string>();
            this.environments = new List<Environment> { Environment.Global };
        }

        public List<IStatement> Parse()
        {//reset global when calling another body
            return ActionBody();
        }

        List<IStatement> ActionBody()
        {
            List<IStatement> statements = new List<IStatement>();

            do
            {
                try
                {
                    if (MatchAndMove(TokenType.Log))
                    {
                        statements.Add(new Log(Comparison()));
                    }
                    else if (MatchAndMove(TokenType.Identifier))
                    {
                        statements.Add(Declaration());
                    }

                    if (tokens.Current.Type != TokenType.Semicolon)
                    {
                        throw new ParsingError($"Unfinished statement {positionForErrorBuilder}");
                    }
                }
                catch (ParsingError error)
                {
                    Errors.Add(error.Message);
                }

            } while (tokens.MoveNext() && tokens.Current.Type != TokenType.End);

            return statements;
        }

        IStatement Declaration()
        {
            Token variable = tokens.Previous;

            if (tokens.Current.Type == TokenType.Semicolon)
            {
                return (new Declaration(variable, environments[environments.Count - 1]));
            }
            else if (MatchAndMove(new List<TokenType> { TokenType.Asign, TokenType.Increase, TokenType.Decrease }))
            {
                return (new Declaration(variable, environments[environments.Count - 1], tokens.Previous, Comparison()));
            }
            else if (MatchAndMove(TokenType.IncreaseOne) || MatchAndMove(TokenType.DecreaseOne))
            {
                return (new Declaration(variable, environments[environments.Count - 1], tokens.Previous));
            }

            throw new ParsingError($"Invalid declaration: {variable.Value} at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
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

            while (MatchAndMove(new List<TokenType> { TokenType.Multiply, TokenType.Divide }))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Power());
            }

            return expr;
        }

        IExpression Power()
        {
            IExpression expr = Boolean();

            while (MatchAndMove(new List<TokenType> { TokenType.PowerTo}))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Boolean());
            }

            return expr;
        }

        IExpression Boolean()
        {
            IExpression expr = Unary();

            while (MatchAndMove(new List<TokenType> { TokenType.And, TokenType.AndEnd, TokenType.Or, TokenType.OrEnd }))
            {
                expr = new BooleanOperation(tokens.Previous, expr, Unary());
            }

            return expr;
        }

        IExpression Unary()
        {
            while (MatchAndMove(new List<TokenType> { TokenType.Minus, TokenType.Not }))
            {
                return new UnaryOperation(tokens.Previous, Primary());
            }

            return Concatenation();
        }

        IExpression Concatenation()
        {
            IExpression expr = Primary();

            while (MatchAndMove(new List<TokenType> { TokenType.JoinString, TokenType.JoinStringWithSpace }))
            {
                expr = new StringOperation(tokens.Previous, expr, Primary());
            }

            return expr;
        }

        IExpression Primary()
        {
            IExpression expr;

            if (MatchAndMove(TokenType.OpenParen))
            {
                expr = Comparison();
                if (tokens.Current.Type != TokenType.CloseParen)
                    throw new ParsingError($"Unclosed parenthesis {positionForErrorBuilder}");
            }
            else
            {
                if ((new List<TokenType> { TokenType.End, TokenType.Semicolon, TokenType.Comma, TokenType.CloseParen }).Contains(tokens.Current.Type))
                    throw new ParsingError($"Value expected {positionForErrorBuilder}");

                if (tokens.Current.Type == TokenType.Identifier)
                {
                    expr = environments[environments.Count-1][tokens.Current.Value];
                    if(tokens.TryLookAhead!=null && (tokens.TryLookAhead.Type == TokenType.IncreaseOne || tokens.TryLookAhead.Type == TokenType.DecreaseOne))
                    {
                        IStatement stmt = new Declaration(tokens.Current, environments[environments.Count - 1], tokens.TryLookAhead);
                        tokens.MoveNext();
                    }
                }
                else expr = new Atom(tokens.Current);
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

            public Token TryLookAhead => (current < -1 || current == tokens.Count - 1) ? null : tokens[current + 1];

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

        bool MatchAndMove(TokenType typeToMatch)
        {
            if (typeToMatch == tokens.Current.Type)
            {
                tokens.MoveNext();
                return true;
            }
            return false;
        }

        string positionForErrorBuilder
        {
            get
            {
                try
                {
                    return $"after {tokens.Previous.Value}, at {tokens.Previous.Coordinates.Item1}:{tokens.Previous.Coordinates.Item2 + tokens.Previous.Value.Length}";
                }
                catch (Exception)
                {

                    return $"at {tokens.Current.Coordinates.Item1}:{tokens.Current.Coordinates.Item2}"; ;
                }
            }
        }
        #endregion
    }
}
