using System;
using UnityEngine;

namespace UtilityKit
{
    [Serializable]
    public class LocalizationData : ScriptableObject
    {
        public SystemLanguage language;
        public LocalizationItem[] items;
    }
}
