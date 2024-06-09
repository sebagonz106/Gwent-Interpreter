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
        Regex symbolPattern = new Regex(@"=([=>])?|[<>](=)?|@(@)?|\+([\+=])?|-([-=])?|\!(=)?|[.,:;*/^%]|&(&)?|\|(\|)?|[\{\}\[\]\(\)]");

        public List<Token> Tokenize(string input, out string errorMessage)
        {
            errorMessage = "";
            List<Token> tokens = new List<Token>();
            int line = 0;
            int column = 0;
            
            input.Trim();
            string[] inputLines = input.Split("\n");
            string currentLine = "";
            bool quotationMarksOpened = false;
            (int, int) lastQuotationCoordinates = (0, 0);

            string currentToken = "";

            while (line<inputLines.Length)
            {
                currentLine = inputLines[line];

                while(column<currentLine.Length)
                {
                    if (currentLine[column] == '$')
                    {
                        try
                        {
                            char c = currentLine[column + 1];
                            errorMessage = "Invalid char \'$\' at " + line + ":" + column;
                            return new List<Token>();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (currentLine[column] == '"')
                    {
                        quotationMarksOpened=!quotationMarksOpened;
                        currentToken += '"';
                        lastQuotationCoordinates = (line + 1, column + 1);
                        if (!quotationMarksOpened)
                        {
                            tokens.Add(new Token(currentToken, TokenType.String, line+1, column + 1));
                            currentToken = "";
                        }
                    }
                    else if (quotationMarksOpened)
                    {
                        currentToken += currentLine[column];
                    }
                    else if (currentLine[column] == ' ')
                    {
                        column++;
                        continue;
                    }
                    else
                    {
                        if (stringPattern.IsMatch(currentLine[column].ToString()))
                        {
                            currentToken = stringPattern.Match(currentLine, column).Value;
                            try
                            {
                                tokens.Add(new Token(currentToken, Token.TypeByValue[currentToken], line + 1, column + 1));
                            }
                            catch (System.Collections.Generic.KeyNotFoundException)
                            {
                                tokens.Add(new Token(currentToken, TokenType.Identifier, line + 1, column + 1));
                                Token.TypeByValue.Add(currentToken, TokenType.Identifier);
                            }

                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                        else if (numPattern.IsMatch(currentLine[column].ToString()))
                        {
                            currentToken = numPattern.Match(currentLine, column).Value;
                            tokens.Add(new Token(currentToken, TokenType.Number, line + 1, column + 1));
                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                        else
                        {
                            currentToken = symbolPattern.Match(currentLine, column).Value;
                            try
                            {
                                tokens.Add(new Token(currentToken, Token.TypeByValue[currentToken], line + 1, column + 1));
                            }
                            catch (System.Collections.Generic.KeyNotFoundException)
                            {
                                errorMessage = $"Invalid expression at {line}:{column}";
                                return new List<Token>();
                            }
                            column += currentToken.Length;
                            currentToken = "";
                            continue;
                        }
                    }

                    column++;
                }

                column = 0;
                line++;
            }
            if(quotationMarksOpened)
            {
                errorMessage = $"Unclosed quotation marks opened at {lastQuotationCoordinates.Item1}:{lastQuotationCoordinates.Item2}";
                return new List<Token>();
            }
            tokens.Add(new Token("$", TokenType.End, line, column));
            return tokens;
        }
    }
}