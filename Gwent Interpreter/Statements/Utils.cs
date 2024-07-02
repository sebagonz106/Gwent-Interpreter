using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter
{
    interface IStatement
    {
        void Execute();
    }

    class Environment
    {
        Dictionary<string, IExpression> usedVariables = new Dictionary<string, IExpression>();
        static Environment global;
        public static Environment Global
        {
            get
            {
                if (global == null)
                {
                    global = new Environment();
                }

                return global;
            }
        }
        public Environment(Environment parent = null)
        {
            if (parent != null)
            {
                foreach (var pair in parent.usedVariables)
                {
                    this.usedVariables.Add(pair.Key, pair.Value);
                }
            }
        }

        public void ResetGlobal() { global = new Environment(); }
        public void Set(Token variable, IExpression value)
        {
            if (usedVariables.ContainsKey(variable.Value))
            {
                this.usedVariables[variable.Value] = value ?? throw new ParsingError($"Already declared variable at {variable.Coordinates.Item1}:{variable.Coordinates.Item2 + variable.Value.Length}");
            }
            else this.usedVariables.Add(variable.Value, value);
        }
        public IExpression this[string name]
        {
            get
            {
                try
                {
                    return usedVariables[name];
                }
                catch (KeyNotFoundException)
                {
                    throw new ParsingError($"Undeclared variable '{name}' used");
                }
            }
        }
    }
}
