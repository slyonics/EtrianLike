﻿using EtrianLike.Main;
using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.ConversationScene
{
    public class SelectionViewModel : ViewModel
    {
        private ConversationScene conversationScene;

        private int selection = -1;

        public SelectionViewModel(Scene iScene, List<string> options, bool showMoney = false)
            : base(iScene, PriorityLevel.MenuLevel)
        {
            conversationScene = iScene as ConversationScene;

            int longestOption = 0;
            foreach (string option in options)
            {
                AvailableOptions.Add(option);
                int optionLength = Text.GetStringLength(GameFont.Dialogue, option);
                if (optionLength > longestOption) longestOption = optionLength;
            }
            int width = longestOption + 14;
            ButtonSize.Value = new Rectangle(0, 0, longestOption + 6, Text.GetStringHeight(GameFont.Dialogue));
            LabelSize.Value = new Rectangle(0, -2, longestOption + 6, ButtonSize.Value.Height);
            WindowSize.Value = new Rectangle(340 - width, 150 - (ButtonSize.Value.Height * options.Count() + 8), width, ButtonSize.Value.Height * options.Count() + 8);

            ShowMoney.Value = showMoney;

            LoadView(GameView.ConversationScene_SelectionView);

            if (!Input.MOUSE_MODE)
            {
                selection = 0;
                (GetWidget<DataGrid>("OptionsList").ChildList[selection] as Button).RadioSelect();
                SelectOption(AvailableOptions.ElementAt(selection));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var input = Input.CurrentInput;
            if (input.CommandPressed(Command.Up)) CursorUp();
            else if (input.CommandPressed(Command.Down)) CursorDown();
            else if (input.CommandPressed(Command.Confirm) && selection != -1)
            {
                Audio.PlaySound(GameSound.Cursor);
                Terminate();
            }
        }

        private void CursorUp()
        {
            if (AvailableOptions.Count() == 0) return;

            Audio.PlaySound(GameSound.menu_select);

            if (selection == -1) selection = 0;
            else if (selection == 0) selection = AvailableOptions.Count() - 1;
            else selection--;

            (GetWidget<DataGrid>("OptionsList").ChildList[selection] as Button).RadioSelect();
            SelectOption(AvailableOptions.ElementAt(selection));
        }

        private void CursorDown()
        {
            if (AvailableOptions.Count() == 0) return;

            Audio.PlaySound(GameSound.menu_select);

            if (selection == -1) selection = 0;
            else if (selection == AvailableOptions.Count() - 1) selection = 0;
            else selection++;

            (GetWidget<DataGrid>("OptionsList").ChildList[selection] as Button).RadioSelect();
            SelectOption(AvailableOptions.ElementAt(selection));
        }

        public override void Terminate()
        {
            //conversationScene.ConversationViewModel.Proceed();
            base.Terminate();
        }

        public void SelectOption(object parameter)
        {
            GameProfile.SetSaveData<string>("LastSelection", parameter.ToString());
            if (Input.MOUSE_MODE) Terminate();
        }

        public ModelCollection<string> AvailableOptions { get; set; } = new ModelCollection<string>();

        public ModelProperty<Rectangle> WindowSize { get; set; } = new ModelProperty<Rectangle>(new Rectangle(-120, 20, 240, 60));
        public ModelProperty<Rectangle> ButtonSize { get; set; } = new ModelProperty<Rectangle>(new Rectangle(-120, 20, 240, 60));
        public ModelProperty<Rectangle> LabelSize { get; set; } = new ModelProperty<Rectangle>(new Rectangle(-120, 20, 240, 60));

        public ModelProperty<bool> ShowMoney { get; set; } = new ModelProperty<bool>(true);
    }
}
