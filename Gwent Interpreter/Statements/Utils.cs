using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

namespace Gwent_Interpreter
{
    interface IStatement
    {
        void Execute();
    }

    class Environment
    {
        Dictionary<string, IExpression> usedVariables = new Dictionary<string, IExpression>();
        Environment parent;
        static Environment global;
        public static Environment Global
        {
            get
            {
                if (global == null)
                {
                    global = new Environment();
                    global.parent = null;
                    global.usedVariables.Add("context", GwentInterpreterContext.Context);
                }

                return global;
            }
        }
        public Environment(Environment parent = null)
        {
            if (parent != null)
            {
                this.parent = parent;
            }
        }

        public void ResetGlobal() { global = new Environment(); }
        public void Set(Token variable, IExpression value)
        {
            if (variable.Value == "context") throw new ParsingError($"'context' is a reserved keyword, change it at {variable.Coordinates.Item1}:{variable.Coordinates.Item2}");

            if (usedVariables.ContainsKey(variable.Value))
            {
                this.usedVariables[variable.Value] = value ?? throw new ParsingError($"Already declared variable at {variable.Coordinates.Item1}:{variable.Coordinates.Item2 + variable.Value.Length}");
            }
            else if (SearchInParents(variable.Value)) ModifyInParents(variable.Value, value);
            else this.usedVariables.Add(variable.Value, value);
        }
        public IExpression this[string name]
        {
            get
            {
                try
                {
                    if (usedVariables.ContainsKey(name)) return usedVariables[name];
                    else if (SearchInParents(name)) return GetFromParents(name);
                    else throw new KeyNotFoundException();
                }
                catch (KeyNotFoundException)
                {
                    throw new ParsingError($"Undeclared variable '{name}' used");
                }
            }
            private set
            {
                try
                {
                    if (usedVariables.ContainsKey(name)) usedVariables[name] = value;
                    else if (SearchInParents(name)) ModifyInParents(name, value);
                    else throw new KeyNotFoundException();
                }
                catch (KeyNotFoundException)
                {
                    throw new ParsingError($"Undeclared variable '{name}' used");
                }
            }
        }
        bool SearchInParents(string key) => parent is null ? false : parent.usedVariables.ContainsKey(key) ? true : parent.SearchInParents(key);
        void ModifyInParents(string key, IExpression value)
        {
            if (parent.usedVariables.ContainsKey(key)) parent.usedVariables[key] = value;
            else parent.ModifyInParents(key, value);
        }
        IExpression GetFromParents(string key)
        {
            if (parent.usedVariables.ContainsKey(key)) return parent.usedVariables[key];
            else return parent.GetFromParents(key);
        }
    }
}
