using UtilityKit;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour
{
    public Button italianButton;
    public Button spanishButton;
    public Button englishButton;

    void Start()
    {
        italianButton.onClick.AddListener(delegate { ChangeToItalian(); });
        spanishButton.onClick.AddListener(delegate { ChangeToSpanish(); });
        englishButton.onClick.AddListener(delegate { ChangeToEnglish(); });
    }

    public void ChangeToEnglish()
    {
        LocalizationManager.SetLanguage(SystemLanguage.English, true);
    }

    public void ChangeToItalian()
    {
        LocalizationManager.SetLanguage(SystemLanguage.Italian, true);
    }

    public void ChangeToSpanish()
    {
        LocalizationManager.SetLanguage(SystemLanguage.Spanish, true);
    }
}
