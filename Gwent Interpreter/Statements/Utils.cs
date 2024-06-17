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
        public Environment(List<Environment> parents = null)
        {
            if (parents != null)
            {
                foreach (var item in parents)
                {
                    foreach (var key in item.usedVariables.Keys)
                    {
                        this.usedVariables.Add(key, item[key]);
                    }
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
        public IExpression this[string name] => usedVariables[name];
    }
}
