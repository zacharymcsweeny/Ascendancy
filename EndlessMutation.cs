using System;
using System.Collections.Generic;
using System.Text;

using XRL.Rules;
using XRL.Messages;
using XRL.UI;
using ConsoleLib.Console;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class EndlessMutation : BaseMutation
    {
        public int MutateCounter;

        public EndlessMutation() => this.DisplayName = "Endless Mutation";

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart) this, "BeginTakeAction");
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "Your genome is malleable and ever-shifting.";
        }

        public override string GetLevelText(int Level)
        {
            string Ret = "Gain {{rules|" + (1 + (this.Level / 10)).ToString() + "}} mutation point(s) every {{rules|" + (10500 - (Level * 500)).ToString() + "}} turns.";
            return Ret;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                ++this.MutateCounter;
                if (this.MutateCounter > Math.Max(10500 - (this.Level * 500), 2500))
                {
                    if (this.ParentObject.HasStat("MP"))
                    {
                        int mutationAmount = 1 + (this.Level / 10);
                        this.ParentObject.GainMP(mutationAmount);
                        Popup.Show("Your restless genome advances once again.\n\n{{C|You gain " + mutationAmount.Things("mutation point") + "!}}");
                    }
                    this.ParentObject.PermuteRandomMutationBuys();
                    this.MutateCounter = 0;
                }
            }
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            return base.Unmutate(GO);
        }
    }
}