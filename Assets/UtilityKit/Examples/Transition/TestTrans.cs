using MCFramework;
using UnityEngine;

public class TestTrans : MonoBehaviour
{
    void Start()
    {
        Invoke("ChangeScene", 2f);
    }

    public void ChangeScene()
    {
        var fader = new FadeTransition()
        {
            nextScene = 1,
            fadeToColor = Color.black
        };
        TransitionManager.TransitionWithDelegate(fader);
    }
}
