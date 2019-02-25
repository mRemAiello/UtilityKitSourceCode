using MCFramework;
using UnityEngine;

public class TestSave : MonoBehaviour
{
    class TestSaveData : IDataStore
    {
        public int classicBestScore;

        public void Init()
        {
            classicBestScore = 0;
        }

        public void PostLoad()
        {
        }

        public void PreSave()
        {
        }
    }

    void Start()
    {
        TestSaveData data = GameSaveManager.LoadData<TestSaveData>("save");

        Debug.Log(data.classicBestScore);

        //data.classicBestScore = 25;

        //GameSavemanager.SaveData("save", data);
    }
}
