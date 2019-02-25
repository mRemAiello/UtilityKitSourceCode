using UnityEngine;

namespace MCFramework
{
    public class GraphicGameSettingsManager : GenericGameSettingsManager<GraphicGameSettingsManager, GraphicGameSettingsData>
    {
        private void Start()
        {          
        }

        /// <summary>
        /// Initialize volumes
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            SetResolution(GameSettingsData.screenWidth, GameSettingsData.screenHeight, GameSettingsData.fullscreen, GameSettingsData.vSync);
            SetQuality(GameSettingsData.quality);
        }

        public static void SetQuality(int quality, bool save = false)
        {
            QualitySettings.SetQualityLevel(quality);

            if (save)
            {
                GameSettingsData.quality = quality;
                Instance.SaveData();
            }
        }

        public static void SetResolution(int width, int height, bool fullscreen, bool vsync, bool save = false)
        {
            Screen.SetResolution(width, height, fullscreen);
            QualitySettings.vSyncCount = vsync ? 1 : 0;

            if (save)
            {
                GameSettingsData.vSync = vsync;
                GameSettingsData.fullscreen = fullscreen;
                GameSettingsData.screenWidth = width;
                GameSettingsData.screenHeight = height;

                Instance.SaveData();
            }
        }
    }
}
