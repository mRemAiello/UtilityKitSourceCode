using UnityEditor;

namespace MCFramework
{
    public class LocalizationDataCreator
    {
        [MenuItem("Assets/Create/McFramework/Localization Data")]
        static public void CreateItem()
        {
            ScriptableObjectUtility.CreateAsset<LocalizationData>("LocalizationData");
        }
    }
}
