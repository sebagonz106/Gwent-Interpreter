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

        public List<Token> Tokenize(string input, out string[] errorMessages)
        {
            List<string> errors = new List<string>();
            errorMessages = new string[0];
            List<Token> tokens = new List<Token>();
            int line = 0;
            int column = 0;
            
            input.Trim();
            string[] inputLines = input.Split('\n');
            bool quotationMarksOpened = false;
            (int, int) lastQuotationCoordinates = (0, 0);

            string currentToken = "";
            while (line<inputLines.Length)
            {
                string currentLine = inputLines[line];

                while(column<currentLine.Length)
                {
                    if (currentLine[column] == '$' && !quotationMarksOpened && (line!=inputLines.Length-1 && column != currentLine.Length - 1))
                    {
                        errors.Add("Invalid char \'$\' at " + line + ":" + column);
                        currentToken = "";
                        column++;
                        continue;
                    }
                    try
                    {
                        if (currentLine[column] == '/' && currentLine[column+1] == '/') break;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        errors.Add("Invalid char \'/\' at " + line + ":" + column);
                        break;
                    }

                    if (currentLine[column] == '"')
                    {
                        currentToken += '"';
                        if (quotationMarksOpened)
                        {
                            tokens.Add(new Token(currentToken, TokenType.String, line + 1, column + 1));
                            currentToken = "";
                        }
                        quotationMarksOpened = !quotationMarksOpened;
                        lastQuotationCoordinates = (line + 1, column + 1);
                    }
                    else if (quotationMarksOpened)
                    {
                        if(currentLine[column] == '\\')
                        {
                            try
                            {
                                switch (currentLine[++column])
                                {
                                    case 'n':
                                        currentToken += '\n';
                                        break;
                                    case 't':
                                        currentToken += '\t';
                                        break;
                                    case '\'':
                                        currentToken += '\'';
                                        break;
                                    case '"':
                                        currentToken += '\"';
                                        break;
                                    case '\\':
                                        currentToken += '\\';
                                        break;
                                    default:
                                        errors.Add("Invalid scape sequence at " + line + ":" + column);
                                        break;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                errors.Add("Invalid char \'\\\' at " + line + ":" + column);
                                break;
                            }
                        } //scape sequences
                        else currentToken += currentLine[column];
                    }
                    else if (currentLine[column] == ' ') { }
                    else
                    {
                        if (stringPattern.IsMatch(currentLine[column].ToString()))
                            AddMatch(tokens, stringPattern.Match(currentLine, column).Value, line, ref column);

                        else if (numPattern.IsMatch(currentLine[column].ToString()))
                            AddMatch(tokens, numPattern.Match(currentLine, column).Value, line, ref column, TokenType.Number);

                        else if (symbolPattern.IsMatch(currentLine[column].ToString()))
                            AddMatch(tokens, symbolPattern.Match(currentLine, column).Value, line, ref column);

                        else errors.Add($"Invalid char \'{currentLine[column]}\' at {line}:{column}");
                    }
                    column++;
                }
                column = 0;
                line++;
            }

            if(quotationMarksOpened) errors.Add($"Unclosed quotation marks opened at {lastQuotationCoordinates.Item1}:{lastQuotationCoordinates.Item2}");
            
            if (errors.Count > 0)
            {
                errorMessages = errors.ToArray();
                return new List<Token>();
            }

            tokens.Add(new Token("$", TokenType.End, line, column));
            return tokens;
        }

        private void AddMatch(List<Token> tokens, string match, int line, ref int column, TokenType specificTypeToUse = TokenType.End)
        {
            try
            {
                tokens.Add(new Token(match, (specificTypeToUse == TokenType.End? Token.TypeByValue[match] : specificTypeToUse), line + 1, column + 1));
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                tokens.Add(new Token(match, TokenType.Identifier, line + 1, column + 1));
                Token.TypeByValue.Add(match, TokenType.Identifier);
            }
            column += match.Length-1;
        }
    }
}