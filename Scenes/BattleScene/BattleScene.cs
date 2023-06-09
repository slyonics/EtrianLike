﻿using EtrianLike.Models;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.BattleScene
{
    public class BattleScene : Scene
    {
        public static List<EnemyRecord> ENEMIES { get; private set; }
        public static List<EncounterRecord> ENCOUNTERS { get; private set; }

        private static readonly Rectangle[] BACKGROUND_SOURCE = new Rectangle[]
        {
            new Rectangle(0, 0, 16, 112),
            new Rectangle(16, 0, 16, 112),
            new Rectangle(32, 0, 16, 112),
        };

        public EncounterRecord encounterRecord;

        private BattleViewModel battleViewModel;

        private List<Battler> initiativeList = new List<Battler>();

        private bool introFinished = false;

        private GameMusic mapMusic;


        protected List<Particle> overlayParticleList = new List<Particle>();

        public BattleScene(string encounterName)
        {
            encounterRecord = ENCOUNTERS.First(x => x.Name == encounterName);

            string[] enemyTokens = encounterRecord.Enemies;
            List<Texture2D> enemySpriteList = new List<Texture2D>();
            int totalEnemyWidth = 0;
            foreach (string enemyName in enemyTokens)
            {
                EnemyRecord enemyData = ENEMIES.First(x => x.Name == enemyName);
                Texture2D enemySprite = AssetCache.SPRITES[(GameSprite)Enum.Parse(typeof(GameSprite), "Enemies_" + enemyData.Sprite)];
                totalEnemyWidth += enemySprite.Width;
            }


            battleViewModel = AddView(new BattleViewModel(this, ENCOUNTERS.First(x => x.Name == encounterName)));



            List<Battler> battlerList = new List<Battler>();
            battlerList.AddRange(PlayerList);
            battlerList.AddRange(EnemyList);
            foreach (Battler battler in battlerList) battler.Initiative = (int)(battler.BiggestStat * Rng.RandomDouble(0.75, 1.25));
            foreach (Battler battler in battlerList.OrderByDescending(x => x.Initiative)) EnqueueInitiative(battler);
        }

        public override void BeginScene()
        {
            sceneStarted = true;

            mapMusic = Audio.CurrentMusic;
            Audio.PlayMusic(GameMusic.Battle);
        }

        public override void EndScene()
        {
            Audio.PlayMusic(mapMusic);

            foreach (ModelProperty<StatusScene.HeroModel> heroModel in GameProfile.PlayerProfile.Party)
            {
                heroModel.Value.Health.Value = heroModel.Value.MaxHealth.Value;
            }

            base.EndScene();
        }

        public static void Initialize()
        {
            ENEMIES = AssetCache.LoadRecords<EnemyRecord>("EnemyData");
            ENCOUNTERS = AssetCache.LoadRecords<EncounterRecord>("EncounterData");

            BattleEnemy.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (PriorityLevel == PriorityLevel.TransitionLevel) return;
            if (battleViewModel.Closed) return;

            int i = 0;
            while (i < overlayParticleList.Count) { overlayParticleList[i].Update(gameTime); i++; }
            overlayParticleList.RemoveAll(x => x.Terminated);

            if (battleViewModel.EnemyPanel.Transitioning) return;
            else if (!introFinished)
            {
                introFinished = true;
                var convoRecord = new ConversationScene.ConversationRecord()
                {
                    DialogueRecords = new ConversationScene.DialogueRecord[] {
                            new ConversationScene.DialogueRecord() { Text = encounterRecord.Intro }
                        }
                };
                var convoScene = new ConversationScene.ConversationScene(convoRecord, ConversationScene.ConversationViewModel.CONVO_BOUNDS);
                CrossPlatformGame.StackScene(convoScene);
            }

            EnemyList.RemoveAll(x => x.Terminated);
            if (!controllerList.Any(y => y.Any(x => x is BattleController)) && !PlayerList.Any(x => x.Busy) && !EnemyList.Any(x => x.Busy) && CrossPlatformGame.CurrentScene == this)
            {
                if (EnemyList.Count == 0)
                {
                    Audio.PlayMusic(GameMusic.Victory, false);

                    List<ConversationScene.DialogueRecord> dialogueRecords = new List<ConversationScene.DialogueRecord>();
                    dialogueRecords.Add(new ConversationScene.DialogueRecord() { Text = "Victory!" });
                    foreach (BattlePlayer battlePlayer in PlayerList)
                    {
                        if (!battlePlayer.Dead)
                        {
                            List<ConversationScene.DialogueRecord> reports = battlePlayer.GrowAfterBattle(encounterRecord);
                            foreach (ConversationScene.DialogueRecord report in reports) dialogueRecords.Add(report);
                        }

                        foreach (var equipment in battlePlayer.HeroModel.Equipment)
                        {
                            if (equipment.Value.ItemType != StatusScene.ItemType.Consumable) equipment.Value.ChargesLeft = equipment.Value.Charges;
                        }
                    }
                    var convoRecord = new ConversationScene.ConversationRecord()
                    {
                        DialogueRecords = dialogueRecords.ToArray()
                    };

                    var convoScene = new ConversationScene.ConversationScene(convoRecord, ConversationScene.ConversationViewModel.CONVO_BOUNDS);
                    convoScene.OnTerminated += new TerminationFollowup(battleViewModel.Close);
                    CrossPlatformGame.StackScene(convoScene);
                }
                else if (PlayerList.All(x => x.Dead))
                {
                    GameProfile.SetSaveData<bool>("Wipeout", true);
                    PickReviveConvo();

                    foreach (BattlePlayer battlePlayer in PlayerList)
                    {
                        bool isProtagonist = (battlePlayer == PlayerList[0]);

                        foreach (var equipment in battlePlayer.HeroModel.Equipment)
                        {
                            if (equipment.Value.ItemType != StatusScene.ItemType.Consumable) equipment.Value.ChargesLeft = equipment.Value.Charges;

                            // if (!isProtagonist) GameProfile.Inventory.Add(equipment.Value);
                        }
                    }

                    GameProfile.Inventory.ModelList.Clear();
                    GameProfile.PlayerProfile.Party.ModelList.Clear();
                    GameProfile.PlayerProfile.Party.ModelList.Add(new ModelProperty<StatusScene.HeroModel>(PlayerList[0].Stats as StatusScene.HeroModel));
                    //for (int j = 1; j <= 10; j++) GameProfile.SetSaveData<bool>("JunkChest" + j + "Opened", false);
                    foreach (var keyValuePair in GameProfile.SaveData)
                    {
                        if (keyValuePair.Key.EndsWith("ChestOpened"))
                        {
                            GameProfile.SaveData[keyValuePair.Key] = false;
                        }
                    }

                    string narration = "Despite their best effort, the story of the Sparkle Tales ended here...";

                    var convoRecord = new ConversationScene.ConversationRecord()
                    {
                        DialogueRecords = new ConversationScene.DialogueRecord[]
                        {
                            new ConversationScene.DialogueRecord() { Text = narration, Script = new string[] { "Wait 2000", "ChangeScene EtrianLike.Scenes.TitleScene.TitleScene" } }
                        }
                    };
                    var convoScene = new ConversationScene.ConversationScene(convoRecord, ConversationScene.ConversationViewModel.CONVO_BOUNDS, 3500);
                    CrossPlatformGame.StackScene(convoScene);
                }
                else ActivateNextBattler();
            }
        }

        public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D pixelRender, RenderTarget2D compositeRender)
        {
            graphicsDevice.SetRenderTarget(pixelRender);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, null);
            DrawBackground(spriteBatch);
            spriteBatch.End();

            Matrix matrix = (Camera == null) ? Matrix.Identity : Camera.Matrix;
            Effect shader = (spriteShader == null) ? null : spriteShader.Effect;
            foreach (Entity entity in entityList) entity.DrawShader(spriteBatch, Camera, matrix);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, shader, matrix);
            DrawGame(spriteBatch, shader, matrix);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, null);
            DrawOverlay(spriteBatch);
            spriteBatch.End();

            foreach (BattleEnemy battleEnemy in EnemyList) battleEnemy.DrawShader(spriteBatch);
            foreach (BattlePlayer battlePlayer in PlayerList) battlePlayer.DrawShader(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, null);
            foreach (Particle particle in overlayParticleList) particle.Draw(spriteBatch, null);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(compositeRender);

            if (!CrossPlatformGame.ClearedCompositeRender)
            {
                CrossPlatformGame.ClearedCompositeRender = true;
                graphicsDevice.Clear(Color.Transparent);
            }

            shader = (SceneShader == null) ? null : SceneShader.Effect;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, shader, Matrix.Identity);
            spriteBatch.Draw(pixelRender, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        public void AddBattler(Battler battler)
        {
            if (battler is BattleEnemy) EnemyList.Add(battler as BattleEnemy);
            else if (battler is BattlePlayer) PlayerList.Add(battler as BattlePlayer);
        }

        public override T AddParticle<T>(T newParticle)
        {
            if (newParticle.Foreground) overlayParticleList.Add(newParticle);
            else particleList.Add(newParticle);
            return newParticle;
        }

        public void ActivateNextBattler()
        {
            Battler readyBattler = initiativeList.First();
            int timeAdvance = readyBattler.ActionTime;
            foreach (Battler battler in initiativeList) battler.ActionTime -= timeAdvance;
            readyBattler.StartTurn();
        }

        public void EnqueueInitiative(Battler battler)
        {
            if (initiativeList.Count == 0)
            {
                initiativeList.Add(battler);
                return;
            }

            initiativeList.Remove(battler);
            int insertIndex = initiativeList.FindLastIndex(x => x.ActionTime <= battler.ActionTime);
            initiativeList.Insert(Math.Max(insertIndex + 1, initiativeList.Count - 1), battler);
        }

        private void PickReviveConvo()
        {
            if (GameProfile.PlayerProfile.Party.Count() > 1 && !GameProfile.GetSaveData<bool>("PartyResetExplained"))
            {
                GameProfile.SetSaveData<bool>("PartyResetExplained", true);
                GameProfile.SetSaveData<string>("ReviveConvo", "WhereMyPartyRevive");
                return;
            }

            if (GameProfile.PlayerProfile.Party.Count() == 1 && !GameProfile.GetSaveData<bool>("SoloWarningExplained"))
            {
                GameProfile.SetSaveData<bool>("SoloWarningExplained", true);
                GameProfile.SetSaveData<string>("ReviveConvo", "SoloRevive");
                return;
            }

            GameProfile.SetSaveData<string>("ReviveConvo", "DefaultRevive");
        }

        public List<Battler> InitiativeList { get => initiativeList; }
        public List<BattlePlayer> PlayerList { get; } = new List<BattlePlayer>();
        public List<BattleEnemy> EnemyList { get; } = new List<BattleEnemy>();
        public BattleViewModel BattleViewModel { get => battleViewModel; }
    }
}
