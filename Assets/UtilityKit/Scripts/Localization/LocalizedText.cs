using UnityEngine;
using TMPro;

namespace MCFramework
{
    public class LocalizedText : MonoBehaviour
    {
        private TextMeshProUGUI m_TextMeshPro;
        public string key;

        void Start()
        {
            m_TextMeshPro = GetComponent<TextMeshProUGUI>();
            m_TextMeshPro.text = LocalizationManager.GetLocalizedValue(key);

            InvokeRepeating("UpdateText", 0.1f, 0.1f);
        }

        void UpdateText()
        {
            m_TextMeshPro.text = LocalizationManager.GetLocalizedValue(key);
        }
    }
}
