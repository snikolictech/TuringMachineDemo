using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TuringMachine
{
    internal class Program
    {
        private static void Main()
        {
            FileReader.MethSwitch methSwitch;

            methSwitch = FileReader.MethSwitch.SkipFirst;
            var RuleCons = new FileReader(methSwitch, 4);

            methSwitch = FileReader.MethSwitch.TakeFirst;
            var SettCons = new FileReader(methSwitch, 4);

            var RuleArray = new Rule[RuleCons.Element.Count];
            RuleArray = Rule.CreateRuleSet(RuleArray, RuleCons.Element);

            var TapeMach = new TapeMachine
            {
                OffsetPos = int.Parse(SettCons.Element[1][0]),
                State = SettCons.Element[2][0],
                Halt = SettCons.Element[3][0]
            };

            var Inits = SettCons.ParseInits(SettCons.Element[0][0]);
            TapeMach.TapeContentsR = TapeMach.InitFrom(Inits);
            TapeMach.RunMachine(RuleArray);

            Console.ReadLine();
        }
    }

    internal class FileReader
    {
        public enum MethSwitch
        {
            SkipFirst,
            TakeFirst
        };

        private readonly StreamReader Sr = new StreamReader("coho.txt");
        public List<string[]> Element = new List<string[]>();
        //public FileReader(MethSwitch methSwitch, int amount)
        //{
        //    string line;
        //    List<string> entries = new List<string>();

        //    while ((line = Sr.ReadLine()) != null)
        //    {
        //        entries.Add(line);
        //    }

        //    if (methSwitch == MethSwitch.SkipFirst)
        //    {
        //        foreach (var entry in entries.Skip<string>(amount))
        //        {
        //            Element.Add(entry.Split());
        //        }
        //    }
        //    else if (methSwitch == MethSwitch.TakeFirst)
        //    {
        //        foreach (var entry in entries.Take<string>(amount))
        //        {
        //            Element.Add(entry.Split());
        //        }
        //    }
        //}

        public FileReader(MethSwitch methSwitch, int amount)
        {
            string line;
            var entries = new List<string>();

            while ((line = Sr.ReadLine()) != null)
            {
                entries.Add(line);
            }

            if (methSwitch == MethSwitch.SkipFirst)
            {
                foreach (var entry in entries.Skip(amount))
                {
                    Element.Add(entry.Split());
                }
            }
            else if (methSwitch == MethSwitch.TakeFirst)
            {
                foreach (var entry in entries.Take(amount))
                {
                    Element.Add(entry.Split());
                }
            }
        }

        public string[] ParseInits(string inits)
        {
            var Inits = new string[inits.Length];
            for (var i = 0; i < inits.Length; i++)
            {
                Inits[i] = inits[i].ToString();
            }
            return Inits;
        }
    }

    internal class Rule
    {
        public string move;
        public string newState;
        public string read;
        public string state;
        public string write;

        public Rule()
        {
        }

        public Rule(string state, string read, string write, string move, string newState)
        {
            this.state = state;
            this.read = read;
            this.write = write;
            this.move = move;
            this.newState = newState;
        }

        public static Rule[] CreateRuleSet(Rule[] RuleArray, List<string[]> Element)
        {
            var lineCounter = 0;
            foreach (var ruleLine in Element)
            {
                RuleArray[lineCounter] = new Rule(ruleLine[0], ruleLine[1], ruleLine[2], ruleLine[3], ruleLine[4]);
                lineCounter++;
            }
            return RuleArray;
        }
    }

    internal class TapeMachine
    {
        private readonly List<string> TapeContentsL = new List<string>();
        public string Halt;
        public int OffsetPos;
        public string State;
        private List<string> TapeContents; //reference to TapeContentsR / TapeContentsL
        public List<string> TapeContentsR = new List<string>();

        public void RunMachine(Rule[] ruleArray)
        {
            var currentRule = new Rule();

            while (State != Halt)
            {
                TapeContents = (OffsetPos < 0) ? TapeContentsL : TapeContentsR;

                if (Math.Abs(OffsetPos) == TapeContents.Count - 1) //increase collection if needed
                {
                    TapeContents.Add("-");
                }

                for (var i = 0; i < ruleArray.Length; i++)
                {
                    currentRule = ruleArray[i];

                    if (State == currentRule.state)
                    {
                        if (currentRule.read == TapeContents[Math.Abs(OffsetPos)])
                        {
                            TapeContents[Math.Abs(OffsetPos)] = currentRule.write;

                            OffsetPos += int.Parse(currentRule.move);
                            State = currentRule.newState;
                            PrintOutTape();
                        }
                        if (currentRule.read == "*")
                        {
                            OffsetPos += int.Parse(currentRule.move);
                            State = currentRule.newState;
                        }
                    }
                }
            }
        }

        public List<string> InitFrom(string[] Inits)
        {
            var temp = new List<string>();

            foreach (var entry in Inits)
            {
                temp.Add(entry);
            }
            return temp;
        }

        private void PrintOutTape()
        {
            for (var c = TapeContentsL.Count - 1; c > 0; c--) //print left side, then...
            {
                Console.Write(TapeContentsL[c] + " ");
            }
            for (var c = 0; c < TapeContentsR.Count - 1; c++) //print right side
            {
                Console.Write(TapeContentsR[c] + " ");
            }
            Console.WriteLine();
        }
    }
}