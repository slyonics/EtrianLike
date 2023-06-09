﻿using EtrianLike.SceneObjects.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Collections.Specialized.BitVector32;

namespace EtrianLike.Scenes.BattleScene
{
    public class BattleEnemy : Battler
    {
        private const int FADE_IN_DURATION = 600;
        private const int ATTACK_DURATION = 500;
        private const int DEATH_DURATION = 600;

        private static readonly Dictionary<string, Texture2D> ENEMY_SHADOWS = new Dictionary<string, Texture2D>();

        private static Effect ENEMY_BATTLER_EFFECT;
        private static Texture2D STATIC_TEXTURE;

        public EnemyRecord EnemyRecord { get; set; }

        private int fadeInTime;
        private int attackTimeLeft;
        private int deathTimeLeft;

        public BattleEnemy(Widget iParent, float widgetDepth)
            : base(iParent, widgetDepth)
        {
            shader = ENEMY_BATTLER_EFFECT.Clone();
            shader.Parameters["destroyInterval"].SetValue(1.1f);
            shader.Parameters["noise"].SetValue(STATIC_TEXTURE);
            shader.Parameters["flashInterval"].SetValue(0.0f);
            shader.Parameters["flashColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
        }

        public override void LoadAttributes(XmlNode xmlNode)
        {
            base.LoadAttributes(xmlNode);

            stats = new BattlerModel(EnemyRecord);

            AnimatedSprite = new AnimatedSprite(AssetCache.SPRITES[(GameSprite)Enum.Parse(typeof(GameSprite), "Enemies_" + EnemyRecord.Sprite)], null);
            shadow = ENEMY_SHADOWS["Enemies_" + EnemyRecord.Sprite];

            bounds = AnimatedSprite.SpriteBounds();
            bounds.Y -= EnemyRecord.ShadowOffset / 2;
            if (AnimatedSprite.SpriteBounds().Height > 200) bounds.Y -= 24;

            battleScene.AddBattler(this);
        }

        public static void Initialize()
        {
            var enemyTextures = Enum.GetValues(typeof(GameSprite));
            foreach (GameSprite textureName in enemyTextures)
            {
                if (textureName == GameSprite.None) continue;

                Texture2D sprite = AssetCache.SPRITES[textureName];
                ENEMY_SHADOWS.Add(textureName.ToString(), BuildShadow(new Rectangle(-sprite.Width / 2, -sprite.Height / 2, sprite.Width, sprite.Height / 2)));
            }

            ENEMY_BATTLER_EFFECT = AssetCache.EFFECTS[GameShader.BattleEnemy];
            STATIC_TEXTURE = new Texture2D(CrossPlatformGame.GameInstance.GraphicsDevice, 200, 200);
            Color[] colorData = new Color[STATIC_TEXTURE.Width * STATIC_TEXTURE.Height];
            for (int y = 0; y < STATIC_TEXTURE.Height; y++)
            {
                for (int x = 0; x < STATIC_TEXTURE.Width; x++)
                {
                    colorData[y * STATIC_TEXTURE.Width + x] = new Color(Rng.RandomInt(0, 255), 255, 255, 255);
                }
            }
            STATIC_TEXTURE.SetData<Color>(colorData);
        }


        public override void DrawShadow(SpriteBatch spriteBatch)
        {
            Color shadowColor = Color.Lerp(SHADOW_COLOR, new Color(0, 0, 0, 0), Math.Min(1.0f, positionZ / (AnimatedSprite.SpriteBounds().Width + AnimatedSprite.SpriteBounds().Height) / 2));
            if (Dead) shadowColor.A = (byte)MathHelper.Lerp(0, shadowColor.A, (float)deathTimeLeft / DEATH_DURATION);
            spriteBatch.Draw(shadow, new Vector2((int)(Center.X - shadow.Width / 2), (int)(Center.Y) + 1 + EnemyRecord.ShadowOffset), null, shadowColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, SHADOW_DEPTH);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Transitioning) return;

            if (fadeInTime < FADE_IN_DURATION)
            {
                fadeInTime += gameTime.ElapsedGameTime.Milliseconds;
                float flashInterval = Math.Min((float)fadeInTime / FADE_IN_DURATION, 1.0f);
                shader.Parameters["flashInterval"].SetValue(1.0f - flashInterval);
            }

            if (attackTimeLeft > 0)
            {
                attackTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                if (attackTimeLeft > 0 && (attackTimeLeft / (ATTACK_DURATION / 4)) % 2 == 0) AnimatedSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                else AnimatedSprite.SpriteEffects = SpriteEffects.None;
            }

            if (deathTimeLeft > 0)
            {
                deathTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                shader.Parameters["destroyInterval"].SetValue((float)deathTimeLeft / DEATH_DURATION);
                if (deathTimeLeft <= 0) { deathTimeLeft = -1; }
            }
            else if (deathTimeLeft == -1)
            {
                if (!Busy) Terminate();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawShadow(spriteBatch);
        }

        public void DrawShader(SpriteBatch spriteBatch)
        {
            if (Transitioning) return;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, shader, null);
            AnimatedSprite.Draw(spriteBatch, Bottom + (attackTimeLeft > 0 ? new Vector2(0, 8) : Vector2.Zero), null, Depth);
            spriteBatch.End();
        }

        public override void StartTurn()
        {
            base.StartTurn();

            if (Confusion)
            {
                bool resist = stats.MaxHealth.Value >= 300;
                if (Rng.RandomInt(0, 2) + (resist ? 1 : 0)  >= 2)
                {
                    Confusion = false;

                    BattleController battleController = new BattleController(battleScene, this, this, new string[] { "Dialogue $attackerName recovered from confusion." });
                    ExecuteAction(battleController);
                }
                else
                {
                    BattleController preController = new BattleController(battleScene, this, this, new string[] { "Dialogue $attackerName lashed out in confusion!" });
                    ExecuteAction(preController);

                    BattleController battleController = new BattleController(battleScene, this, this, EnemyRecord.Attacks[0].Script);
                    ExecuteAction(battleController);

                    EndTurn();

                    return;
                }
            }

            Dictionary<AttackData, double> attacks = EnemyRecord.Attacks.ToDictionary(x => x, x => (double)x.Weight);
            AttackData attack = Rng.WeightedEntry<AttackData>(attacks);

            BattleController prescriptController = null;
            if (attack.PreScript != null)
            {
                prescriptController = new BattleController(battleScene, this, null, attack.PreScript);
                ExecuteAction(prescriptController);
            }

            if (prescriptController != null)
            {
                prescriptController.OnStart += new TerminationFollowup(() =>
                {
                    if (attack.AttackAll)
                    {
                        int delay = 0;
                        foreach (var player in battleScene.PlayerList.FindAll(x => !x.Dead))
                        {
                            string[] script = new string[attack.Script.Count() + 1];
                            script[0] = "Wait " + delay;
                            for (int i = 1; i < script.Count(); i++) script[i] = attack.Script[i - 1];
                            delay += 200;

                            BattleController battleController = new BattleController(battleScene, this, player, script);
                            battleScene.AddController(battleController);
                        }
                    }
                    else
                    {
                        BattleController battleController = new BattleController(battleScene, this, null, attack.Script);
                        ExecuteAction(battleController);
                    }
                });
            }
            else
            {
                if (attack.AttackAll)
                {
                    int delay = 0;
                    foreach (var player in battleScene.PlayerList.FindAll(x => !x.Dead))
                    {
                        string[] script = new string[attack.Script.Count() + 1];
                        script[0] = "Wait " + delay;
                        for (int i = 1; i < script.Count(); i++) script[i] = attack.Script[i - 1];
                        delay += 200;

                        BattleController battleController = new BattleController(battleScene, this, player, script);
                        battleScene.AddController(battleController);
                    }
                }
                else
                {
                    BattleController battleController = new BattleController(battleScene, this, null, attack.Script);
                    ExecuteAction(battleController);
                }
            }    

            EndTurn();
        }

        public override void Damage(int damage)
        {
            base.Damage(damage);

            if (Dead)
            {
                deathTimeLeft = DEATH_DURATION;
                Audio.PlaySound(GameSound.EnemyDeath);
            }
        }

        public override void Animate(string animationName)
        {
            switch (animationName)
            {
                case "Attack": attackTimeLeft = ATTACK_DURATION; break;
            }
        }

        public Rectangle EnemySize { get => new Rectangle(0, 0, AnimatedSprite.SpriteBounds().Width, AnimatedSprite.SpriteBounds().Height); }

        public override bool Busy { get => base.Busy || ParticleList.Count > 0 || deathTimeLeft > 0 || Transitioning || fadeInTime < FADE_IN_DURATION; }

        public override bool Transitioning { get => GetParent<Panel>().Transitioning; }

        public int ShadowOffset { get => EnemyRecord.ShadowOffset; }
    }
}
