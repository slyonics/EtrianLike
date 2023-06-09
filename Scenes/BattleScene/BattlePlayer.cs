﻿using EtrianLike.Models;
using EtrianLike.SceneObjects.Particles;
using EtrianLike.SceneObjects.Widgets;
using EtrianLike.Scenes.StatusScene;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EtrianLike.Scenes.BattleScene
{
    public class BattlePlayer : Battler
    {
        public const int HERO_WIDTH = 100;
        public const int HERO_HEIGHT = 100;

        protected enum HeroAnimation
        {
            Ready,
            Attack,
            Hit,
            Dead,
            Hurting
        }

        public static readonly Dictionary<string, Animation> HERO_ANIMATIONS = new Dictionary<string, Animation>()
        {
            { HeroAnimation.Ready.ToString(), new Animation(0, 0, HERO_WIDTH, HERO_HEIGHT, 1, 10000) },
            { HeroAnimation.Attack.ToString(), new Animation(0, 0, HERO_WIDTH, HERO_HEIGHT, 1, 10000) },
            { HeroAnimation.Hit.ToString(), new Animation(0, 0, HERO_WIDTH, HERO_HEIGHT, 1, 10000) },
            { HeroAnimation.Dead.ToString(), new Animation(0, 0, HERO_WIDTH, HERO_HEIGHT, 1, 10000) },
            { HeroAnimation.Hurting.ToString(), new Animation(0, 0, HERO_WIDTH, HERO_HEIGHT, 1, 10000) }
        };

        private Dictionary<string, int> exercise = new Dictionary<string, int>();

        private AnimatedSprite shadowSprite;

        private HeroModel heroModel;
        public HeroModel HeroModel
        {
            get => heroModel; set
            {
                heroModel = value; if (HeroModel.FlightHeight.Value > 1)
                {
                    shadowSprite = new AnimatedSprite(AssetCache.SPRITES[HeroModel.ShadowSprite.Value], HERO_ANIMATIONS);
                }
            }
        }

        public BattlePlayer(Widget iParent, float widgetDepth)
            : base(iParent, widgetDepth)
        {
            shader = AssetCache.EFFECTS[GameShader.BattlePlayer].Clone();
            shader.Parameters["flashInterval"].SetValue(0.0f);
        }

        public override void LoadAttributes(XmlNode xmlNode)
        {
            base.LoadAttributes(xmlNode);
            stats = HeroModel;
            AnimatedSprite = new AnimatedSprite(AssetCache.SPRITES[HeroModel.ProfileSprite.Value], HERO_ANIMATIONS);
            bounds = AnimatedSprite.SpriteBounds();
            battleScene.AddBattler(this);

            if (Stats.Health.Value > HeroModel.MaxHealth.Value / 4) HeroModel.HealthColor.Value = new Color(252, 252, 252, 255);
            else if (Stats.Health.Value > 0) HeroModel.HealthColor.Value = new Color(228, 0, 88, 255);
            else HeroModel.HealthColor.Value = new Color(136, 20, 0, 255);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            shadowSprite?.Draw(spriteBatch, Bottom, null, Depth);
        }

        public void DrawShader(SpriteBatch spriteBatch)
        {
            if (!drawSprite) return;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, shader, null);
            AnimatedSprite.Draw(spriteBatch, Bottom - new Vector2(0, Dead ? 0 : HeroModel.FlightHeight.Value), null, Depth);
            spriteBatch.End();
        }

        public override void StartTurn()
        {
            base.StartTurn();

            PlayAnimation("Ready");

            HeroModel.NameColor.Value = new Color(206, 109, 10);

            // MENU STUFF
            battleScene.BattleViewModel.StartPlayerTurn(this);
        }

        public override void EndTurn(int initiativeModifier = 0)
        {
            base.EndTurn(initiativeModifier);

            HeroModel.NameColor.Value = new Color(252, 252, 252, 255);

            battleScene.BattleViewModel.EndPlayerTurn(this);
        }

        public void Exercise(string stat)
        {
            if (exercise.ContainsKey(stat)) exercise[stat] = exercise[stat] + 1;
            else exercise.Add(stat, 1);
        }

        public List<ConversationScene.DialogueRecord> GrowAfterBattle(EncounterRecord encounterRecord)
        {
            List<ConversationScene.DialogueRecord> reports = new List<ConversationScene.DialogueRecord>();

            int characterIndex = GetParent<DataGrid>().ChildList.IndexOf(GetParent<Panel>().GetParent<Panel>());

            foreach (var statUsage in exercise)
            {
                int amountGained = Math.Min(3, statUsage.Value);

                switch (statUsage.Key)
                {
                    case "Health":
                        {
                            double challengeBias = encounterRecord.Challenge - (Stats as HeroModel).NakedHealth.Value / 100;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).HealthGrowth.Value) points += Rng.RandomInt(3, 8);
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s HP increased by " + points + " points!";
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;

                    case "Magic":
                        {
                            double challengeBias = encounterRecord.Challenge - (Stats as HeroModel).NakedMagic.Value / 30;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).MagicGrowth.Value) points += Rng.RandomInt(2, 5);
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s SP increased by " + points + " points!";
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;

                    case "Strength":
                        {
                            double challengeBias = encounterRecord.Challenge - (Stats as HeroModel).NakedStrength.Value / 10;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).StrengthGrowth.Value) points++;
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s strength increased by " + points + ((points > 1) ? " points!" : " point!");
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;

                    case "Defense":
                        {
                            double challengeBias = encounterRecord.Challenge - (Stats as HeroModel).NakedDefense.Value / 10;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).DefenseGrowth.Value) points++;
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s defense increased by " + points + ((points > 1) ? " points!" : " point!");
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;

                    case "Agility":
                        {
                            double challengeBias = encounterRecord.Challenge - Math.Max((Stats as HeroModel).NakedAgility.Value, (Stats as HeroModel).NakedMana.Value) / 10;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).AgilityGrowth.Value) points++;
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s agility increased by " + points + ((points > 1) ? " points!" : " point!");
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;

                    case "Mana":
                        {
                            double challengeBias = encounterRecord.Challenge - Math.Max((Stats as HeroModel).NakedAgility.Value, (Stats as HeroModel).NakedMana.Value) / 10;
                            int points = 0;
                            for (int i = 0; i < amountGained; i++)
                            {
                                if (Rng.RandomDouble(0, 1) < challengeBias * (Stats as HeroModel).ManaGrowth.Value) points++;
                            }
                            if (points <= 0) continue;
                            string report = Stats.Name.Value + "'s magic increased by " + points + ((points > 1) ? " points!" : " point!");
                            ConversationScene.DialogueRecord dialogueRecord = new ConversationScene.DialogueRecord()
                            {
                                Text = report,
                                Script = new string[] { "IncreaseStat " + characterIndex + " " + statUsage.Key + " " + points }
                            };
                            reports.Add(dialogueRecord);
                        }
                        break;
                }
            }

            return reports;
        }

        public override void Damage(int damage)
        {
            base.Damage(damage);

            PlayAnimation("Hit", Idle);

            if (Stats.Health.Value > HeroModel.MaxHealth.Value / 4) HeroModel.HealthColor.Value = new Color(252, 252, 252, 255);
            else if (Stats.Health.Value > 0) HeroModel.HealthColor.Value = new Color(228, 0, 88, 255);
            else HeroModel.HealthColor.Value = new Color(136, 20, 0, 255);

            Exercise("Health");

            if (Defending || Rng.RandomInt(0, 4) == 0) Exercise("Defense");
        }

        public override void Heal(int healing)
        {
            base.Heal(healing);

            if (Stats.Health.Value > HeroModel.MaxHealth.Value / 4) HeroModel.HealthColor.Value = new Color(252, 252, 252, 255);
            else if (Stats.Health.Value > 0) HeroModel.HealthColor.Value = new Color(228, 0, 88, 255);
            else HeroModel.HealthColor.Value = new Color(136, 20, 0, 255);
        }

        public override void Animate(string animationName)
        {
            PlayAnimation(animationName, Idle);
        }

        public void Idle()
        {
            if (Stats.Health.Value > HeroModel.MaxHealth.Value / 4) PlayAnimation("Ready");
            else if (Stats.Health.Value > 0) PlayAnimation("Hurting");
            else PlayAnimation("Dead");
        }

        public override Vector2 Top { get => new Vector2(currentWindow.Center.X, currentWindow.Center.Y + bounds.Y - bounds.Height / 4) + Position; }
        public override Vector2 Center { get => new Vector2(currentWindow.Center.X, currentWindow.Center.Y + bounds.Y + bounds.Height / 4) + Position; }

        public override Rectangle SpriteBounds
        {
            get
            {
                return new Rectangle(currentWindow.Left + (int)Position.X, currentWindow.Top + (int)Position.Y, 100, 100);
            }
        }
    }
}
