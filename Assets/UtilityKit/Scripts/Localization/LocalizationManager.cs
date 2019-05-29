using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UtilityKit
{
    public class LocalizationManager : GenericGameSettingsManager<LocalizationManager, LocalizationSettingsData>
    {
        public SystemLanguage defaultLocalization;
        public LocalizationData[] localizationData;

        private Dictionary<string, string> m_LocalizedText;
        private readonly string m_MissingTextString = "LABEL_STRING_NOT_FOUND";

        private void Start()
        {
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            SystemLanguage currentLanguage = GameSettingsData.currentLanguage;
            LocalizationData data = localizationData.FirstOrDefault(x => x.language == currentLanguage);
            if (data == null)
                currentLanguage = defaultLocalization;

            SetLanguage(currentLanguage, true);
        }

        public static void SetLanguage(SystemLanguage language, bool save = false)
        {
            Instance.m_LocalizedText = new Dictionary<string, string>();

            LocalizationData toLoad = Instance.localizationData.FirstOrDefault(x => x.language == language);
            if (toLoad != null)
            {
                for (int i = 0; i < toLoad.items.Length; i++)
                {
                    Instance.m_LocalizedText.Add(toLoad.items[i].key, toLoad.items[i].value);
                }

                LocalizedText[] texts = FindObjectsOfType<LocalizedText>();
                foreach (LocalizedText text in texts)
                {
                    text.UpdateText();
                }
            }

            if (save)
            {
                GameSettingsData.currentLanguage = language;
                Instance.SaveData();
            }
        }

        public static string GetLocalizedValue(string key)
        {
            if (Instance.m_LocalizedText == null)
                SetLanguage(GameSettingsData.currentLanguage);

            string result = Instance.m_MissingTextString;
            if (Instance.m_LocalizedText.ContainsKey(key))
            {
                result = Instance.m_LocalizedText[key];
            }

            return result;
        }
    }
}
