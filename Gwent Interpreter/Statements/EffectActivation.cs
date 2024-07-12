using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class EffectActivation : IStatement
    {
        IExpression effectName;
        List<(Token, IExpression)> _params;
        IExpression selector;
        EffectStatement effectReference;

        public EffectActivation(IExpression effectName, List<(Token, IExpression)> @params, IExpression selector)
        {
            this.effectName = effectName;
            _params = @params;
            this.selector = selector;
        }

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            GetPossibleError(effectName, errors);
            if (effectName.Return != ReturnType.String) errors.Add("invalid type");
            GetPossibleError(selector, errors);
            if (selector.Return != ReturnType.List) errors.Add("invalid type");

            foreach (var item in _params)
            {
                GetPossibleError(item.Item2, errors);
            }

            try
            {
                effectReference = EffectStatement.Effects[(string)effectName.Evaluate()];
                effectReference.Receive(_params, selector);
            }
            catch (KeyNotFoundException)
            {
                errors.Add("previously undeclared method assigned");
            }
            catch(EvaluationError error)
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
