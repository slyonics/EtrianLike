using EtrianLike.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtrianLike.Scenes.CreditsScene
{
    public class CreditsViewModel : ViewModel
    {
        public CreditsViewModel(Scene iScene, GameView viewName)
            : base(iScene, PriorityLevel.GameLevel, viewName)
        {

        }

        public void Back()
        {
            //CrossPlatformGame.Transition(typeof(TitleScene.TitleScene));
            Close();
        }
    }
}
