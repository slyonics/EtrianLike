using EtrianLike.Main;
using EtrianLike.Models;
using EtrianLike.SceneObjects.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtrianLike.Scenes.TitleScene
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel(Scene iScene, GameView viewName)
            : base(iScene, PriorityLevel.CutsceneLevel, viewName)
        {

        }

        public void ToggleFullscreen()
        {
            DisplayMode.Value = (DisplayMode.Value == "Windowed" ? "Fullscreen" : "Windowed");
        }

        public void ToggleAntialiasing()
        {
            Antialiasing.Value = (Antialiasing.Value == "Disabled" ? "8x AA" : "Disabled");
        }

        public void Apply()
        {
            Settings.SetProgramSetting<int>("SoundVolume", (int)(GetWidget<GaugeBar>("SoundBar").Value * 100));
            Settings.SetProgramSetting<int>("MusicVolume", (int)(GetWidget<GaugeBar>("MusicBar").Value * 100));
            Audio.ApplySettings();

            bool newFullscreen = DisplayMode.Value == "Fullscreen";
            bool oldFullscreen = Settings.GetProgramSetting<bool>("Fullscreen");
            Settings.SetProgramSetting<bool>("Fullscreen", DisplayMode.Value == "Fullscreen");

            bool newAntialiasing = Antialiasing.Value == "8x AA";
            bool oldAntialiasing = Settings.GetProgramSetting<bool>("Antialiasing");
            Settings.SetProgramSetting<bool>("Antialiasing", Antialiasing.Value == "8x AA");
            if (newFullscreen != oldFullscreen || newAntialiasing != oldAntialiasing)
            {
                CrossPlatformGame.GameInstance.ApplySettings();
                (parentScene as TitleScene).ResetSettings();
            }
        }

        public void Back()
        {
            Close();
        }

        public ModelProperty<string> DisplayMode { get; set; } = new ModelProperty<string>(Settings.GetProgramSetting<bool>("Fullscreen") ? "Fullscreen" : "Windowed");
        public ModelProperty<string> Antialiasing { get; set; } = new ModelProperty<string>(Settings.GetProgramSetting<bool>("Antialiasing") ? "8x AA" : "Disabled");
        public ModelProperty<float> SoundVolume { get; set; } = new ModelProperty<float>(Settings.GetProgramSetting<int>("SoundVolume") / 100.0f);
        public ModelProperty<float> MusicVolume { get; set; } = new ModelProperty<float>(Settings.GetProgramSetting<int>("MusicVolume") / 100.0f);
    }
}
