﻿using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.StatusScene
{
    public class AbilitiesViewModel : ViewModel, IStatusSubView
    {
        StatusScene statusScene;

        private int partySlot = -1;
        private int abilitySlot = -1;

        public ViewModel ChildViewModel { get; set; }

        public bool SuppressCancel { get; set; }

        public ModelCollection<PartyMemberModel> PartyMembers { get; private set; }
        public ModelCollection<AbilityRecord> AbilitiesList { get; private set; } = new ModelCollection<AbilityRecord>();

        public AbilitiesViewModel(StatusScene iScene, ModelCollection<PartyMemberModel> iPartyMembers)
            : base(iScene, PriorityLevel.GameLevel)
        {
            statusScene = iScene;

            PartyMembers = iPartyMembers;

            LoadView(GameView.StatusScene_AbilitiesView);

            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SuppressCancel = false;

            if (ShowDescription.Value)
            {
                if (Input.CurrentInput.CommandPressed(Command.Up)) AbilityCursorUp();
                else if (Input.CurrentInput.CommandPressed(Command.Down)) AbilityCursorDown();
                else if (Input.CurrentInput.CommandPressed(Command.Cancel))
                {
                    Audio.PlaySound(GameSound.Back);
                    (GetWidget<DataGrid>("AbilitiesList").ChildList[abilitySlot] as Button).UnSelect();
                    abilitySlot = -1;
                    SuppressCancel = true;
                    ShowDescription.Value = false;
                }
            }
            else
            {
                if (Input.CurrentInput.CommandPressed(Command.Up)) PartyCursorUp();
                else if (Input.CurrentInput.CommandPressed(Command.Down)) PartyCursorDown();
                else if (Input.CurrentInput.CommandPressed(Command.Confirm))
                {
                    if (partySlot == -1)
                    {
                        Audio.PlaySound(GameSound.Cursor);
                        partySlot = 0;
                        AbilitiesList.ModelList = PartyMembers[partySlot].HeroModel.Value.Abilities.ModelList;
                        ShowAbilities.Value = true;
                        abilitySlot = -1;
                        (GetWidget<DataGrid>("PartyList").ChildList[partySlot] as Button).RadioSelect();
                    }
                    else if (PartyMembers[partySlot].HeroModel.Value.Abilities.Count() > 0)
                    {
                        Audio.PlaySound(GameSound.Cursor);
                        SelectParty(PartyMembers[partySlot].HeroModel);
                    }
                    else Audio.PlaySound(GameSound.Error);
                }
            }
        }

        private void PartyCursorUp()
        {
            partySlot--;
            if (partySlot < 0)
            {
                partySlot = 0;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            AbilitiesList.ModelList = PartyMembers[partySlot].HeroModel.Value.Abilities.ModelList;

            ShowAbilities.Value = true;

            abilitySlot = -1;

            (GetWidget<DataGrid>("PartyList").ChildList[partySlot] as Button).RadioSelect();
        }

        private void PartyCursorDown()
        {
            partySlot++;
            if (partySlot >= GameProfile.PlayerProfile.Party.Count())
            {
                partySlot = GameProfile.PlayerProfile.Party.Count() - 1;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            AbilitiesList.ModelList = PartyMembers[partySlot].HeroModel.Value.Abilities.ModelList;

            ShowAbilities.Value = true;

            abilitySlot = -1;

            (GetWidget<DataGrid>("PartyList").ChildList[partySlot] as Button).RadioSelect();
        }

        private void AbilityCursorUp()
        {
            abilitySlot--;
            if (abilitySlot < 0)
            {
                abilitySlot = 0;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            SelectAbility(PartyMembers[partySlot].HeroModel.Value.Abilities[abilitySlot]);
            (GetWidget<DataGrid>("AbilitiesList").ChildList[abilitySlot] as Button).RadioSelect();
        }

        private void AbilityCursorDown()
        {
            abilitySlot++;
            if (abilitySlot >= AbilitiesList.Count())
            {
                abilitySlot = AbilitiesList.Count() - 1;
                return;
            }

            Audio.PlaySound(GameSound.menu_select);

            SelectAbility(PartyMembers[partySlot].HeroModel.Value.Abilities[abilitySlot]);
            (GetWidget<DataGrid>("AbilitiesList").ChildList[abilitySlot] as Button).RadioSelect();
        }

        public void SelectParty(object parameter)
        {
            HeroModel record;
            if (parameter is IModelProperty)
            {
                record = (HeroModel)((IModelProperty)parameter).GetValue();
            }
            else record = (HeroModel)parameter;

            partySlot = PartyMembers.ToList().FindIndex(x => x.Value.HeroModel.Value == record);

            AbilitiesList.ModelList = record.Abilities.ModelList;

            ShowAbilities.Value = true;

            if (Input.MOUSE_MODE)
            {
                abilitySlot = -1;
                ShowDescription.Value = false;
            }
            else if (PartyMembers[partySlot].HeroModel.Value.Abilities.Count() > 0)
            {
                Audio.PlaySound(GameSound.menu_select);
                abilitySlot = 0;
                SelectAbility(PartyMembers[partySlot].HeroModel.Value.Abilities.First());
                (GetWidget<DataGrid>("AbilitiesList").ChildList[abilitySlot] as Button).RadioSelect();
            }
        }

        public void SelectAbility(object parameter)
        {
            CommandRecord record;
            if (parameter is IModelProperty)
            {
                record = (CommandRecord)((IModelProperty)parameter).GetValue();
            }
            else record = (CommandRecord)parameter;

            abilitySlot = AbilitiesList.ToList().FindIndex(x => x.Value == record);

            Description.Value = record.Description;

            ShowDescription.Value = true;
        }

        public void ResetSlot()
        {
            if (partySlot >= 0) (GetWidget<DataGrid>("PartyList").ChildList[partySlot] as Button).UnSelect();
            partySlot = -1;
            abilitySlot = -1;

            AbilitiesList.ModelList = new List<ModelProperty<AbilityRecord>>();
            ShowDescription.Value = false;
            ShowAbilities.Value = false;
        }

        public void MoveAway()
        {

        }

        public bool SuppressLeftRight { get => true; }

        public ModelProperty<bool> ShowAbilities { get; set; } = new ModelProperty<bool>(false);
        public ModelProperty<bool> ShowDescription { get; set; } = new ModelProperty<bool>(false);

        public ModelProperty<string> Description { get; set; } = new ModelProperty<string>("");
    }
}
