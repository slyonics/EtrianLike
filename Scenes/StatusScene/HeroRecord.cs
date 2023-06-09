﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.StatusScene
{
    public enum HeroType
    {
        Cyra,
        Angélia,
        Wren
    }

    public enum ElementType
    {
        Physical,
        Fire,
        Electric,
        Nuclear,
        Poison,
        Glass,
        Water
    }

    public class HeroRecord
    {
        public HeroType Name { get; set; }
        public ClassType Class { get; set; }
        public string Sprite { get; set; }
        public string ProfileSprite { get; set; }
        public string PortraitSprite { get; set; }
        public double HealthGrowth { get; set; }
        public double MagicGrowth { get; set; }
        public double StrengthGrowth { get; set; }
        public double DefenseGrowth { get; set; }
        public double AgilityGrowth { get; set; }
        public double ManaGrowth { get; set; }
        public int FlightHeight { get; set; }
        public int EquipmentSlots { get; set; } = 6;
        public string[] InitialEquipment { get; set; }
        public string[] InitialAbilities { get; set; }
        public string[] Actions { get; set; }
        public BattleScene.BattlerModel BattlerModel { get; set; }
    }
}
