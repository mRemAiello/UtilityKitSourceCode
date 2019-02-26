using UtilityKit;

public class TestSaveData : IDataStore
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
