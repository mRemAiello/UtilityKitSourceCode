using System;

namespace MCFramework
{
    [Serializable]
    public class LocalizationItem
    {
        public string key;
        public string value;

        public LocalizationItem()
        {
            key = "";
            value = "";
        }
    }
}

