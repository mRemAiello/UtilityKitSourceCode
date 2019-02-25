using UtilityKit;
using UnityEngine;
using UnityEngine.UI;
using AudioVolumeChannel = UtilityKit.AudioVolumeChannel;

public class TestAudio : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    public Toggle musicToggle;
    public Toggle soundToggle;

    public AudioClip music;
    public AudioClip sfx;

    void Start()
    {
        masterSlider.value = AudioManager.GameSettingsData.masterVolume;
        musicSlider.value = AudioManager.GameSettingsData.musicVolume;
        soundSlider.value = AudioManager.GameSettingsData.soundVolume;

        musicToggle.isOn = AudioManager.GameSettingsData.musicMuted;
        soundToggle.isOn = AudioManager.GameSettingsData.soundMuted;

        masterSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        musicSlider.onValueChanged.AddListener(delegate { OnMusicValueChange(); });
        soundSlider.onValueChanged.AddListener(delegate { OnSoundValueChange(); });

        musicToggle.onValueChanged.AddListener(delegate { OnMusicToggleChange(); });
        soundToggle.onValueChanged.AddListener(delegate { OnSoundToggleChange(); });

        AudioManager.PlayMusic(music, true, 5.0f);        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.PlaySound(sfx);
        }
    }

    public void OnMasterVolumeChange()
    {
        AudioManager.SetVolume(AudioVolumeChannel.Master, masterSlider.value, true);
    }

    public void OnMusicValueChange()
    {
        AudioManager.SetVolume(AudioVolumeChannel.Music, musicSlider.value, true);
    }

    public void OnSoundValueChange()
    {
        AudioManager.SetVolume(AudioVolumeChannel.Sound, soundSlider.value, true);
    }

    public void OnMusicToggleChange()
    {
        AudioManager.MuteVolume(AudioMuteChannel.Music, musicToggle.isOn, true);
    }

    public void OnSoundToggleChange()
    {
        AudioManager.MuteVolume(AudioMuteChannel.Sound, soundToggle.isOn, true);
    }
}
