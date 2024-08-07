﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class OnActivation : IStatement
    {
        (int, int) coordinates;
        List<(EffectActivation, EffectActivation)> effects;

        public OnActivation((int, int) coordinates, List<(EffectActivation, EffectActivation)> effects)
        {
            this.coordinates = coordinates;
            this.effects = effects;
        }

        public (int, int) Coordinates => coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            foreach (var item in effects)
            {
                item.Item1.CheckSemantic(out errors);
                if (item.Item2.Coordinates != (0,0))
                {
                    item.Item2.CheckSemantic(out List<string> temp); //postAction
                    errors.AddRange(temp);
                }
            }

            return errors.Count == 0;
        }

        public void Execute()
        {
            foreach (var item in effects)
            {
                item.Item1.Execute(); 
                if(!(item.Item2 is null)) item.Item2.Execute(); //postAction
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
