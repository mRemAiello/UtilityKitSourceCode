using UtilityKit;
using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{
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
