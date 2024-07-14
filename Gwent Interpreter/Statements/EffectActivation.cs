using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class EffectActivation : IStatement
    {
        (int, int) coordinates;
        IExpression effectName;
        List<(Token, IExpression)> _params;
        IExpression selector;
        EffectStatement effectReference;

        public EffectActivation(IExpression effectName, List<(Token, IExpression)> @params, IExpression selector, (int,int) coordinates)
        {
            this.coordinates = coordinates;
            this.effectName = effectName;
            _params = @params;
            this.selector = selector;
        }

        public (int, int) Coordinates => coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            GetPossibleError(effectName, errors);
            if (effectName.Return != ReturnType.String) errors.Add($"Not a string at name in effect assignation at {coordinates.Item1}:{coordinates.Item2}");
            GetPossibleError(selector, errors);
            if (!(selector is null) && selector.Return != ReturnType.List) errors.Add($"Invalid selector declaration at {coordinates.Item1}:{coordinates.Item2}");

            foreach (var item in _params)
            {
                GetPossibleError(item.Item2, errors);
            }

            try
            {
                effectReference = EffectStatement.Effects[(string)effectName.Evaluate()];
                effectReference.Receive(_params, selector); //if a null selector is received, targets will remain un-initialized
            }
            catch (KeyNotFoundException)
            {
                errors.Add($"Previously undeclared method assigned at {coordinates.Item1}:{coordinates.Item2}");
            }
            catch(EvaluationError error) //thrown at Recieve call
            {
                errors.Add(error.Message);
            }

            return errors.Count == 0;
        }

        public void Execute() => effectReference.Execute();

        private void GetPossibleError(IExpression expr, List<string> errors)
        {
            if(!expr.CheckSemantic(out string error)) errors.Add(error);
        }
    }
}
