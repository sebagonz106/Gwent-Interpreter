using System;
using System.Collections.Generic;
using System.Text;

namespace Gwent_Interpreter.Statements
{
    class Input : IStatement
    {
        List<IStatement> cards;
        List<IStatement> effects;
        bool executed = false;

        public Input(List<IStatement> cards, List<IStatement> effects)
        {
            this.cards = cards;
            this.effects = effects;
        }

        public (int, int) Coordinates => (0, 0);

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            List<string> temp = new List<string>();
           
            foreach (var item in effects) item.CheckSemantic(out errors);
            foreach (var item in cards) item.CheckSemantic(out temp);

            errors.AddRange(temp);
            return errors.Count == 0;
        }

        public void Execute()
        {
            if(!executed) foreach (var item in cards) item.Execute();
            executed = true;
        }

        public List<Card> CreatedCards()
        {
            if (!executed) Execute();
            return CardStatement.Cards;
        }
    }
}
