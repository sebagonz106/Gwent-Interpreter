using System.Collections.Generic;

namespace Gwent_Interpreter
{
    class Token
    {
        public string Value { get; private set; }
        public TokenType Type { get; private set; }
        public (int,int) Coordinates { get; private set; }

        public Token(string value, TokenType type, int line, int column)
        {
            Value = value;
            Type = type;
            Coordinates = (line, column);
        }

        public Token(object value, Token variable)
        {
            Value = value.ToString();
            Type = variable.Type;
            Coordinates = (variable.Coordinates.Item1, variable.Coordinates.Item2);
        }

        public override string ToString() => $"{Value}, {Type}, {Coordinates.Item1}, {Coordinates.Item2}{(this.Type==TokenType.String? "" : $"-{Coordinates.Item2 + Value.Length-1}")}";

        public static Dictionary<string, TokenType> TypeByValue = new Dictionary<string, TokenType>
        {
            //Native DLS keywords:
            {"card",  TokenType.Card}, {"effect",  TokenType.EffectDeclaration}, {"Effect",  TokenType.EffectParam}, {"Amount",  TokenType.Amount}, {"Name",  TokenType.Name}, {"Params",  TokenType.Params},
            {"Action",  TokenType.Action}, {"Type",  TokenType.Type}, {"Faction",  TokenType.Faction}, {"Power",  TokenType.Power}, {"Range",  TokenType.Range},
            {"OnActivation",  TokenType.OnActivation}, {"Selector",  TokenType.Selector}, {"Source",  TokenType.Source}, {"Single",  TokenType.Single},
            {"Predicate",  TokenType.Predicate}, {"PostAction",  TokenType.PostAction},
            //Common expressions:
            { "if",  TokenType.If}, {"else",  TokenType.Else}, {"for",  TokenType.For}, {"while",  TokenType.While}, {"=>",  TokenType.Lambda}, {"$", TokenType.End}, { "log", TokenType.Log},
            //Separation symbols:
            {",",  TokenType.Comma}, {".",  TokenType.Dot}, {";",  TokenType.Semicolon}, {":",  TokenType.DoubleDot},
            { "(",  TokenType.OpenParen}, {")",  TokenType.CloseParen},
            { "{",  TokenType.OpenBrace}, {"}",  TokenType.CloseBrace},
            { "[",  TokenType.OpenBracket}, {"]",  TokenType.CloseBracket},
            //Arithmetic expressions:
            {"+",  TokenType.Plus}, {"-",  TokenType.Minus}, {"*",  TokenType.Multiply}, {"/",  TokenType.Divide}, {"^",  TokenType.PowerTo},
            { "=", TokenType.Assign}, {"++",  TokenType.IncreaseOne}, {"+=",  TokenType.Increase}, {"--",  TokenType.DecreaseOne},
            { "-=",  TokenType.Decrease}, {"%",  TokenType.Module},{"@",  TokenType.JoinString}, {"@@",  TokenType.JoinStringWithSpace},
            //Boolean expressions:
            {"&",  TokenType.And}, {"&&",  TokenType.AndEnd}, {"|",  TokenType.Or}, {"||",  TokenType.OrEnd},
            { "!",  TokenType.Not}, {"true",  TokenType.True}, {"false",  TokenType.False},
            //Comparation expressions:
            {"==",  TokenType.Equals}, {"!=",  TokenType.NotEquals}, {"<",  TokenType.Less}, {"<=",  TokenType.LessEqual}, {">",  TokenType.Greater}, {">=",  TokenType.GreaterEqual}
        };
    }

    public enum TokenType
    {
        //Native DLS keywords:
        Card, EffectDeclaration, EffectParam, Name, Params, Amount, Action, Type, Faction, Power, Range, OnActivation, Selector, Source, Single, Predicate, PostAction,
        //Common expressions:
        Number, String, If, Else, For, While, Lambda, End, Log,
        //Separation symbols:
        Comma, Dot, Semicolon, DoubleDot, OpenParen, CloseParen, OpenBrace, CloseBrace, OpenBracket, CloseBracket,
        //Arithmetic expressions:
        Plus, Minus, Multiply, Divide, PowerTo, Assign, IncreaseOne, Increase, DecreaseOne, Decrease, Module, JoinString, JoinStringWithSpace,
        //Boolean expressions:
        And, AndEnd, Or, OrEnd, Not, True, False,
        //Comparation expressions:
        Equals, NotEquals, Less, LessEqual, Greater, GreaterEqual,

        Identifier
    }
}
