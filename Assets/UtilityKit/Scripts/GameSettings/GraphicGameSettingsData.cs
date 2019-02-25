using System;
using UnityEngine;

namespace UtilityKit
{
    [Serializable]
    public class GraphicGameSettingsData : IDataStore
    {
        public int screenWidth;
        public int screenHeight;
        public int refreshRate;
        public bool fullscreen;
        public bool vSync;
        public int quality;

        public void Init()
        {
            screenWidth = Screen.currentResolution.width;
            screenHeight = Screen.currentResolution.height;
            refreshRate = Screen.currentResolution.refreshRate;
            fullscreen = true;
            vSync = true;
            quality = 0;
        }

        /// <summary>
        /// Called just before we save
        /// </summary>
        public void PreSave()
        {
        }

        /// <summary>
        /// Called just after load
        /// </summary>
        public void PostLoad()
        {
        }
    }
}