using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using XRL.Core;
using XRL.Rules;
using XRL.Language;
using XRL.Messages;
using XRL.UI;
using XRL.World.Capabilities;
using XRL.World.Effects;
using ConsoleLib.Console;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Dreamweaving : BaseMutation
    {
        public Guid LucidityActivatedAbilityID = Guid.Empty;
        public Guid LucidLanceActivatedAbilityID = Guid.Empty;
        public Guid DeepDreamingActivatedAbilityID = Guid.Empty;
        public Guid RefineLucidityPowerActivatedAbilityID = Guid.Empty;
        public Guid RefineLuciditySpeedActivatedAbilityID = Guid.Empty;
        public Guid RefineLucidityEfficiencyActivatedAbilityID = Guid.Empty;

        public int DreamCharge;
        public int LucidityPowerLevel;
        public int LuciditySpeedLevel;
        public int LucidityEfficiencyLevel;
        public float LucidityEfficiencyFactor = 0.95f;
        public int LucidityMax = 1000;

        public int LucidLanceCost = 1000;
        public int RefineLucidityPowerCost = 10000;
        public int RefineLuciditySpeedCost = 10000;
        public int RefineLucidityEfficiencyCost = 10000;

        [NonSerialized]
        private static GameObject _Projectile;

        private static GameObject Projectile
        {
        get
        {
            if (!GameObject.validate(ref Dreamweaving._Projectile))
                Dreamweaving._Projectile = GameObject.createUnmodified("ProjectileDreamweaving");
            return Dreamweaving._Projectile;
        }
        }

        public Dreamweaving() => this.DisplayName = "Dreamweaving";

        public override bool CanLevel() => false;

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart) this, "BeginTakeAction");
            Object.RegisterPartEvent((IPart) this, "CommandLucidity");
            Object.RegisterPartEvent((IPart) this, "CommandLucidLance");
            Object.RegisterPartEvent((IPart) this, "CommandDeepDreaming");
            Object.RegisterPartEvent((IPart) this, "CommandRefineLucidityPower");
            Object.RegisterPartEvent((IPart) this, "CommandRefineLuciditySpeed");
            Object.RegisterPartEvent((IPart) this, "CommandRefineLucidityEfficiency");
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "You can sculpt reality.";
        }

        public override string GetLevelText(int Level)
        {
            string Ret = "You can store up to {{rules|" + (this.LucidityMax).ToString() + "}} dreamweaving charge.\nYou have increased your dreamweaving mastery {{rules|" + (this.LucidityPowerLevel).ToString() + "}} times.";
            return Ret;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                this.DreamCharge += 10 + (this.LucidityMax / 10000);
                if (this.DreamCharge > this.LucidityMax)
                {
                    this.DreamCharge = this.LucidityMax / 2;
                    this.LucidityMax += this.DreamCharge / 10;
                    IComponent<GameObject>.AddPlayerMessage("You slip further into the dream. Your maximum lucidity increases by " + (this.DreamCharge / 10).ToString() + ".");
                }
                this.SyncLucidity();
            }
            else if (E.ID == "CommandLucidity")
            {
                string Message = "As the lucid dreamer, you can shape the dream you walk.\n";

                Message += "\nYou have {{rules|" + this.DreamCharge.ToString() + "}} out of your available {{rules|" + this.LucidityMax.ToString() + "}} dreamweaving capacity remaining.";
                Message += "\nYou recover {{rules|" + (10 + this.LucidityMax / 10000).ToString() + "}} dreamweaving capacity each round.";
                Message += "\nWhen you reach maximum capacity, your remaining capacity is halved to permanently increase your maximum capacity by a small amount.";
                Message += "\n";
                Message += "\nYour dreamweaving power has been refined {{rules|" + this.LucidityPowerLevel.ToString() + "}} times.";
                Message += "\nYour dreamweaving speed has been refined {{rules|" + this.LuciditySpeedLevel.ToString() + "}} times.";
                Message += "\nYour dreamweaving efficiency has been refined {{rules|" + this.LucidityEfficiencyLevel.ToString() + "}} times.";
                Message += "\n";
                Message += "\nLucid Lance consumes {{rules|" + (Math.Floor(this.LucidLanceCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel))).ToString() + "}} capacity to deal {{c|\x1a}}{{y|" + GetLancePenetrationBonus().ToString() + "}} {{r|\x03}}{{y|" + GetLanceDamage() + "}}.";
                Message += "\nRefining your dreamweaving consumes {{rules|" + Math.Floor(this.RefineLucidityEfficiencyCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel)).ToString() + "}} capacity to increase the corresponding value.";

                Popup.Show(Message);
            }
            else if (E.ID == "CommandLucidLance")
            {
                if (!this.IsMyActivatedAbilityUsable(this.LucidLanceActivatedAbilityID))
                    return false;
                if (this.ParentObject.OnWorldMap())
                {
                    if (this.ParentObject.IsPlayer())
                        Popup.ShowFail("You cannot do that on the world map.");
                    return false;
                }
                List<Cell> cellList = this.PickLine(999, AllowVis.Any, (Predicate<GameObject>) (o => o.HasPart("Combat") && o.PhaseMatches(this.ParentObject)), Label: "Lance", Snap: true);
                if (cellList == null || cellList.Count <= 0)
                    return false;
                if (cellList.Count > 1000)
                    cellList.RemoveRange(1000, cellList.Count - 1000);
                this.ParentObject?.PlayWorldSound("Sounds/Abilities/sfx_ability_mutation_lightManipulation_laser_fire");
                Cell cell1 = cellList[0];
                Cell cell2 = cellList[cellList.Count - 1];
                float degrees = (float) Math.Atan2((double) (cell2.X - cell1.X), (double) (cell2.Y - cell1.Y)).toDegrees();
                cellList.RemoveAt(0);
                for (int index = 0; index < cellList.Count; ++index)
                {
                    Cell cell3 = cellList[index];
                }
                int index1 = 0;
                int count = cellList.Count;
                while (index1 < count && !this.Lance(cellList[index1], count))
                    ++index1;

                this.DreamCharge -= (int)(this.LucidLanceCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel));
                IComponent<GameObject>.AddPlayerMessage("You have " + this.DreamCharge.ToString() + " lucidity remaining.");
                this.UseEnergy(Math.Max(1000 - (this.LuciditySpeedLevel * 5), 0));
            }
            else if (E.ID == "CommandDeepDreaming")
            {
                // int LucidityIncrease = this.DreamCharge / 10;
                // this.LucidityMax += LucidityIncrease;
                // this.DreamCharge = 0;
                // Popup.Show("You dream deeper.\n{{C|Your maximum lucidity increases by " + LucidityIncrease.ToString() + "!}}");
                this.ParentObject.ApplyEffect((Effect) new DeepDream());
                this.SyncLucidity();
                this.UseEnergy(Math.Max(1000 - (this.LuciditySpeedLevel * 5), 0));
            }
            else if (E.ID == "CommandRefineLucidityPower")
            {
                if (this.DreamCharge >= (int)(this.RefineLucidityPowerCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel)))
                {
                    ++this.LucidityPowerLevel;
                    this.DreamCharge -= (int)(this.RefineLucidityPowerCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel));
                    Popup.Show("You refine your control over the dream.\n{{C|Your dreamweaving power increases by 1!}}");
                    this.SyncLucidity();
                    this.UseEnergy(Math.Max(1000 - (this.LuciditySpeedLevel * 5), 0));
                }
            }
            else if (E.ID == "CommandRefineLuciditySpeed")
            {
                if (this.DreamCharge >= (int)(this.RefineLuciditySpeedCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel)))
                {
                    ++this.LuciditySpeedLevel;
                    this.DreamCharge -= (int)(this.RefineLuciditySpeedCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel));
                    Popup.Show("You refine your control over the dream.\n{{C|Your dreamweaving speed increases by 1!}}");
                    this.SyncLucidity();
                    this.UseEnergy(Math.Max(1000 - (this.LuciditySpeedLevel * 5), 0));
                }
            }
            else if (E.ID == "CommandRefineLucidityEfficiency")
            {
                if (this.DreamCharge >= (int)(this.RefineLucidityEfficiencyCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel)))
                {
                    ++this.LucidityEfficiencyLevel;
                    this.DreamCharge -= (int)(this.RefineLucidityEfficiencyCost * Math.Pow(this.LucidityEfficiencyFactor, this.LucidityEfficiencyLevel));
                    Popup.Show("You refine your control over the dream.\n{{C|Your dreamweaving efficiency increases by 1!}}");
                    this.SyncLucidity();
                    this.UseEnergy(Math.Max(1000 - (this.LuciditySpeedLevel * 5), 0));
                }
            }
            return base.FireEvent(E);
        }

        public bool Lance(Cell C, int PathLength = 0)
        {
            TextConsole textConsole = Look._TextConsole;
            ScreenBuffer screenBuffer = TextConsole.ScrapBuffer.WithMap();
            bool flag = false;
            if (C != null)
            {
                GameObject combatTarget = C.GetCombatTarget(this.ParentObject, true, Projectile: Dreamweaving.Projectile, InanimateSolidOnly: true);
                if (combatTarget != null)
                {
                int penetrationBonus = this.GetLancePenetrationBonus();
                int Result = Stat.RollDamagePenetrations(combatTarget.Stat("AV"), penetrationBonus, penetrationBonus);
                if (Result > 0)
                {
                    string resultColor = Stat.GetResultColor(Result);
                    int num = 0;
                    string damage = this.GetLanceDamage();
                    for (int index = 0; index < Result; ++index)
                    num += damage.RollCached();
                    GameObject gameObject = combatTarget;
                    int Amount = num;
                    GameObject parentObject = this.ParentObject;
                    string Message = "from %t lance ray! {{" + resultColor + "|(x" + Result.ToString() + ")}}";
                    GameObject Owner = parentObject;
                    gameObject.TakeDamage(Amount, Message, "Lucid Lance", Owner: Owner, ShowForInanimate: true);
                }
                else if (this.ParentObject.IsPlayer())
                    IComponent<GameObject>.AddPlayerMessage("Your lance ray doesn't penetrate " + Grammar.MakePossessive(combatTarget.the + combatTarget.ShortDisplayName) + " armor.", 'r');
                else if (combatTarget.IsPlayer())
                    IComponent<GameObject>.AddPlayerMessage(Grammar.MakePossessive(this.ParentObject.the + this.ParentObject.ShortDisplayName) + " lance ray doesn't penetrate your armor.", 'g');
                flag = true;
                }
            }
            if (C.IsVisible() || this.ParentObject.IsPlayer())
            {
                switch (Stat.Random(1, 3))
                {
                case 1:
                    screenBuffer.WriteAt(C, "&C\u000F");
                    break;
                case 2:
                    screenBuffer.WriteAt(C, "&Y\u000F");
                    break;
                default:
                    screenBuffer.WriteAt(C, "&B\u000F");
                    break;
                }
                screenBuffer.Draw();
                int millisecondsTimeout = 10 - PathLength / 5;
                if (millisecondsTimeout > 0)
                Thread.Sleep(millisecondsTimeout);
            }
            return flag;
        }

        public string GetLanceDamage()
        {
            int LanceLevel = this.LucidityPowerLevel / 4;
            if (LanceLevel == 0)
                return "1d2";
            if (LanceLevel <= 1)
                return "1d3";
            if (LanceLevel <= 2)
                return "1d4";
            if (LanceLevel <= 3)
                return "1d5";
            if (LanceLevel <= 4)
                return "1d4+1";
            if (LanceLevel <= 5)
                return "1d5+1";
            if (LanceLevel <= 6)
                return "1d4+2";
            if (LanceLevel <= 7)
                return "1d5+2";
            if (LanceLevel <= 8)
                return "1d4+3";
            if (LanceLevel <= 9)
                return "1d5+3";
            return LanceLevel > 9 ? "1d5+" + (LanceLevel - 6).ToString() : "1d4+4";
        }

        public int GetLancePenetrationBonus() => 4 + (this.LucidityPowerLevel - 1) / 8;

        public void SyncLucidity()
        {
            StringBuilder stringBuilder = Event.NewStringBuilder();
            stringBuilder.Append("Lucidity [").Append(this.DreamCharge).Append("/").Append(this.LucidityMax).Append("]");
            this.SetMyActivatedAbilityDisplayName(this.LucidityActivatedAbilityID, stringBuilder.ToString());
            this.SyncLucidLance();
            this.SyncDeepDreaming();
            this.SyncRefineLucidityPower();
            this.SyncRefineLuciditySpeed();
            this.SyncRefineLucidityEfficiency();
        }

        public void SyncLucidLance()
        {
            if (this.DreamCharge < 1000)
                this.DisableMyActivatedAbility(this.LucidLanceActivatedAbilityID);
            else
                this.EnableMyActivatedAbility(this.LucidLanceActivatedAbilityID);
        }

        public void SyncDeepDreaming()
        {
            // if (this.DreamCharge < this.LucidityMax)
                // this.DisableMyActivatedAbility(this.DeepDreamingActivatedAbilityID);
            // else
                this.EnableMyActivatedAbility(this.DeepDreamingActivatedAbilityID);
        }

        public void SyncRefineLucidityPower()
        {
            if (this.DreamCharge < this.RefineLucidityPowerCost)
                this.DisableMyActivatedAbility(this.RefineLucidityPowerActivatedAbilityID);
            else
                this.EnableMyActivatedAbility(this.RefineLucidityPowerActivatedAbilityID);
        }

        public void SyncRefineLuciditySpeed()
        {
            if (this.DreamCharge < this.RefineLuciditySpeedCost)
                this.DisableMyActivatedAbility(this.RefineLuciditySpeedActivatedAbilityID);
            else
                this.EnableMyActivatedAbility(this.RefineLuciditySpeedActivatedAbilityID);
        }

        public void SyncRefineLucidityEfficiency()
        {
            if (this.DreamCharge < this.RefineLucidityEfficiencyCost)
                this.DisableMyActivatedAbility(this.RefineLucidityEfficiencyActivatedAbilityID);
            else
                this.EnableMyActivatedAbility(this.RefineLucidityEfficiencyActivatedAbilityID);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.LucidityActivatedAbilityID = this.AddMyActivatedAbility("Lucidity", "CommandLucidity", "Dreamweaving", Icon: "ê");
            this.LucidLanceActivatedAbilityID = this.AddMyActivatedAbility("Lucid Lance", "CommandLucidLance", "Dreamweaving", Icon: "ê");
            this.DeepDreamingActivatedAbilityID = this.AddMyActivatedAbility("Dream Deeper", "CommandDeepDreaming", "Dreamweaving", Icon: "ê");
            this.RefineLucidityPowerActivatedAbilityID = this.AddMyActivatedAbility("Refine Lucidity Power", "CommandRefineLucidityPower", "Dreamweaving", Icon: "ê");
            this.RefineLuciditySpeedActivatedAbilityID = this.AddMyActivatedAbility("Refine Lucidity Speed", "CommandRefineLuciditySpeed", "Dreamweaving", Icon: "ê");
            this.RefineLucidityEfficiencyActivatedAbilityID = this.AddMyActivatedAbility("Refine Lucidity Efficiency", "CommandRefineLucidityEfficiency", "Dreamweaving", Icon: "ê");
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            this.RemoveMyActivatedAbility(ref this.LucidityActivatedAbilityID);
            this.RemoveMyActivatedAbility(ref this.LucidLanceActivatedAbilityID);
            this.RemoveMyActivatedAbility(ref this.DeepDreamingActivatedAbilityID);
            this.RemoveMyActivatedAbility(ref this.RefineLucidityPowerActivatedAbilityID);
            this.RemoveMyActivatedAbility(ref this.RefineLuciditySpeedActivatedAbilityID);
            this.RemoveMyActivatedAbility(ref this.RefineLucidityEfficiencyActivatedAbilityID);
            return base.Unmutate(GO);
        }
    }
}