using System;
using System.Collections;
using System.Collections.Generic;
using Gwent_Interpreter.Expressions;
using Gwent_Interpreter.Statements;
using System.Linq;

namespace Gwent_Interpreter
{
    class Parser
    {
        TokenEnumerator tokens;
        Stack<Environment> environments;
        public List<string> Errors { get; private set; }

        public Parser(List<Token> tokens)
        {
            this.tokens = new TokenEnumerator(tokens);
            this.tokens.MoveNext();
            this.Errors = new List<string>();
            this.environments = new Stack<Environment>();
            this.environments.Push(Environment.Global);
        }

        public List<IStatement> Parse()
        {
            List<IStatement> list = new List<IStatement>();

            if (MatchAndMove(TokenType.EffectDeclaration) && MatchAndMove(TokenType.OpenBrace))
            {
                if(MatchAndMove(TokenType.Action) && MatchAndMove(TokenType.DoubleDot) && MatchAndMove(TokenType.OpenBrace))
                {
                    list.Add(ActionBody());
                }

                if(!MatchAndMove(TokenType.CloseBrace)) throw new ParsingError($"Unfinished statement ('}}' missing) {positionForErrorBuilder}");
            }
            else throw new ParsingError($"Invalid effect declaration {positionForErrorBuilder}");

            return list;
        }

        IStatement ActionBody()
        {
            List<IStatement> statements = new List<IStatement>();

            if (environments.Count > 1) environments.Push(new Environment(environments.Peek()));
            do
            {
                try
                {
                    if (MatchAndMove(TokenType.If)) statements.Add(If());

                    else if (MatchAndMove(TokenType.While)) statements.Add(While());

                    else if (MatchAndMove(TokenType.For)) statements.Add(For());

                    else statements.Add(SingleStatement());

                    if (tokens.Current.Type == TokenType.End)
                    {
                        throw new ParsingError($"Unfinished statement ('}}' missing) {positionForErrorBuilder}");
                    }
                }
                catch (ParsingError error)
                {
                    Errors.Add(error.Message);

                    while (!MatchAndMove(TokenType.Semicolon))
                    {
                        if (MatchAndStay(TokenType.End, TokenType.CloseBrace)) break;
                        tokens.MoveNext();
                    }
                }

            } while (!MatchAndMove(TokenType.CloseBrace));

            MatchAndMove(TokenType.Semicolon);

            if (environments.Count > 1) environments.Pop();

            return new StatementBlock(statements);
        }

        IStatement If()
        {
            IStatement stmt = null;
            (int, int) coordinates = tokens.Current.Coordinates;

            if (!MatchAndMove(TokenType.OpenParen)) throw new ParsingError($"Invalid if statement declaration ('(' missing) {positionForErrorBuilder}");
            IExpression condition = Comparison();
            if (!MatchAndMove(TokenType.CloseParen)) throw new ParsingError($"Invalid if statement declaration (')' missing) {positionForErrorBuilder}");

            if (MatchAndMove(TokenType.OpenBrace)) stmt = ActionBody();
            else stmt = SingleStatement();

            if (MatchAndMove(TokenType.Else))
            {
                if (MatchAndMove(TokenType.OpenBrace)) stmt = new If(condition, stmt, coordinates, ActionBody());
                else stmt = new If(condition, stmt, coordinates, SingleStatement());
            }
            else stmt = new If(condition, stmt, coordinates);

            return stmt;
        }

        IStatement While()
        {
            (int, int) coordinates = tokens.Current.Coordinates;

            if (!MatchAndMove(TokenType.OpenParen)) throw new ParsingError($"Invalid while statement declaration ('(' missing) {positionForErrorBuilder}");
            IExpression condition = Comparison();
            if (!MatchAndMove(TokenType.CloseParen)) throw new ParsingError($"Invalid while statement declaration (')' missing) {positionForErrorBuilder}");

            IStatement body = null;

            if (MatchAndMove(TokenType.OpenBrace)) body = ActionBody();
            else body = SingleStatement();

            return new While(condition, body, coordinates);

        }

        IStatement For()
        {
            Token item = null;

            if (MatchAndMove(TokenType.Identifier)) item = tokens.Previous;
            else throw new ParsingError($"Invalid for statement declaration (identifier missing) {positionForErrorBuilder}");

            IExpression collection = Comparison();

            IStatement body = null;
            if (MatchAndMove(TokenType.OpenBrace)) body = ActionBody();
            else body = SingleStatement();

            return new For(item, collection, environments.Peek(), body);
        }

        IStatement SingleStatement()
        {
            IStatement stmt = null;

            if (MatchAndMove(TokenType.Log))
            {
                stmt = new Log(Comparison());
            }
            else if (MatchAndMove(TokenType.Identifier))
            {
                stmt = Declaration();
            }
            else throw new ParsingError($"Empty statement {positionForErrorBuilder}");

            if (!MatchAndMove(TokenType.Semicolon))
            {
                throw new ParsingError($"Unfinished statement (';' missing) {positionForErrorBuilder}");
            }

            return stmt;
        }

        IStatement Declaration()
        {
            Token variable = tokens.Previous;

            if (MatchAndStay(TokenType.Semicolon))
            {
                return (new Declaration(variable, environments.Peek()));
            }
            else if (MatchAndMove(TokenType.Assign, TokenType.Increase, TokenType.Decrease))
            {
                return (new Declaration(variable, environments.Peek(), tokens.Previous, Comparison()));
            }

            throw new ParsingError($"Invalid declaration: {variable.Value} at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");
        }

        #region Expression Builders
        IExpression Comparison()
        {
            IExpression expr = Term();

            if (MatchAndMove(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual, TokenType.Equals, TokenType.NotEquals ))
            {
                expr = new ComparingOperation(tokens.Previous, expr, Term());
            }

            return expr;
        }

        IExpression Term()
        {
            IExpression expr = Factor();

            while (MatchAndMove(TokenType.Plus, TokenType.Minus))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Factor());
            }

            return expr;
        }

        IExpression Factor()
        {
            IExpression expr = Power();

            while (MatchAndMove(TokenType.Multiply, TokenType.Divide))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Power());
            }

            return expr;
        }

        IExpression Power()
        {
            IExpression expr = Boolean();

            while (MatchAndMove( TokenType.PowerTo))
            {
                expr = new ArithmeticOperation(tokens.Previous, expr, Boolean());
            }

            return expr;
        }

        IExpression Boolean()
        {
            IExpression expr = Unary();

            while (MatchAndMove(TokenType.And, TokenType.AndEnd, TokenType.Or, TokenType.OrEnd ))
            {
                expr = new BooleanOperation(tokens.Previous, expr, Unary());
            }

            return expr;
        }

        IExpression Unary()
        {
            while (MatchAndMove(TokenType.Minus, TokenType.Not))
            {
                return new UnaryOperation(tokens.Previous, Primary());
            }

            return Concatenation();
        }

        IExpression Concatenation()
        {
            IExpression expr = Primary();

            while (MatchAndMove(TokenType.JoinString, TokenType.JoinStringWithSpace))
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
                if (tokens.Current.Type != TokenType.CloseParen && !(expr is Predicate))
                    throw new ParsingError($"Unclosed parenthesis {positionForErrorBuilder}");
            }
            else
            {
                if (MatchAndStay (TokenType.End, TokenType.Semicolon, TokenType.Comma, TokenType.CloseParen))
                    throw new ParsingError($"Value expected {positionForErrorBuilder}");

                if (MatchAndMove(TokenType.Identifier))
                {
                    Token variable = tokens.Previous;

                    if (MatchAndMove(TokenType.IncreaseOne, TokenType.DecreaseOne)) expr = new Atom(new Declaration(variable, environments.Peek(), tokens.Previous));

                    else if (MatchAndMove(TokenType.OpenBracket))
                    {
                        expr = new Indexer(new Atom(tokens.Current), tokens.Previous.Coordinates, Comparison());

                        if (!MatchAndMove(TokenType.CloseBracket)) throw new ParsingError($"Unclosed bracket {positionForErrorBuilder}");
                    }

                    else if (MatchAndMove(TokenType.CloseParen) && MatchAndMove(TokenType.Lambda)) return Predicate(variable); //there are no methods or properties to be called on a predicate

                    else expr = new Atom(new Declaration(variable, environments.Peek()));

                    while (MatchAndMove(TokenType.Dot))
                    {
                        Token caller = null;
                        if (MatchAndMove(TokenType.Identifier)) caller = tokens.Previous;
                        else throw new ParsingError($"Value expected {positionForErrorBuilder}");

                        if (MatchAndMove(TokenType.OpenParen))
                        {
                            if (!MatchAndMove(TokenType.CloseParen))
                            {
                                List<IExpression> arguments = new List<IExpression>();

                                do arguments.Add(Comparison()); while (MatchAndMove(TokenType.Comma));
                                if (!MatchAndMove(TokenType.CloseParen)) throw new ParsingError($"Unclosed parenthesis {positionForErrorBuilder}");

                                expr = new Method(caller, expr, arguments.ToArray());
                            }
                            else expr = new Method(caller, expr);
                        }
                        else expr = new Property(caller, expr);
                    }
                }
                else { expr = new Atom(tokens.Current); tokens.MoveNext(); }
            }

            return expr;
        }

        IExpression Predicate(Token variable = null)
        {
            if(variable is null)
            {
                if (MatchAndMove(TokenType.OpenParen) && MatchAndMove(TokenType.Identifier))
                {
                    variable = tokens.Previous;
                    if (!MatchAndMove(TokenType.CloseParen)) throw new ParsingError($"Unclosed parenthesis {positionForErrorBuilder}");
                }
                else throw new ParsingError($"Invalid predicate declaration {positionForErrorBuilder}");

                if (!MatchAndMove(TokenType.Lambda)) throw new ParsingError($"Invalid predicate declaration {positionForErrorBuilder}");
            }
            return new Predicate(variable, Comparison(), new Environment(environments.Peek()));
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

        bool MatchAndMove(params TokenType[] typesToMatch)
        {
            if (typesToMatch.Contains(tokens.Current.Type))
            {
                tokens.MoveNext();
                return true;
            }
            return false;
        }

        bool MatchAndStay(params TokenType[] typesToMatch) => typesToMatch.Contains(tokens.Current.Type);

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