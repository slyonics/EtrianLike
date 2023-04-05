using EtrianLike.Main;
using EtrianLike.Models;
using EtrianLike.SceneObjects.Controllers;
using EtrianLike.Scenes.StatusScene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.SplashScene
{
    public class SplashScene : Scene, ISkippableWait
    {
        private Texture2D splashSprite = AssetCache.SPRITES[GameSprite.Background_Splash];

        public SplashScene()
            : base()
        {
            AddController(new SkippableWaitController(PriorityLevel.MenuLevel, this));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(splashSprite, new Rectangle(0, 0, CrossPlatformGame.ScreenWidth, CrossPlatformGame.ScreenHeight), new Rectangle(0, 0, 1, 1), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(splashSprite, new Rectangle((CrossPlatformGame.ScreenWidth - splashSprite.Width) / 2, (CrossPlatformGame.ScreenHeight - splashSprite.Height) / 2, splashSprite.Width, splashSprite.Height), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }

        public void Notify(SkippableWaitController sender)
        {
            if (GameProfile.SaveList.Count > 0) CrossPlatformGame.Transition(typeof(TitleScene.TitleScene));
            else NewGame();
        }

        public static void NewGame()
        {
            GameProfile.NewState();

            GameProfile.SetSaveData<int>("RandomBattle", 6);

            /*
            GameProfile.PlayerProfile.Party.Add(new HeroModel(HeroType.Jenna));
            GameProfile.PlayerProfile.Party.Add(new HeroModel(HeroType.Stephan));
            */


            // GameProfile.SetSaveData<HeroModel>("PartyLeader", GameProfile.PlayerProfile.Party.First().Value);

            // CrossPlatformGame.Transition(typeof(MapScene.MapScene), "Overworld", 5, 7, SceneObjects.Maps.Orientation.Up);

            // CrossPlatformGame.Transition(typeof(MapScene.MapScene), "SchoolOrigin");


            GameProfile.PlayerProfile.Party.Add(new HeroModel(HeroType.Cyra));
            GameProfile.PlayerProfile.Party.Add(new HeroModel(HeroType.Angélia));
            GameProfile.PlayerProfile.Party.Add(new HeroModel(HeroType.Wren));

            GameProfile.SetSaveData<HeroModel>("PartyLeader", GameProfile.PlayerProfile.Party.First().Value);

            // CrossPlatformGame.Transition(typeof(IntroScene.IntroScene));

            CrossPlatformGame.Transition(typeof(MapScene.MapScene), "SchoolHall");
        }

        public bool Terminated { get => false; }
    }
}
