﻿using EtrianLike.Main;
using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using EtrianLike.Scenes.StatusScene;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.TitleScene
{
    public class SaveModel
    {
        public ModelProperty<string> Header { get; set; }
        public ModelProperty<string> PlayerLocation { get; set; }
        public ModelProperty<int> SaveSlot { get; set; }
        public ModelProperty<string> Date { get; set; }
    }

    public class TitleViewModel : ViewModel
    {
        public static readonly Dictionary<string, Animation> HERO_ANIMATIONS = new Dictionary<string, Animation>()
        {
            { "Idle", new Animation(0, 0, 24, 32, 4, 400) }
        };

        private ViewModel settingsViewModel;

        public ModelCollection<SaveModel> AvailableSaves { get; set; } = new ModelCollection<SaveModel>();

        private DataGrid dataGrid;
        private int slot = -1;

        public TitleViewModel(Scene iScene, GameView viewName)
            : base(iScene, PriorityLevel.GameLevel)
        {
            GameProfile.NewState();

            /*
            GameProfile.PlayerProfile.WindowStyle.Value = "TechWindow";
            GameProfile.PlayerProfile.FrameStyle.Value = "TechFrame";
            GameProfile.PlayerProfile.SelectedStyle.Value = "TechSelected";
            GameProfile.PlayerProfile.FrameSelectedStyle.Value = "TechFrameSelected";
            GameProfile.PlayerProfile.LabelStyle.Value = "TechLabel";
            GameProfile.PlayerProfile.Font.Value = GameFont.DotFont;
            */

            var saves = GameProfile.GetAllSaveData();
            foreach (var saveEntry in saves)
            {
                var save = saveEntry.Value;
                AvailableSaves.Add(new SaveModel()
                {
                    Header = new ModelProperty<string>(((HeroModel)save["PartyLeader"]).Name + " L5"),
                    PlayerLocation = new ModelProperty<string>((string)save["PlayerLocation"]),
                    SaveSlot = new ModelProperty<int>(saveEntry.Key),
                    Date = new ModelProperty<string>(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString())
                });
            }

            LoadView(GameView.TitleScene_TitleView);

            dataGrid = GetWidget<DataGrid>("SaveList");
            if (AvailableSaves.Count() > 0)
            {
                slot = 0;
                (dataGrid.ChildList[slot] as Button).RadioSelect();
            }
            else
            {
                GetWidget<Button>("NewGame").RadioSelect();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.CurrentInput.CommandPressed(Command.Up))
            {
                if (slot == -1) return;
                slot--;

                Audio.PlaySound(GameSound.menu_select);

                if (slot > -1) (dataGrid.ChildList[slot] as Button).RadioSelect();
                else
                {
                    (dataGrid.ChildList[0] as Button).UnSelect();
                    GetWidget<Button>("NewGame").RadioSelect();
                }
            }
            else if (Input.CurrentInput.CommandPressed(Command.Down))
            {
                if (slot == AvailableSaves.Count() - 1) return;
                slot++;

                Audio.PlaySound(GameSound.menu_select);

                if (slot > -1)
                {
                    GetWidget<Button>("NewGame").UnSelect();
                    (dataGrid.ChildList[slot] as Button).RadioSelect();
                }
                else
                {
                    (dataGrid.ChildList[0] as Button).UnSelect();
                    GetWidget<Button>("NewGame").RadioSelect();
                }
            }
            else if (Input.CurrentInput.CommandPressed(Command.Confirm))
            {
                Audio.PlaySound(GameSound.Cursor);

                if (slot == -1) NewGame();
                else Continue(slot);
            }
        }

        public void NewGame()
        {
            foreach (var widget in dataGrid.ChildList)
            {
                (widget as Button).UnSelect();
            }

            SplashScene.SplashScene.NewGame();
        }

        public void Continue(object saveSlot)
        {
            GetWidget<Button>("NewGame").UnSelect();

            GameProfile.LoadState("Save" + saveSlot.ToString() + ".sav");

            string mapName = GameProfile.GetSaveData<string>("LastMap");
            int roomX = GameProfile.GetSaveData<int>("LastRoomX");
            int roomY = GameProfile.GetSaveData<int>("LastRoomY");
            MapScene.MapScene.Direction direction = GameProfile.GetSaveData<MapScene.MapScene.Direction>("LastDirection");

            CrossPlatformGame.Transition(typeof(MapScene.MapScene), mapName, roomX, roomY, direction);
        }

        public void SettingsMenu()
        {
            settingsViewModel = new SettingsViewModel(parentScene, GameView.TitleScene_SettingsView);
            parentScene.AddOverlay(settingsViewModel);
        }

        public void Credits()
        {
            //CrossPlatformGame.Transition(typeof(CreditsScene.CreditsScene));
            settingsViewModel = new CreditsScene.CreditsViewModel(parentScene, GameView.CreditsScene_CreditsView);
            parentScene.AddOverlay(settingsViewModel);
        }

        public void Exit()
        {
            Settings.SaveSettings();

            CrossPlatformGame.GameInstance.Exit();
        }

        public override void Terminate()
        {
            base.Terminate();

            settingsViewModel.Terminate();
        }

        public ModelProperty<bool> SaveAvailable { get; set; } = new ModelProperty<bool>(false);
    }
}
