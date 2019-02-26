using UtilityKit;
using UnityEngine;
using UnityEngine.UI;

public class TestSave : MonoBehaviour
{
    private TestSaveData data;

    public Button updateHighScoreButton;
    public Button loadHighScoreButton;
    public Text highScoreText;

    void Start()
    {
        LoadGame();

        updateHighScoreButton.onClick.AddListener(delegate { UpdateHighScores(); });
        loadHighScoreButton.onClick.AddListener(delegate { LoadGame(); });
    }

    public void LoadGame()
    {
        data = GameSaveManager.LoadData<TestSaveData>("save");
        highScoreText.text = string.Format("Best Score: {0}", data.classicBestScore);
    }

    public void UpdateHighScores()
    {
        data = GameSaveManager.LoadData<TestSaveData>("save");
        data.classicBestScore = Random.Range(1, 50);
        GameSaveManager.SaveData("save", data);
    }
}
