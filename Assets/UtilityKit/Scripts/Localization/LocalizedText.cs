using UnityEngine;
using TMPro;

namespace UtilityKit
{
    public class LocalizedText : MonoBehaviour
    {
        private TextMeshProUGUI m_TextMeshPro;
        public string key;

        void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            m_TextMeshPro = GetComponent<TextMeshProUGUI>();
            m_TextMeshPro.text = LocalizationManager.GetLocalizedValue(key);
        }
    }
}
