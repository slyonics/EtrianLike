using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using EtrianLike.Scenes.StatusScene;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.BattleScene
{
    public class CategoryViewModel : ViewModel
    {
        BattleScene battleScene;

        ViewModel childViewModel;

        Button fightButton;
        Button skillsButton;
        Button fleeButton;

        int category = 0;
        int slot = -1;


        public CategoryViewModel(BattleScene iScene, BattlePlayer iBattlePlayer)
            : base(iScene, PriorityLevel.GameLevel)
        {
            battleScene = iScene;

            ActivePlayer = iBattlePlayer;

            LoadView(GameView.BattleScene_CategoryView);

            fightButton = GetWidget<Button>("Fight");
            skillsButton = GetWidget<Button>("Skills");
            fleeButton = GetWidget<Button>("Flee");

            GetWidget<Button>("Skills").Enabled = ActivePlayer.HeroModel.Abilities.Count() > 0;
            fleeButton.Enabled = battleScene.encounterRecord.CanFlee;


            fightButton.RadioSelect();
            Description.Value = "Attack an enemy with your equipped weapon.";

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var input = Input.CurrentInput;
            if (childViewModel == null)
            {
                if (input.CommandPressed(Command.Left)) CursorLeft();
                else if (input.CommandPressed(Command.Right)) CursorRight();
                else if (input.CommandPressed(Command.Up)) CursorUp();
                else if (input.CommandPressed(Command.Down)) CursorDown();
                else if (input.CommandPressed(Command.Confirm)) CursorSelect();
            }
            else
            {
                if (input.CommandPressed(Command.Cancel) && slot == -1) slot = 0;
                if (childViewModel.Terminated) childViewModel = null;
            }
        }

        private void CursorLeft()
        {
            switch (category)
            {
                case 1:
                    Audio.PlaySound(GameSound.menu_select);
                    Fight();
                    slot = 0;
                    category = 0;
                    fightButton.RadioSelect();
                    break;

                case 2:
                    if (skillsButton.Enabled)
                    {
                        Audio.PlaySound(GameSound.menu_select);
                        skillsButton.RadioSelect();
                        Skills();
                        slot = -1;
                        category = 1;
                    }
                    else
                    {
                        Audio.PlaySound(GameSound.menu_select);
                        Fight();
                        slot = 0;
                        category = 0;
                        fightButton.RadioSelect();
                    }
                    break;
            }
        }

        private void CursorRight()
        {
            switch (category)
            {
                case 0:
                    if (skillsButton.Enabled)
                    {
                        Audio.PlaySound(GameSound.menu_select);
                        skillsButton.RadioSelect();
                        Skills();
                        slot = -1;
                        category = 1;
                    }
                    else
                    {
                        Audio.PlaySound(GameSound.menu_select);
                        fleeButton.RadioSelect();
                        slot = -1;
                        category = 2; 
                        ShowSkills.Value = false;
                        Description.Value = "End the battle and continue exploring.";
                    }
                    break;

                case 1:
                    if (fleeButton.Enabled)
                    {
                        Audio.PlaySound(GameSound.menu_select);
                        fleeButton.RadioSelect();
                        slot = -1;
                        category = 2;
                        ShowSkills.Value = false;
                        Description.Value = "End the battle and continue exploring.";
                    }
                    break;
            }
        }

        private void CursorUp()
        {
            if (fightButton.Selected) return;

            Audio.PlaySound(GameSound.menu_select);

            if (slot == -1) slot = 0;
            else if (slot == 0) slot = AvailableCommands.Count() - 1;
            else slot--;

            (GetWidget<DataGrid>("CommandList").ChildList[slot] as Button).RadioSelect();
            SelectCommand(AvailableCommands.ElementAt(slot));
            ActivePlayer.HeroModel.LastSlot.Value = slot;
        }

        private void CursorDown()
        {
            if (fightButton.Selected) return;

            Audio.PlaySound(GameSound.menu_select);

            if (slot == -1) slot = 0;
            else if (slot == AvailableCommands.Count() - 1) slot = 0;
            else slot++;

            (GetWidget<DataGrid>("CommandList").ChildList[slot] as Button).RadioSelect();
            SelectCommand(AvailableCommands.ElementAt(slot));
            ActivePlayer.HeroModel.LastSlot.Value = slot;
        }

        private void CursorSelect()
        {
            if (fightButton.Selected)
            {
                childViewModel?.Terminate();
                childViewModel = new TargetViewModel(battleScene, ActivePlayer, ActivePlayer.HeroModel.Equipment.First().Value);
                battleScene.AddView(childViewModel);
                return;
            }

            if (fleeButton.Selected)
            {
                Flee();
                return;
            }

            if (slot == -1)
            {
                slot = 0;
                (GetWidget<DataGrid>("CommandList").ChildList[slot] as Button).RadioSelect();
                SelectCommand(AvailableCommands.ElementAt(slot));
                ActivePlayer.HeroModel.LastSlot.Value = slot;
                return;
            }
            CommandRecord record = (GetWidget<DataGrid>("CommandList").Items.ElementAt(slot) as IModelProperty).GetValue() as CommandRecord;
            if (!record.Usable) return;

            Audio.PlaySound(GameSound.menu_select);

            childViewModel = new TargetViewModel(battleScene, ActivePlayer, record);
            battleScene.AddView(childViewModel);
        }

        public void Fight()
        {
            category = 0;

            ShowSkills.Value = false;

            Description.Value = "Attack an enemy with your equipped weapon.";

            if (Input.MOUSE_MODE)
            {
                childViewModel?.Terminate();
                childViewModel = new TargetViewModel(battleScene, ActivePlayer, ActivePlayer.HeroModel.Equipment.First().Value);
                battleScene.AddView(childViewModel);
            }
        }

        public void Skills()
        {
            category = 1;

            Description.Value = "Spend MP to use a special ability.";

            childViewModel?.Terminate();
            childViewModel = null;

            List<ModelProperty<CommandRecord>> commands = new List<ModelProperty<CommandRecord>>();
            foreach (var command in ActivePlayer.HeroModel.Abilities.ModelList)
            {
                var c = command.Value as CommandRecord;
                c.Usable = ActivePlayer.HeroModel.Magic.Value >= c.Cost;
                commands.Add(new ModelProperty<CommandRecord>(c));
            }
            AvailableCommands.ModelList = commands;

            ShowSkills.Value = true;
        }

        public void Item()
        {
            Description.Value = "End the battle and continue exploring.";

            childViewModel?.Terminate();
            childViewModel = null;
        }

        public void Flee()
        {
            category = 2;

            ShowSkills.Value = false;

            Description.Value = "End the battle and continue exploring.";
            GetWidget<CrawlText>("Desc").FinishText();

            var dialogueRecords = new List<ConversationScene.DialogueRecord>();

            dialogueRecords.Add(new ConversationScene.DialogueRecord()
            {
                Text = "Flee from battle?",
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
                    Terminate();

                    var convoRecord = new ConversationScene.ConversationRecord()
                    {
                        DialogueRecords = new ConversationScene.DialogueRecord[] {
                                new ConversationScene.DialogueRecord() { Text = "Escaped from battle." }
                            }
                    };

                    var convoScene = new ConversationScene.ConversationScene(convoRecord, ConversationScene.ConversationViewModel.CONVO_BOUNDS);
                    convoScene.OnTerminated += battleScene.BattleViewModel.Close;
                    CrossPlatformGame.StackScene(convoScene);
                }
            });
        }

        public void SelectCommand(object parameter)
        {
            if (fightButton.Selected)
            {
                childViewModel?.Terminate();
                childViewModel = new TargetViewModel(battleScene, ActivePlayer, ActivePlayer.HeroModel.Equipment.First().Value);
                battleScene.AddView(childViewModel);
            }
            else
            {
                CommandRecord record;
                if (parameter is IModelProperty)
                {
                    record = (CommandRecord)((IModelProperty)parameter).GetValue();
                }
                else record = (CommandRecord)parameter;

                childViewModel?.Terminate();
                childViewModel = null;

                if (Input.MOUSE_MODE) slot = -1;

                if (Input.MOUSE_MODE)
                {
                    childViewModel = new TargetViewModel(battleScene, ActivePlayer, record);
                    battleScene.AddView(childViewModel);
                }

                ActivePlayer.HeroModel.LastSlot.Value = slot = AvailableCommands.ToList().FindIndex(x => x.Value == record);

                Description.Value = record.Description;

            }
        }


        public BattlePlayer ActivePlayer { get; set; }
        public ModelCollection<CommandRecord> AvailableCommands { get; set; } = new ModelCollection<CommandRecord>();
        public ModelProperty<bool> ShowSkills { get; set; } = new ModelProperty<bool>(false);

        public ModelProperty<string> Description { get; set; } = new ModelProperty<string>("");


        public Rectangle SkillBounds { get; set; } = new Rectangle(CrossPlatformGame.ScreenWidth / 2 - 248, -60, 230, 240);


    }
}
