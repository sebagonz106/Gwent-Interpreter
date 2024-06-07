using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Gwent_Interpreter
{
    class Lexer
    {
        Regex stringPattern = new Regex(@"[_a-zA-Z]+[_a-zA-Z0-9]*");
        Regex numPattern = new Regex(@"\d+(\.\d+)?");
        Regex symbolPattern = new Regex(@"=(>)?|[<>](=)?|@(@)?|\+([\+=])?|-([-=])?|[.,:;*/^%!]|&(&)?|\|(\|)?|[\{\}\[\]\(\)]");

        public List<Token> Tokenize(string input)
        {
            List<Token> tokens = new List<Token>();
            int line = 0;
            int column = 0;

            input.Trim();
            string[] inputLines = input.Split("\n");
            string currentLine = "";
            int comillas = 0;
            string currentToken = "";

            while (line<inputLines.Length)
            {
                currentLine = inputLines[line];

                while(column<currentLine.Length)
                {
                    if(currentLine[column] == ' ')
                    {
                        column++;
                        continue;
                    }
                    if (currentLine[column] == '"')
                    {
                        comillas++;
                        currentToken += '"';
                        if(comillas%2==0)
                        {
                            tokens.Add(new Token(currentToken, TokenType.String, line, column - currentToken.Length));
                            currentToken = "";
                        }
                    }
                    else if (comillas % 2 == 1)
                    {
                        currentToken += currentLine[column];
                    }
                    else
                    {
                        if (stringPattern.IsMatch($"{currentLine[column]}"))
                        {
                            currentToken = stringPattern.Match(currentLine, column).Value;
                            try
                            {
                                tokens.Add(new Token(currentToken, Token.TypeByValue[currentToken], line, column)); 
                            }
                            catch (System.Collections.Generic.KeyNotFoundException)
                            {
                                tokens.Add(new Token(currentToken, TokenType.Identifier, line, column));
                            }

                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                        else if (numPattern.IsMatch($"{currentLine[column]}"))
                        {
                            currentToken = numPattern.Match(currentLine, column).Value;
                            tokens.Add(new Token(currentToken, TokenType.Number, line, column));
                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                        else
                        {
                            currentToken = symbolPattern.Match(currentLine, column).Value;
                            try
                            {
                                tokens.Add(new Token(currentToken, Token.TypeByValue[currentToken], line, column));
                            }
                            catch (System.Collections.Generic.KeyNotFoundException)
                            {
                                throw new System.NotImplementedException();
                            }
                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                    }

                    column++;
                }
                line++;
            }

            tokens.Add(new Token("$", TokenType.End, line, column));
            return tokens;
        }
    }
}