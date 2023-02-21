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
    public class RampantGrowth : BaseMutation
    {
        public int GrowthCounter;

        public RampantGrowth() => this.DisplayName = "Rampant Growth";

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart) this, "BeginTakeAction");
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "Your potential is ever improving.";
        }

        public override string GetLevelText(int Level)
        {
            string Ret = "Gain {{rules|" + (1 + (Level / 10)) + "}} attribute point(s) every {{rules|" + (10500 - (Level * 500)).ToString() + "}} turns.";
            return Ret;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                ++this.GrowthCounter;
                if (this.GrowthCounter > Math.Max(10500 - (this.Level * 500), 2500))
                {
                    if (this.ParentObject.HasStat("AP"))
                    {
                        int growthAmount = 1 + (this.Level / 10);
                        this.ParentObject.GetStat("AP").BaseValue += growthAmount;
                        Popup.Show("Your potential increases.\n\n{{C|You gain " + growthAmount.Things("attribute point") + "!}}");
                    }
                    this.GrowthCounter = 0;
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