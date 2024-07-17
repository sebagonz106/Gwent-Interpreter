using System;
using System.Collections.Generic;
using System.Text;
using Gwent_Interpreter.GameLogic;

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
        Token targets = new Token("targets", TokenType.Identifier, 0, 0);
        Token context = new Token("context", TokenType.Identifier, 0, 0);
        static Dictionary<string, EffectStatement> effects = new Dictionary<string, EffectStatement>();

        public static Dictionary<string, EffectStatement> Effects => effects;

        public (int, int) Coordinates => coordinates;

        public EffectStatement(IExpression name, List<(Token,Token)> paramsAndType, IStatement body, Environment environment, (int,int) coordinates, Token targets = null, Token context = null)
        {
            this.coordinates = coordinates;
            this.name = name;
            this.action = body;
            this.environment = environment;
            this._params = new Dictionary<string, ReturnType>();
            this.targets = targets is null? this.targets : targets;
            this.context = context is null ? this.context : context;

            foreach (var pair in paramsAndType)
            {
                ReturnType temp = ReturnType.Object;
                string type = pair.Item2.Value.ToString();

                switch (type)
                {
                    case "String":
                        temp = ReturnType.String;
                        break;
                    case "Number":
                        temp = ReturnType.Num;
                        break;
                    case "Bool":
                        temp = ReturnType.Bool;
                        break;
                    case "Card":
                        temp = ReturnType.Card;
                        break;
                    case "List":
                        temp = ReturnType.List;
                        break;
                    case "Object":
                        break;
                    default:
                        throw new ParsingError($"Invalid return type expression received in param at {pair.Item1.Coordinates} (return types include: \"String\", \"Num\", \"Bool\", \"Object\")");
                }

                _params.Add(pair.Item1.Value, temp);

                environment.Set(pair.Item1, null);
            }
        }

        public void Receive(List<(Token, IExpression)> _params, IExpression targets)
        {
            if(this.context.Value!="context") environment.Set(this.context, GwentInterpreterContext.Context);
            environment.Set(this.targets, targets);
            string warnings = "";
            string errors = "";

            foreach (var pair in _params)
            {
                try
                {
                    if (this._params[pair.Item1.Value] is ReturnType.Object) warnings += ($"You must make sure expression at {pair.Item1.Coordinates} has a valid return type or a run time error may occur\n");
                    else if (!(pair.Item2.Return is ReturnType.Object || pair.Item2.Return == this._params[pair.Item1.Value])) errors += ($"Invalid expression received in param at {pair.Item1.Coordinates}\n");
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
            errors = new List<string>();

            if (name.Return == ReturnType.String)
            {
                if (!this.name.CheckSemantic(out string error)) errors.Add(error);
                string name = (string)this.name.Evaluate();

                if (effects.ContainsKey(name)) errors.Add($"An effect with the same name as the one at {coordinates.Item1}:{coordinates.Item2} has already been declared");
                else effects.Add(name, this);
            }
            else errors.Add($"Not a string at name in effect declaration at {coordinates.Item1}:{coordinates.Item2}");

            if (!action.CheckSemantic(out List<string> temp)) errors.AddRange(temp);
            return errors.Count == 0;
        }

        public void Execute()
        {
            if (!receivedTargetsAndParams) throw new EvaluationError($"Trying to run \"{name}\" effect whitout assigning parameters properly");
            else action.Execute();
        }
    }
}
