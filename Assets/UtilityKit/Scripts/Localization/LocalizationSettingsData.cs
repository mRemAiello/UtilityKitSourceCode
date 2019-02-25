using System;
using UnityEngine;

namespace MCFramework
{
    [Serializable]
    public class LocalizationSettingsData : IDataStore
    {
        public SystemLanguage currentLanguage;

        public void Init()
        {          
            currentLanguage = Application.systemLanguage;
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

