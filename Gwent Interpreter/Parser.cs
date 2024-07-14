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

        public IStatement TestParse() => ActionBody();

        public Input Parse()
        {
            List<IStatement> cards = new List<IStatement>();
            List<IStatement> effects = new List<IStatement>();

            while (!MatchAndStay(TokenType.End))
            {
                try
                {
                    if (MatchAndMove(TokenType.EffectDeclaration) && MatchAndMove(TokenType.OpenBrace))
                    {
                        effects.Add(EffectDeclaration());
                        if (!MatchAndMove(TokenType.CloseBrace)) throw new ParsingError($"Unfinished statement ('}}' missing) {positionForErrorBuilder}");
                    }
                    else if (MatchAndMove(TokenType.Card) && MatchAndMove(TokenType.OpenBrace))
                    {
                        cards.Add(CardDeclaration());
                        if (!MatchAndMove(TokenType.CloseBrace)) throw new ParsingError($"Unfinished statement ('}}' missing) {positionForErrorBuilder}");
                    }
                    else throw new ParsingError($"Invalid declaration {positionForErrorBuilder}");
                }
                catch(ParsingError error)
                {
                    if (PanicMode(error.Message)) break;
                }
            }

            return new Input(cards, effects);
        }

        #region Effect
        public IStatement EffectDeclaration()
        {
            if (MatchAndStay(TokenType.CloseBrace)) throw new ParsingError("Empty effect" + positionForErrorBuilder);

            environments.Push(new Environment(environments.Peek()));
            IExpression name = null;
            List<(Token, Token)> paramsAndType = new List<(Token, Token)>();
            IStatement body = null;
            (int, int) coordinates = tokens.Current.Coordinates;
            Token targets = null;
            Token context = null;

            do
            {
                try
                {
                    if (MatchAndStay(TokenType.End)) throw new ParsingError($"Unfinished statement { positionForErrorBuilder} ('}}' missing)");

                    else if (MatchAndMove(TokenType.Name))
                    {
                        if (!(name is null)) throw new ParsingError("A name has already been declared" + positionForErrorBuilder);

                        if (MatchAndMove(TokenType.DoubleDot)) name = Comparison();
                        else throw new ParsingError("Invalid Name declaration" + positionForErrorBuilder + " (':' missing)");

                        if (!MatchAndMove(TokenType.Comma) && tokens.TryLookAhead.Type != TokenType.CloseBrace) throw new ParsingError("Invalid Name declaration" + positionForErrorBuilder + " (',' missing)");
                    }

                    else if (MatchAndMove(TokenType.Params)) //params can be declared in separated blocks
                    {
                        if (MatchAndMove(TokenType.DoubleDot))
                        {
                            if (MatchAndMove(TokenType.OpenBrace))
                            {
                                while (MatchAndStay(TokenType.Identifier))
                                {
                                    paramsAndType.Add(Param());
                                    MatchAndMove(TokenType.Comma); // space or change of line will be taken as the end of a param declaration, a comma is not requierd
                                }
                                if (!MatchAndMove(TokenType.CloseBrace)) throw new ParsingError("Invalid Params declaration" + positionForErrorBuilder + " ('}' expected)");
                            }
                            else if (MatchAndStay(TokenType.Identifier))
                            {
                                paramsAndType.Add(Param());
                            }
                            else throw new ParsingError("Invalid Params declaration" + positionForErrorBuilder + " ('{' missing)");
                        }
                        else throw new ParsingError("Invalid Params declaration" + positionForErrorBuilder + " (':' missing)");

                        if (!MatchAndMove(TokenType.Comma) && tokens.TryLookAhead.Type != TokenType.CloseBrace) throw new ParsingError("Invalid Params declaration" + positionForErrorBuilder + " (',' missing)");
                    }

                    else if (MatchAndMove(TokenType.Action))
                    {
                        if (!(body is null)) throw new ParsingError("An action has already been declared" + positionForErrorBuilder);

                        if (MatchAndMove(TokenType.DoubleDot))
                        {
                            if (MatchAndMove(TokenType.OpenParen))
                            {
                                if (MatchAndMove(TokenType.Identifier)) targets = tokens.Previous;
                                else throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (targets identifier expected)");

                                if (!MatchAndMove(TokenType.Comma)) throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (',' expected between targets and context idenfiers)");

                                if (MatchAndMove(TokenType.Identifier)) context = tokens.Previous;
                                else throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (context identifier expected)");

                                if (!MatchAndMove(TokenType.CloseParen)) throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (')' expected after context identifier)");
                            }

                            if (!MatchAndMove(TokenType.Lambda)) throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (=>)' expected after targets and context idenfiers)");

                            if (MatchAndMove(TokenType.OpenBrace)) body = ActionBody();
                            else body = SingleStatement();

                            if (body is null) throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (body expected)");
                        }
                        else throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (':' missing)");

                        if (!MatchAndMove(TokenType.Comma) && tokens.TryLookAhead.Type != TokenType.CloseBrace) throw new ParsingError("Invalid Action declaration" + positionForErrorBuilder + " (',' expected)");
                    }

                    else throw new ParsingError("Invalid effect declaration ('Name', 'Params' or 'Action' expected)" + positionForErrorBuilder);
                }
                catch (ParsingError error)
                {
                    if (PanicMode(error.Message, TokenType.Comma)) break;
                }
            } while (MatchAndMove(TokenType.CloseBrace));

            if (name is null) throw new ParsingError("Invalid effect declaration (A name must be declared)");
            if (body is null) throw new ParsingError("Invalid effect declaration (An action must be declared)");

            return new EffectStatement(name, paramsAndType, body, environments.Pop(), coordinates, targets, context);
        }

        (Token, Token) Param()
        {
            Token _param = null;
            Token type = null;

            if (MatchAndMove(TokenType.Identifier)) _param = tokens.Previous;
            else throw new ParsingError("Invalid parameter declaration" + positionForErrorBuilder + " (parameter identifier expected)");

            if (!MatchAndMove(TokenType.DoubleDot)) throw new ParsingError("Invalid parameter declaration" + positionForErrorBuilder + " (':' expected after parameter name)");

            if (MatchAndMove(TokenType.Identifier)) type = tokens.Previous;
            else throw new ParsingError("Invalid parameter declaration" + positionForErrorBuilder + " (parameter's type expected)");

            return (_param, type);
        }

        #endregion

        #region Card
        public IStatement CardDeclaration()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Action Body
        IStatement ActionBody()
        {
            List<IStatement> statements = new List<IStatement>();

            if (environments.Count > 1) environments.Push(new Environment(environments.Peek()));
            do
            {
                try
                {
                    if (MatchAndStay(TokenType.End))  throw new ParsingError($"Unfinished statement ('}}' missing) {positionForErrorBuilder}");

                    else if (MatchAndMove(TokenType.If)) statements.Add(If());

                    else if (MatchAndMove(TokenType.While)) statements.Add(While());

                    else if (MatchAndMove(TokenType.For)) statements.Add(For());

                    else statements.Add(SingleStatement());
                }
                catch (ParsingError error)
                {
                    if (PanicMode(error.Message, TokenType.Semicolon)) break;
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
        #endregion

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

                    if (MatchAndMove(TokenType.IncreaseOne, TokenType.DecreaseOne)) expr = new DeclarationAtom(new Declaration(variable, environments.Peek(), tokens.Previous));

                    else if (MatchAndMove(TokenType.OpenBracket))
                    {
                        expr = new Indexer(new ValueAtom(tokens.Current), tokens.Previous.Coordinates, Comparison());

                        if (!MatchAndMove(TokenType.CloseBracket)) throw new ParsingError($"Unclosed bracket {positionForErrorBuilder}");
                    }

                    else if (MatchAndMove(TokenType.CloseParen) && MatchAndMove(TokenType.Lambda)) return Predicate(variable); //there are no methods or properties to be called on a predicate

                    else expr = new DeclarationAtom(new Declaration(variable, environments.Peek()));
                }
                else { expr = new ValueAtom(tokens.Current); tokens.MoveNext(); }

                while (MatchAndMove(TokenType.Dot)) //checking if property or method call
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
            environments.Push(new Environment(environments.Peek()));
            IExpression condition = Comparison();
            return new Predicate(variable, condition, environments.Pop());
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

        bool PanicMode(string error, TokenType breaker = TokenType.CloseBrace)
        {
            Errors.Add(error);

            while (!MatchAndMove(breaker))
            {
                if (MatchAndStay(TokenType.End, TokenType.CloseBrace)) return true;
                else tokens.MoveNext();
            }

            return false;
        }
        #endregion
    }
}