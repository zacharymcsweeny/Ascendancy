using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Qud.API;
using XRL.Language;
using XRL.Rules;
using XRL.Messages;
using XRL.UI;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using ConsoleLib.Console;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Transcendence : BaseMutation
    {
        public int TranscendenceCounter;
        public int TranscendenceLevel;

        public Transcendence() => this.DisplayName = "Transcendence";

        public override bool CanLevel() => false;

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart) this, "BeginTakeAction");
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "You approach perfection.";
        }

        public override string GetLevelText(int Level)
        {
            string Ret = "Perfection nears every {{rules|" + (Math.Max(12000 - (this.TranscendenceLevel * 600), 6000)).ToString() + "}} rounds.\nYou have taken {{rules|" + this.TranscendenceLevel.ToString() + "}} steps along your path.";
            return Ret;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                ++this.TranscendenceCounter;
                if (this.TranscendenceCounter > Math.Max(12000 - (this.TranscendenceLevel * 600), 6000))
                {
                    string Message = "Perfection approaches.\n";
                    if (this.ParentObject.HasStat("MP"))
                    {
                        int TranscendenceMutationAmount = 1 + (this.TranscendenceLevel / 10);
                        this.ParentObject.GainMP(TranscendenceMutationAmount);
                        Message += "\n{{C|You gain " + TranscendenceMutationAmount.Things("mutation point") + "!}}";
                    }
                    this.ParentObject.PermuteRandomMutationBuys();
                    if (this.ParentObject.HasStat("AP"))
                    {
                        int TranscendenceAttributeAmount = 1 + (this.TranscendenceLevel / 10);
                        this.ParentObject.GetStat("AP").BaseValue += TranscendenceAttributeAmount;
                        Message += "\n{{C|You gain " + TranscendenceAttributeAmount.Things("attribute point") + "!}}";
                    }
                    // if (this.ParentObject.HasStat("SP"))
                    // {
                    //     int TranscendenceSkillPointAmount = 45 + (this.TranscendenceLevel * 5);
                    //     this.ParentObject.GainSP(TranscendenceSkillPointAmount);
                    //     Message += "\n{{C|You gain " + TranscendenceSkillPointAmount.Things("skill point") + "!}}";
                    // }
                    if (this.ParentObject.HasStat("AV"))
                    {
                        int TranscendenceAVAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceAVAmount > 0)
                        {
                            this.ParentObject.GetStat("AV").BaseValue += TranscendenceAVAmount;
                            Message += "\n{{C|You gain " + TranscendenceAVAmount + "AV!}}";
                        }
                    }
                    if (this.ParentObject.HasStat("DV"))
                    {
                        int TranscendenceDVAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceDVAmount > 0)
                        {
                            this.ParentObject.GetStat("DV").BaseValue += TranscendenceDVAmount;
                            Message += "\n{{C|You gain " + TranscendenceDVAmount + " DV!}}";
                        }
                    }
                    if (this.ParentObject.HasStat("HeatResistance"))
                    {
                        int TranscendenceHeatResistanceAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceHeatResistanceAmount > 0)
                        {
                            this.ParentObject.GetStat("HeatResistance").BaseValue += TranscendenceHeatResistanceAmount;
                            Message += "\n{{C|You gain " + TranscendenceHeatResistanceAmount + " heat resistance!}}";
                        }
                    }
                    if (this.ParentObject.HasStat("ColdResistance"))
                    {
                        int TranscendenceColdResistanceAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceColdResistanceAmount > 0)
                        {
                            this.ParentObject.GetStat("ColdResistance").BaseValue += TranscendenceColdResistanceAmount;
                            Message += "\n{{C|You gain " + TranscendenceColdResistanceAmount + " cold resistance!}}";
                        }
                    }
                    if (this.ParentObject.HasStat("ElectricResistance"))
                    {
                        int TranscendenceElectricResistanceAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceElectricResistanceAmount > 0)
                        {
                            this.ParentObject.GetStat("ElectricResistance").BaseValue += TranscendenceElectricResistanceAmount;
                            Message += "\n{{C|You gain " + TranscendenceElectricResistanceAmount + " electric resistance!}}";
                        }
                    }
                    if (this.ParentObject.HasStat("AcidResistance"))
                    {
                        int TranscendenceAcidResistanceAmount = this.TranscendenceLevel / 20;
                        if (TranscendenceAcidResistanceAmount > 0)
                        {
                            this.ParentObject.GetStat("AcidResistance").BaseValue += TranscendenceAcidResistanceAmount;
                            Message += "\n{{C|You gain " + TranscendenceAcidResistanceAmount + " acid resistance!}}";
                        }
                    }

                    int TranscendenceQuicknessAmount = this.TranscendenceLevel / 20;
                    if (TranscendenceQuicknessAmount > 0)
                    {
                        this.ParentObject.Statistics["Speed"].BaseValue += TranscendenceQuicknessAmount;
                        Message += "\n{{C|You gain " + TranscendenceQuicknessAmount + " Quickness!}}";
                    }

                    int TranscendenceMoveSpeedAmount = this.TranscendenceLevel / 20;
                    if (TranscendenceMoveSpeedAmount > 0)
                    {
                        this.ParentObject.Statistics["MoveSpeed"].BaseValue -= TranscendenceMoveSpeedAmount;
                        Message += "\n{{C|You gain " + TranscendenceMoveSpeedAmount + " movement speed!}}";
                    }

                    int TranscendenceCyberCreditAmount = this.TranscendenceLevel / 10;
                    if (TranscendenceCyberCreditAmount > 0)
                    {
                        this.ParentObject.ModIntProperty("CyberneticsLicenses", TranscendenceCyberCreditAmount);
                        this.ParentObject.ModIntProperty("FreeCyberneticsLicenses", TranscendenceCyberCreditAmount);
                        Message += "\n{{C|Your cybernetics license tier goes up by " + TranscendenceCyberCreditAmount + "!}}";
                    }

                    Popup.Show(Message);

                    if (this.TranscendenceLevel > 9)
                    {
                        RapidAdvancement(3);
                    }

                    ++this.TranscendenceLevel;
                    this.TranscendenceCounter = 0;
                }
            }
            return base.FireEvent(E);
        }

        public void RapidAdvancement(int Amount)
        {
            if (Amount <= 0)
                return;
            string str = GetMutationTermEvent.GetFor(this.ParentObject);
            bool flag1 = this.ParentObject.IsPlayer() && this.ParentObject.Stat("MP") >= 4;
            bool flag2 = false;
            if (flag1 && Popup.ShowYesNo("Your genome enters an excited state! Would you like to spend {{rules|4}} mutation points to buy " + Grammar.A(str) + " before rapidly mutating?", false) == DialogResult.Yes)
            {
                flag2 = MutationsAPI.BuyRandomMutation(this.ParentObject, Confirm: false, MutationTerm: str);
            }
            List<BaseMutation> list = this.ParentObject.GetPhysicalMutations().Where<BaseMutation>((Func<BaseMutation, bool>) (m => m.CanLevel())).ToList<BaseMutation>();
            if (list.Count > 0)
            {
                if (!flag1 && this.ParentObject.IsPlayer())
                    Popup.Show("Your genome enters an excited state!");
                if (this.ParentObject.IsPlayer())
                {
                    string[] array = list.Select<BaseMutation, string>((Func<BaseMutation, string>) (m => m.DisplayName + " ({{C|" + m.Level.ToString() + "}})")).ToArray<string>();
                    int index = Popup.ShowOptionList("Choose a physical " + str + " to rapidly advance.", array);
                    Popup.Show("You have rapidly advanced " + list[index].DisplayName + " by " + Grammar.Cardinal(Amount) + " ranks to rank {{C|" + (list[index].Level + Amount).ToString() + "}}!");
                    list[index].RapidLevel(Amount);
                }
                else
                list.GetRandomElement<BaseMutation>((Random) null).RapidLevel(Amount);
            }
            else
            {
                if (!flag2)
                return;
                Popup.Show("You have no physical " + Grammar.Pluralize(str) + " to rapidly advance!");
            }
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