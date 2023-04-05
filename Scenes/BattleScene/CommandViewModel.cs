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
    public class CommandViewModel : ViewModel
    {
        BattleScene battleScene;

        TargetViewModel targetViewModel;


        int category = -1;
        int slot = -1;


        public CommandViewModel(BattleScene iScene, BattlePlayer iBattlePlayer)
            : base(iScene, PriorityLevel.GameLevel)
        {
            battleScene = iScene;

            ActivePlayer = iBattlePlayer;

            LoadView(GameView.BattleScene_CommandView);


            SelectAbilities();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var input = Input.CurrentInput;
            if (targetViewModel == null)
            {
                if (input.CommandPressed(Command.Up)) CursorUp();
                else if (input.CommandPressed(Command.Down)) CursorDown();
                else if (input.CommandPressed(Command.Confirm)) CursorSelect();
            }
            else
            {
                if (input.CommandPressed(Command.Cancel) && slot == -1) slot = 0;
                if (targetViewModel.Terminated) targetViewModel = null;
            }
        }

        private void CursorUp()
        {
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
            if (slot == -1) return;
            CommandRecord record = (GetWidget<DataGrid>("CommandList").Items.ElementAt(slot) as IModelProperty).GetValue() as CommandRecord;
            if (!record.Usable) return;

            Audio.PlaySound(GameSound.menu_select);

            targetViewModel = new TargetViewModel(battleScene, ActivePlayer, record);
            battleScene.AddView(targetViewModel);
        }

        public void SelectAbilities()
        {
            if (category == 1) return;

            targetViewModel?.Terminate();
            targetViewModel = null;

            if (Input.MOUSE_MODE) slot = -1;

            List<ModelProperty<CommandRecord>> commands = new List<ModelProperty<CommandRecord>>();
            foreach (var command in ActivePlayer.HeroModel.Abilities.ModelList) commands.Add(new ModelProperty<CommandRecord>(command.Value as CommandRecord));
            AvailableCommands.ModelList = commands;
            ActivePlayer.HeroModel.LastCategory.Value = category = 1;

            Description1.Value = Description2.Value = Description3.Value = Description4.Value = Description5.Value = null;
        }

        public void SelectCommand(object parameter)
        {
            CommandRecord record;
            if (parameter is IModelProperty)
            {
                record = (CommandRecord)((IModelProperty)parameter).GetValue();
            }
            else record = (CommandRecord)parameter;

            targetViewModel?.Terminate();
            targetViewModel = null;

            if (Input.MOUSE_MODE) slot = -1;

            if (Input.MOUSE_MODE)
            {
                targetViewModel = new TargetViewModel(battleScene, ActivePlayer, record);
                battleScene.AddView(targetViewModel);
            }

            ActivePlayer.HeroModel.LastSlot.Value = slot = AvailableCommands.ToList().FindIndex(x => x.Value == record);


            Description1.Value = record.Description.ElementAtOrDefault(0);
            Description2.Value = record.Description.ElementAtOrDefault(1);
            Description3.Value = record.Description.ElementAtOrDefault(2);
            Description4.Value = record.Description.ElementAtOrDefault(3);
            Description5.Value = record.Description.ElementAtOrDefault(4);
        }


        public BattlePlayer ActivePlayer { get; set; }
        public ModelCollection<CommandRecord> AvailableCommands { get; set; } = new ModelCollection<CommandRecord>();

        public ModelProperty<string> Description1 { get; set; } = new ModelProperty<string>("");
        public ModelProperty<string> Description2 { get; set; } = new ModelProperty<string>("");
        public ModelProperty<string> Description3 { get; set; } = new ModelProperty<string>("");
        public ModelProperty<string> Description4 { get; set; } = new ModelProperty<string>("");
        public ModelProperty<string> Description5 { get; set; } = new ModelProperty<string>("");


        public Rectangle PortraitBounds { get; set; } = new Rectangle(CrossPlatformGame.ScreenWidth - 255, 20, 255, 500); 
    }
}
