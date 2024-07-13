using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class OnActivation : IStatement
    {
        (int, int) coordinates;
        List<(EffectActivation, EffectActivation)> effects;

        public (int, int) Coordinates => coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            foreach (var item in effects)
            {
                item.Item1.CheckSemantic(out errors);
                item.Item2.CheckSemantic(out List<string> temp); //postAction
                errors.AddRange(temp);
            }

            return errors.Count == 0;
        }

        public void Execute()
        {
            foreach (var item in effects)
            {
                item.Item1.Execute(); 
                item.Item2.Execute(); //postAction
            }
        }

        public bool MethodExecution(Context context)
        {
            try
            {
                Execute();
                return true;
            }
            catch(EvaluationError)
            {
                return false;
            }
        }
    }
}
