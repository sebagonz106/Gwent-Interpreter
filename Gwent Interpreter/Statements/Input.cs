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
        static int lastEffectCount=0;

        static string mainPath = "D:\\Gwent-Pro\\Gwent-Interpreter\\Gwent Interpreter\\Files\\";

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
                int previousCardCount = CardStatement.Cards.Count;
                foreach (var item in cards) item.Execute();
                createdCards = CardStatement.Cards.GetRange(previousCardCount, CardStatement.Cards.Count - previousCardCount);
                executed = true;

                string effectsWarning = WriteFilesMindingRepetition(EffectStatement.EffectDeclaration, lastEffectCount, "Effects\\", ".gwf");
                string cardsWarning = WriteFilesMindingRepetition(CardStatement.CardDeclaration, previousCardCount, "Cards\\", ".gwc");

                lastEffectCount = EffectStatement.Effects.Count;

                if (effectsWarning.Length != 0 || cardsWarning.Length != 0) throw new Warning(effectsWarning + cardsWarning);
            }
        }

        static string WriteFilesMindingRepetition(Dictionary<string,string> dictionary, int start, string folder, string ext)
        {
            string warnings = "";
            int startSave = start;

            foreach (var pair in dictionary) //checking if there will be an error before creating the files, as this step will be invalidated in Interpreter class
                if (start > 0) start--;
                else if (File.Exists(mainPath + folder + pair.Key + ext))
                    warnings+=$"{pair.Key} was previously declared. Another name must be used.\n";

            if (warnings.Length == 0) foreach (var pair in dictionary)
            {
                if (startSave > 0) startSave--;
                else
                {
                    StreamWriter sw = new StreamWriter(mainPath + folder + pair.Key + ext);
                    sw.WriteLine(pair.Value);
                    sw.Close();
                }
            }

            return warnings;
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

        public static void Reset() => lastEffectCount = 0;
    }
}
