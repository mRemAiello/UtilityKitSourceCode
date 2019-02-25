using MCFramework;
using UnityEngine;
using UnityEngine.UI;

public class TestAudio : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public Toggle musicToggle;
    public Toggle sfxToggle;

    public AudioClip music;
    public AudioClip sfx;

    void Start()
    {
        AudioManager.PlayBGM(music, true, 10.0f);        

        masterSlider.value = AudioManager.GameSettingsData.masterVolume;
        bgmSlider.value = AudioManager.GameSettingsData.BGMVolume;
        sfxSlider.value = AudioManager.GameSettingsData.SFXVolume;

        masterSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        bgmSlider.onValueChanged.AddListener(delegate { OnBGMValueChange(); });
        sfxSlider.onValueChanged.AddListener(delegate { OnSFXValueChange(); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.PlaySFX(sfx);
        }
    }

    public void OnMasterVolumeChange()
    {
        AudioManager.SetMasterVolume(masterSlider.value, true);
    }

    public void OnBGMValueChange()
    {
        AudioManager.SetBGMVolume(bgmSlider.value, true);
    }

    public void OnSFXValueChange()
    {
        AudioManager.SetBGMVolume(sfxSlider.value, true);
    }
}
