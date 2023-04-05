using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.StatusScene
{
    public class SystemViewModel : ViewModel, IStatusSubView
    {
        StatusScene statusScene;

        public ViewModel ChildViewModel { get; set; }

        private int saveSlot = -1;

        public ModelCollection<TitleScene.SaveModel> AvailableSaves { get; set; } = new ModelCollection<TitleScene.SaveModel>();

        public bool SuppressCancel { get; set; }

        public SystemViewModel(StatusScene iScene)
            : base(iScene, PriorityLevel.GameLevel)
        {
            statusScene = iScene;

            var saves = GameProfile.GetAllSaveData();
            for (int i = 0; i < 4; i++)
            {
                if (saves.ContainsKey(i))
                {
                    var save = saves[i];
                    AnimatedSprite animatedSprite = new AnimatedSprite(AssetCache.SPRITES[((HeroModel)save["PartyLeader"]).Sprite.Value], StatusViewModel.HERO_ANIMATIONS);
                    AvailableSaves.Add(new TitleScene.SaveModel()
                    {
                        Header = new ModelProperty<string>(GameProfile.PlayerProfile.Party[0].Name.Value),
                        PlayerLocation = new ModelProperty<string>((string)save["PlayerLocation"]),
                        SaveSlot = new ModelProperty<int>(i),
                        Date = new ModelProperty<string>(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()),
                    });
                }
                else
                {
                    HeroModel heroModel = new HeroModel(HeroType.Cyra);
                    heroModel.Name.Value = "- Empty Save -";
                    AnimatedSprite animatedSprite = new AnimatedSprite(AssetCache.SPRITES[GameSprite.Actors_Blank], StatusViewModel.HERO_ANIMATIONS);
                    AvailableSaves.Add(new TitleScene.SaveModel()
                    {
                        Header = new ModelProperty<string>(heroModel.Name.Value),
                        PlayerLocation = new ModelProperty<string>(""),
                        SaveSlot = new ModelProperty<int>(i),
                        Date = new ModelProperty<string>(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()),
                    });
                }
            }

            LoadView(GameView.StatusScene_SystemView);

            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputFrame currentInput = Input.CurrentInput;
            if (currentInput.CommandPressed(Command.Up)) CursorUp();
            else if (currentInput.CommandPressed(Command.Down)) CursorDown();
            else if (currentInput.CommandPressed(Command.Confirm))
            {
                Audio.PlaySound(GameSound.Cursor);
                if (saveSlot == 4) Exit();
                else if (saveSlot == -1)
                {
                    saveSlot = 0;
                    (GetWidget<DataGrid>("Saves").ChildList[saveSlot] as Button).RadioSelect();
                }
                else Save(saveSlot);
            }
        }

        private void CursorUp()
        {
            if (saveSlot == -1) return;

            saveSlot--;
            if (saveSlot < 0)
            {
                saveSlot = 0;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            (GetWidget<DataGrid>("Saves").ChildList[saveSlot] as Button).RadioSelect();
            GetWidget<Button>("Exit").UnSelect();
        }

        private void CursorDown()
        {
            saveSlot++;

            if (saveSlot >= 5)
            {
                saveSlot = 4;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            if (saveSlot == 4)
            {
                GetWidget<Button>("Exit").RadioSelect();
                (GetWidget<DataGrid>("Saves").ChildList[3] as Button).UnSelect();
            }
            else (GetWidget<DataGrid>("Saves").ChildList[saveSlot] as Button).RadioSelect();
        }

        public void Exit()
        {
            GetWidget<Button>("Exit").UnSelect();

            CrossPlatformGame.Transition(typeof(TitleScene.TitleScene));
        }

        public void Save(object parameter)
        {
            if (parameter is IModelProperty)
            {
                saveSlot = (int)((IModelProperty)parameter).GetValue();
            }
            else saveSlot = (int)parameter;
            GameProfile.SaveSlot = saveSlot;

            if (AvailableSaves[saveSlot].Header.Value == "- Empty Save -")
            {
                ((MapScene.MapScene)CrossPlatformGame.SceneStack.First(x => x is MapScene.MapScene)).SaveData();
                GameProfile.SaveState();

                var save = GameProfile.SaveData;

                AvailableSaves[saveSlot].PlayerLocation.Value = (string)save["PlayerLocation"];
                AvailableSaves[saveSlot].Date.Value = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            }
            else
            {
                var dialogueRecords = new List<ConversationScene.DialogueRecord>();

                dialogueRecords.Add(new ConversationScene.DialogueRecord()
                {
                    Text = "Overwrite existing save?",
                    Script = new string[] { "DisableEnd", "WaitForText", "SelectionPrompt", "No", "Yes", "End", "Switch $selection", "Case No", "EndConversation", "Break", "Case Yes", "EndConversation", "Break", "End" }
                });

                var convoRecord = new ConversationScene.ConversationRecord()
                {
                    DialogueRecords = dialogueRecords.ToArray()
                };
                var convoScene = new ConversationScene.ConversationScene(convoRecord);
                CrossPlatformGame.StackScene(convoScene, true);
                convoScene.OnTerminated += new TerminationFollowup(() =>
                {
                    if (GameProfile.GetSaveData<string>("LastSelection") == "Yes")
                    {
                        ((MapScene.MapScene)CrossPlatformGame.SceneStack.First(x => x is MapScene.MapScene)).SaveData();
                        GameProfile.SaveState();

                        var save = GameProfile.SaveData;

                        AvailableSaves[saveSlot].PlayerLocation.Value = (string)save["PlayerLocation"];
                        AvailableSaves[saveSlot].Date.Value = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                    }
                });
            }
        }

        public void ResetSlot()
        {
            if (saveSlot != 4 && saveSlot != -1) (GetWidget<DataGrid>("Saves").ChildList[saveSlot] as Button).UnSelect();
            GetWidget<Button>("Exit").UnSelect();
            saveSlot = -1;
        }

        public void MoveAway()
        {

        }

        public bool SuppressLeftRight { get => false; }
    }
}
