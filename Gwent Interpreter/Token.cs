﻿using System;
using System.Collections.Generic;
using System.Text;

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

        public override string ToString() => $"{Value}, {Type}, {Coordinates.Item1}, {Coordinates.Item2}";

        public static Dictionary<string, TokenType> TypeByValue = new Dictionary<string, TokenType>
        {
            //Native DLS keywords:
            {"card",  TokenType.Card}, {"effect",  TokenType.Effect}, {"Amount",  TokenType.Amount}, {"Name",  TokenType.Name}, {"Params",  TokenType.Params},
            {"Action",  TokenType.Action}, {"Type",  TokenType.Type}, {"Faction",  TokenType.Faction}, {"Power",  TokenType.Power}, {"Range",  TokenType.Range},
            {"OnActivation",  TokenType.OnActivation}, {"Selector",  TokenType.Selector}, {"Source",  TokenType.Source}, {"Single",  TokenType.Single},
            {"Predicate",  TokenType.Predicate}, {"PostAction",  TokenType.PostAction},
            //Common expressions:
            { "if",  TokenType.If}, {"else",  TokenType.Else}, {"for",  TokenType.For}, {"while",  TokenType.While}, {"=>",  TokenType.Lambda}, {"$", TokenType.End},
            //Separation symbols:
            {",",  TokenType.Colon}, {".",  TokenType.Dot}, {";",  TokenType.Semicolon}, {":",  TokenType.DoubleDot},
            { "(",  TokenType.ParentesisAbierto}, {")",  TokenType.ParentesisCerrado},
            { "{",  TokenType.LlaveAbierta}, {"}",  TokenType.LlaveCerrada},
            { "[",  TokenType.CorcheteAbierto}, {"]",  TokenType.CorcheteCerrado},
            //Arithmetic expressions:
            {"+",  TokenType.Plus}, {"-",  TokenType.Minus}, {"*",  TokenType.Multiply}, {"/",  TokenType.Divide}, {"^",  TokenType.PowerTo},
            { "=", TokenType.Asign}, {"++",  TokenType.IncreaseOne}, {"+=",  TokenType.Increase}, {"--",  TokenType.DecreaseOne},
            { "-=",  TokenType.Decrease}, {"%",  TokenType.Module},{"@",  TokenType.JoinString}, {"@@",  TokenType.JoinStringWithSpace},
            //Boolean expressions:
            {"&",  TokenType.And}, {"&&",  TokenType.AndEnd}, {"|",  TokenType.Or}, {"||",  TokenType.OrEnd},
            { "!",  TokenType.Not}, {"true",  TokenType.True}, {"false",  TokenType.False},
            //Comparation expressions:
            {"==",  TokenType.Equals}, {"<",  TokenType.Under}, {"<=",  TokenType.UnderEqual}, {">",  TokenType.Over}, {">=",  TokenType.OverEqual}
        };
    }

    public enum TokenType
    {
        //Native DLS keywords:
        Card, Effect, Name, Params, Amount, Action, Type, Faction, Power, Range, OnActivation, Selector, Source, Single, Predicate, PostAction,
        //Common expressions:
        Number, String, If, Else, For, While, Lambda, End,
        //Separation symbols:
        Colon, Dot, Semicolon, DoubleDot, ParentesisAbierto, ParentesisCerrado, LlaveAbierta, LlaveCerrada, CorcheteAbierto, CorcheteCerrado,
        //Arithmetic expressions:
        Plus, Minus, Multiply, Divide, PowerTo, Asign, IncreaseOne, Increase, DecreaseOne, Decrease, Module, JoinString, JoinStringWithSpace,
        //Boolean expressions:
        And, AndEnd, Or, OrEnd, Not, True, False,
        //Comparation expressions:
        Equals, Under, UnderEqual, Over, OverEqual,

        Identifier
    }
}
