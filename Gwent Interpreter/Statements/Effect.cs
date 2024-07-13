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

        public EffectStatement(IExpression name, List<(Token,IExpression)> paramsAndType, IStatement body, Environment environment, (int,int) coordinates)
        {
            this.coordinates = coordinates;
            this.name = name;
            this.action = body;
            this.environment = environment;
            this._params = new Dictionary<string, ReturnType>();

            foreach (var pair in paramsAndType)
            {
                ReturnType temp = ReturnType.Object;
                string type = "";

                try
                {
                    type = (string)pair.Item2.Evaluate();
                }
                catch (InvalidCastException)
                {
                    throw new ParsingError($"Invalid return type expression received in param at {pair.Item1.Coordinates} (return types include: \"String\", \"Num\", \"Bool\", \"Object\")");
                }

                switch (type)
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
            string warnings = "";
            string errors = "";

            foreach (var pair in _params)
            {
                try
                {
                    if (this._params[pair.Item1.Value] is ReturnType.Object) warnings += ($"You must make sure expression at {pair.Item1.Coordinates} has a valid return type or a run time error may occur\n");
                    else if (pair.Item2.Return != this._params[pair.Item1.Value]) errors += ($"Invalid expression received in param at {pair.Item1.Coordinates}\n");
                }
                catch (KeyNotFoundException)
                {
                    errors += ($"Invalid param received at {pair.Item1.Coordinates}");
                }

                environment.Set(pair.Item1, pair.Item2);
            }
            if (errors != "") throw new EvaluationError(errors);
            receivedTargetsAndParams = true;
            if (warnings != "") throw new Warning(warnings);
        }

        public bool CheckSemantic(out List<string> errors)
        {
            action.CheckSemantic(out errors);

            if (name.Return == ReturnType.String)
            {
                effects.Add((string)name.Evaluate(), this);
                if(!name.CheckSemantic(out string error)) errors.Add(error);
            }
            else errors.Add($"Not a string at name in effect declaration at {coordinates.Item1}:{coordinates.Item2}");

            return errors.Count == 0;
        }

        public void Execute()
        {
            if (!receivedTargetsAndParams) throw new EvaluationError($"Trying to run \"{name}\" effect whitout assigning parameters properly");
            else action.Execute();
        }
    }
}
