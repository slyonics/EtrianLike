﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.StatusScene
{
    public enum TargetType
    {
        SingleEnemy,
        AllEnemy,
        SingleAlly,
        AllAlly,
        //All,
        Self,
        //Auto
    }

    [Serializable]
    public class CommandRecord
    {
        public CommandRecord()
        {

        }

        public CommandRecord(CommandRecord clone)
        {
            Name = clone.Name;
            Animation = clone.Animation;
            Description = (string)clone.Description.Clone();
            Icon = clone.Icon;
            ChargesLeft = clone.ChargesLeft;
            Charges = clone.Charges;
            Cost = clone.Cost;
            Targetting = clone.Targetting;
            TargetDead = clone.TargetDead;
            TargetMechanical = clone.TargetMechanical;
            TargetOrganic = clone.TargetOrganic;
            if (clone.Script != null) Script = (string[])clone.Script.Clone();
            if (clone.PreScript != null) PreScript = (string[])clone.PreScript.Clone();
        }

        public string Name { get; set; }
        public string Animation { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int ChargesLeft { get; set; } = -1;
        public int Charges { get; set; } = -1;
        public int Cost { get; set; } = -1;
        public bool ShowCharges { get => Charges >= 0; }
        public bool ShowCost { get => Cost >= 0; }
        public bool Usable { get; set; } = true;
        public TargetType Targetting { get; set; }
        public bool TargetDead { get; set; } // true if this can target dead allies
        public bool TargetMechanical { get; set; } // true if this only targets robots
        public bool TargetOrganic { get; set; } // true if this only targets non-robots
        public string[] PreScript { get; set; }
        public string[] Script { get; set; }
    }
}
