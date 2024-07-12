using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class EffectStatement : IStatement
    {
        (int, int) coordinates;
        IExpression name;
        Dictionary<string, ReturnType> _params;
        IStatement action;
        Environment environment;
        bool receivedTargetsAndParams = false;
        static Token targets = new Token("targets", TokenType.Identifier, 0, 0);
        static Dictionary<string, EffectStatement> effects = new Dictionary<string, EffectStatement>();

        public static Dictionary<string, EffectStatement> Effects => effects;

        public EffectStatement(IExpression name, List<(Token,Token)> paramsAndType, IStatement body, Environment environment, (int,int) coordinates)
        {
            this.coordinates = coordinates;
            this.name = name;
            this.action = body;
            this.environment = environment;
            this._params = new Dictionary<string, ReturnType>();

            ReturnType temp = ReturnType.Object;

            foreach (var pair in paramsAndType)
            {
                switch (pair.Item2.Value)
                {
                    case "String":
                        temp = ReturnType.String;
                        break;
                    case "Num":
                        temp = ReturnType.Num;
                        break;
                    case "Bool":
                        temp = ReturnType.Bool;
                        break;
                    default:
                        temp = ReturnType.Object;
                        break;
                }
                _params.Add(pair.Item1.Value, temp);
                environment.Set(pair.Item1, null);
            }
        }

        public void Receive(List<(Token, IExpression)> _params, IExpression targets)
        {
            environment.Set(EffectStatement.targets, targets);

            foreach (var pair in _params)
            {
                try
                {
                    if (pair.Item2.Return != this._params[pair.Item1.Value]) throw new EvaluationError($"Invalid expression received in param at {pair.Item1.Coordinates}");
                }
                catch (KeyNotFoundException)
                {
                    throw new EvaluationError($"Invalid param received at {pair.Item1.Coordinates}");
                }

                environment.Set(pair.Item1, pair.Item2);
                receivedTargetsAndParams = true;
            }
        }

        public bool CheckSemantic(out List<string> errors)
        {
            action.CheckSemantic(out errors);

            if (name.Return == ReturnType.String)
            {
                effects.Add((string)name.Evaluate(), this);
                if(!name.CheckSemantic(out string error)) errors.Add(error);
            }
            else errors.Add($"not a string at name in effect declaration at {coordinates}");

            return errors.Count == 0;
        }

        public void Execute()
        {
            if (!receivedTargetsAndParams) throw new EvaluationError($"trying to run an unassigned effect at {coordinates}");
            else action.Execute();
        }
    }
}
