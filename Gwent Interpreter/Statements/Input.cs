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
        List<Card> createdCards;

        public Input(List<IStatement> cards, List<IStatement> effects)
        {
            this.cards = cards;
            this.effects = effects;
            createdCards = new List<Card>();
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
            if (!executed)
            {
                int previousCount = CardStatement.Cards.Count;
                foreach (var item in cards) item.Execute();
                createdCards = CardStatement.Cards.GetRange(previousCount, CardStatement.Cards.Count - previousCount);
                executed = true;
            }
        }

        public List<Card> CreatedCards()
        {
            if (!executed) Execute();
            return createdCards;
        }
    }
}
