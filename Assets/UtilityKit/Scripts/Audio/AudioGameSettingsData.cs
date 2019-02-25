using System;

namespace MCFramework
{
    [Serializable]
    public class AudioGameSettingsData : IDataStore
    {
        public bool muteBGM;
        public bool muteSFX;
        public float masterVolume;
        public float BGMVolume;
        public float SFXVolume;

        public void Init()
        {
            muteBGM = false;
            muteSFX = false;
            masterVolume = 1.0f;
            BGMVolume = 1.0f;
            SFXVolume = 1.0f;
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

