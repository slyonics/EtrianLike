﻿using EtrianLike.Main;
using EtrianLike.Models;
using EtrianLike.SceneObjects;
using EtrianLike.SceneObjects.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.ConversationScene
{
    public class ConversationController : ScriptController
    {
        private ConversationScene conversationScene;

        public bool EndGame { get; private set; }

        public ConversationController(ConversationScene iScene, string script)
            : base(iScene, script, PriorityLevel.GameLevel)
        {
            conversationScene = iScene;
        }

        public ConversationController(ConversationScene iScene, string[] script)
            : base(iScene, script, PriorityLevel.GameLevel)
        {
            conversationScene = iScene;
        }

        public override bool ExecuteCommand(string[] tokens)
        {
            switch (tokens[0])
            {
                case "AddPortrait": AddPortrait(tokens); break;
                case "RemovePortrait": RemovePortrait(tokens); break;
                case "PortraitSprite": PortraitSprite(tokens); break;
                case "PortraitPosition": PortraitPosition(tokens); break;
                case "PortraitColor": PortraitColor(tokens); break;
                case "PortraitScale": PortraitScale(tokens); break;
                case "GameEvent": GameEvent(tokens); break;
                case "EndGame": EndGame = true; break;
                case "WaitForText": WaitForText(tokens); break;
                case "ProceedText": conversationScene.ConversationViewModel.NextDialogue(); break;
                case "Actor": Actor(tokens); break;
                case "Animate": Animate(tokens); break;
                case "SelectionPrompt": SelectionPrompt(tokens); break;
                case "ChangeConversation": ChangeConversation(tokens); break;
                case "EndConversation": conversationScene.ConversationViewModel.disableEnd = false; conversationScene.ConversationViewModel.Proceed(); break;
                case "DisableEnd": conversationScene.ConversationViewModel.disableEnd = true; break;

                case "IncreaseStat": BattleScene.BattleController.IncreaseStat(tokens); break;
                //case "ChangeMap": MapScene.EventController.ChangeMap(tokens, null); break;
                case "ShowMapView":
                    {
                        TransitionController transitionController = new TransitionController(TransitionDirection.In, 1000, PriorityLevel.GameLevel);
                        transitionController.UpdateTransition += new Action<float>(f => MapScene.MapScene.Instance.MapViewModel.MapColor.Value = Color.Lerp(Color.Black, Color.White, f));
                        transitionController.FinishTransition += new Action<TransitionDirection>(d => MapScene.MapScene.Instance.MapViewModel.MapColor.Value = Color.White);

                        conversationScene.AddController(transitionController);

                        

                        break;
                    }
                case "HideMapView":
                    {
                        TransitionController transitionController = new TransitionController(TransitionDirection.In, 1000, PriorityLevel.GameLevel);
                        transitionController.UpdateTransition += new Action<float>(f => MapScene.MapScene.Instance.MapViewModel.MapColor.Value = Color.Lerp(Color.White, Color.Black, f));
                        transitionController.FinishTransition += new Action<TransitionDirection>(d => MapScene.MapScene.Instance.MapViewModel.MapColor.Value = Color.Black);

                        conversationScene.AddController(transitionController);



                        break;
                    }
                case "SetWaypoint": MapScene.EventController.SetWaypoint(tokens); break;

                case "ShortRest":
                    foreach (var hero in GameProfile.PlayerProfile.Party)
                        hero.Value.Magic.Value = Math.Min(hero.Value.Magic.Value + 16, hero.Value.MaxMagic.Value);
                    break;

                case "LongRest":
                    foreach (var hero in GameProfile.PlayerProfile.Party)
                        hero.Value.Magic.Value = hero.Value.MaxMagic.Value;
                    break;

                default: return false;
            }

            return true;
        }

        public override string ParseParameter(string parameter)
        {
            switch (parameter)
            {
                case "$leftPortraitX": return (((int)(CrossPlatformGame.ScreenWidth / 1.6) - 60) / 2 - 40).ToString();
                case "$leftPortraitX2": return (((int)(CrossPlatformGame.ScreenWidth / 1.6) - 60) / 2 + 80).ToString();
                case "$rightPortraitX": return (CrossPlatformGame.ScreenWidth - ((int)(CrossPlatformGame.ScreenWidth / 1.6) - 60) / 2 + 40).ToString();
                case "$birdPortraitX": return (CrossPlatformGame.ScreenWidth - ((int)(CrossPlatformGame.ScreenWidth / 1.6) - 60) / 2 - 100).ToString();
                case "$portraitY": return ((int)(CrossPlatformGame.ScreenHeight)).ToString();
                case "$portraitScaleX": return (CrossPlatformGame.ScreenWidth / 1920.0f / 1.6f).ToString();
                case "$portraitScaleY": return (CrossPlatformGame.ScreenHeight / 1080.0f / 1.6f).ToString();
                default: return base.ParseParameter(parameter);
            }
        }

        private void AddPortrait(string[] tokens)
        {
            if (tokens.Length > 6)
            {
                string name = tokens[1];
                string sprite = tokens[2];
                int startX = int.Parse(tokens[3]);
                int startY = int.Parse(tokens[4]);
                int endX = int.Parse(tokens[5]);
                int endY = int.Parse(tokens[6]);
                float transitionLength = (tokens.Length > 7) ? float.Parse(tokens[7], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

                Portrait portrait = new Portrait(conversationScene, name, sprite, new Vector2(startX, startY), new Vector2(endX, endY), transitionLength);
                conversationScene.AddPortrait(portrait);
            }
            else
            {
                string name = tokens[1];
                string sprite = tokens[2];
                int positionX = int.Parse(tokens[3]);
                int positionY = int.Parse(tokens[4]);
                float transitionLength = (tokens.Length > 5) ? float.Parse(tokens[5], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

                Portrait portrait = new Portrait(conversationScene, name, sprite, new Vector2(positionX, positionY), transitionLength);
                conversationScene.AddPortrait(portrait);
            }
        }

        private void RemovePortrait(string[] tokens)
        {
            if (tokens.Length > 3)
            {
                string name = tokens[1];
                int endX = int.Parse(tokens[2]);
                int endY = int.Parse(tokens[3]);
                float transitionLength = (tokens.Length > 4) ? float.Parse(tokens[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

                Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
                portrait.Remove(new Vector2(endX, endY), transitionLength);
            }
            else
            {
                string name = tokens[1];
                float transitionLength = (tokens.Length > 2) ? float.Parse(tokens[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

                Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
                portrait?.Remove(transitionLength);
            }
        }

        private void PortraitSprite(string[] tokens)
        {
            string name = tokens[1];
            string sprite = tokens[2];
            float transitionLength = (tokens.Length > 3) ? float.Parse(tokens[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

            Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
            portrait.SetSprite(sprite, transitionLength);
        }

        private void PortraitPosition(string[] tokens)
        {
            string name = tokens[1];
            int endX = int.Parse(tokens[2]);
            int endY = int.Parse(tokens[3]);
            float transitionLength = (tokens.Length > 4) ? float.Parse(tokens[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

            Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
            portrait.SetPosition(new Vector2(endX, endY), transitionLength);
        }

        private void PortraitColor(string[] tokens)
        {
            string name = tokens[1];
            int colorR = int.Parse(tokens[2]);
            int colorG = int.Parse(tokens[3]);
            int colorB = int.Parse(tokens[4]);
            int colorA = int.Parse(tokens[5]);
            float transitionLength = (tokens.Length > 6) ? float.Parse(tokens[6], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) : 1.0f;

            Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
            portrait.SetColor(new Color(colorR, colorG, colorB, colorA), transitionLength);
        }

        public void PortraitScale(string[] tokens)
        {
            string name = tokens[1];
            float scaleX = float.Parse(tokens[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            float scaleY = float.Parse(tokens[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

            Portrait portrait = conversationScene.Portraits.Find(x => x.Name == name);
            portrait.AnimatedSprite.Scale = new Vector2(scaleX, scaleY);
        }

        private void GameEvent(string[] tokens)
        {
            switch (tokens[1])
            {

            }
        }

        private void WaitForText(string[] tokens)
        {
            ScriptParser.UnblockFollowup followup = scriptParser.BlockScript();
            conversationScene.ConversationViewModel.OnDialogueScrolled += new Action(followup);
        }

        private void Actor(string[] tokens)
        {
            //CrossPlatformGame.GetScene<MapScene.MapScene>().MapViewModel.SetActor(tokens[1]);
        }

        private void Animate(string[] tokens)
        {
            //CrossPlatformGame.GetScene<MapScene.MapScene>().MapViewModel.AnimateActor(tokens[1]);
        }

        private void SelectionPrompt(string[] tokens)
        {
            List<string> options = new List<string>();
            string skipLine;
            do
            {
                skipLine = scriptParser.DequeueNextCommand();
                options.Add(skipLine);
            } while (skipLine != "End");
            options.RemoveAt(options.Count - 1);

            SelectionViewModel selectionViewModel = new SelectionViewModel(conversationScene, options);
            conversationScene.AddOverlay(selectionViewModel);

            ScriptParser.UnblockFollowup followup = scriptParser.BlockScript();
            selectionViewModel.OnTerminated += new Action(followup);
        }

        private void ChangeConversation(string[] tokens)
        {
            var conversationData = ConversationScene.CONVERSATIONS.FirstOrDefault(x => x.Name == tokens[1]);

            string[] conversationScript = conversationData.DialogueRecords[0].Script;
            if (conversationScript != null) conversationScene.RunScript(conversationData.DialogueRecords[0].Script);

            conversationScene.ConversationViewModel.ChangeConversation(conversationData);
        }
    }
}
