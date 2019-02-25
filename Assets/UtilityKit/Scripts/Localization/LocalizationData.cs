using System;
using UnityEngine;

namespace MCFramework
{
    [Serializable]
    public class LocalizationData : ScriptableObject
    {
        public SystemLanguage language;
        public LocalizationItem[] items;
    }
}
