using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gwent_Interpreter.Statements
{
    class Input : IStatement
    {
        List<IStatement> cards;
        List<IStatement> effects;
        bool executed = false;
        List<Card> createdCards;

        static string mainPath = "D:\\";

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
            string warning = "";

            warning += GetWarningsAndErrors(effects, ref errors);
            warning += GetWarningsAndErrors(cards, ref errors);

            if (warning != "") throw new Warning(warning);
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

                WriteFiles(EffectStatement.EffectDeclaration, "Effects", ".gwf");
                WriteFiles(CardStatement.CardDeclaration, "Cards", ".gwc"); 
            }
        }

        static void WriteFiles(Dictionary<string,string> dictionary, string folder, string ext)
        {
            foreach (var pair in dictionary)
            {
                StreamWriter sw = new StreamWriter(mainPath + folder + pair.Key + ext);
                sw.WriteLine(pair.Value); 
                sw.Close();
            }
        }

        public List<Card> CreatedCards()
        {
            if (!executed) Execute();
            return createdCards;
        }

        static string GetWarningsAndErrors(List<IStatement> list, ref List<string> errors)
        {
            string warning = "";
            
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    if (!list[i].CheckSemantic(out List<string> temp)) errors.AddRange(temp);
                }
                catch (Warning warn)
                {
                    warning += warn.Message + "\n";
                }
            }
            return warning;
        }
    }
}
