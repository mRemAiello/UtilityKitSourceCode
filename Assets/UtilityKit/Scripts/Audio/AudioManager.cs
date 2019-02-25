using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UtilityKit
{
    public enum AudioVolumeChannel
    {
        Master,
        Music,
        Sound
    }

    public enum AudioMuteChannel
    {
        Music,
        Sound
    }

    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : GenericGameSettingsManager<AudioManager, AudioGameSettingsData>
    {
        /// <summary>
        /// Reference to audio mixer for volume changing
        /// </summary>
        public AudioMixer audioMixer;

        private AudioSource m_MusicSource;
        private List<AudioSource> m_SoundSources;

        /// <summary>
        /// Just for enable / disable component
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Initialize volumes
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            if (m_SoundSources == null)
                m_SoundSources = new List<AudioSource>();

            GameObject go = new GameObject("MusicSource", typeof(AudioSource));
            go.transform.parent = transform;
            m_MusicSource = go.GetComponent<AudioSource>();
            m_MusicSource.mute = GameSettingsData.musicMuted;
            m_MusicSource.loop = true;
            m_MusicSource.playOnAwake = false;
            m_MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];

            SetVolumes(GameSettingsData.masterVolume, GameSettingsData.musicVolume, GameSettingsData.soundVolume);
            MuteVolumes(GameSettingsData.musicMuted, GameSettingsData.soundMuted);
        }

        /// <summary>
        /// Set and persist master volume
        /// </summary>
        public static void SetVolume(AudioVolumeChannel audioType, float volume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            switch (audioType)
            {
                case AudioVolumeChannel.Master:
                    Instance.audioMixer.SetFloat("MasterVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;

                case AudioVolumeChannel.Music:
                    Instance.audioMixer.SetFloat("MusicVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;

                case AudioVolumeChannel.Sound:
                    Instance.audioMixer.SetFloat("SoundVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;
            }

            if (save)
            {
                switch (audioType)
                {
                    case AudioVolumeChannel.Master:
                        GameSettingsData.masterVolume = volume;
                        break;

                    case AudioVolumeChannel.Music:
                        GameSettingsData.musicVolume = volume;
                        break;

                    case AudioVolumeChannel.Sound:
                        GameSettingsData.soundVolume = volume;
                        break;
                }

                Instance.SaveData();
            }
        }

        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void SetVolumes(float masterVolume, float musicVolume, float soundVolume, bool save = false)
        {
            SetVolume(AudioVolumeChannel.Master, masterVolume, save);
            SetVolume(AudioVolumeChannel.Music, musicVolume, save);
            SetVolume(AudioVolumeChannel.Sound, soundVolume, save);
        }

        /// <summary>
        /// Mute or unmute
        /// </summary>
        public static void MuteVolume(AudioMuteChannel audioType, bool mute, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            if (audioType == AudioMuteChannel.Music)
            {
                Instance.m_MusicSource.mute = mute;
            }
            else
            {
                foreach (AudioSource source in Instance.m_SoundSources)
                    source.mute = mute;
            }                

            if (save)
            {
                if (audioType == AudioMuteChannel.Music)
                    GameSettingsData.musicMuted = mute;
                else
                    GameSettingsData.soundMuted = mute;
                
                Instance.SaveData();
            }
        }

        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void MuteVolumes(bool muteMusic, bool muteSound, bool save = false)
        {
            MuteVolume(AudioMuteChannel.Music, muteMusic, save);
            MuteVolume(AudioMuteChannel.Sound, muteSound, save);
        }

        /// <summary>
        /// Transform volume from linear to logarithmic
        /// </summary>
        protected static float LogarithmicDbTransform(float volume)
        {
            volume = Mathf.Log(89 * volume + 1) / Mathf.Log(90) * 80;
            return volume - 80;
        }

        private void FadeMusicOut(float duration)
        {
            float delay = 0f;
            float volume = 0f;
            StartCoroutine(FadeMusic(volume, delay, duration));
        }

        private void FadeMusicIn(AudioClip clip, float delay, float duration)
        {
            float volume = GameSettingsData.musicVolume;
            Instance.m_MusicSource.mute = GameSettingsData.musicMuted;
            Instance.m_MusicSource.clip = clip;
            Instance.m_MusicSource.Play();

            StartCoroutine(FadeMusic(volume, delay, duration));
        }

        private IEnumerator FadeMusic(float fadeToVolume, float delay, float duration)
        {
            yield return new WaitForSeconds(delay);

            float elapsed = 0f;
            while (duration > 0)
            {
                float t = elapsed / duration;
                float volume = Mathf.Lerp(0f, fadeToVolume, t);
                Instance.m_MusicSource.volume = volume;

                elapsed += Time.deltaTime;
                yield return 0;
            }
        }

        public static void PlayMusic(AudioClip clip)
        {
            PlayMusic(clip, false, 0.0f);
        }

        public static void PlayMusic(AudioClip clip, bool fade, float fadeDuration)
        {
            if (fade)
            {
                if (Instance.m_MusicSource.isPlaying)
                {
                    Instance.FadeMusicOut(fadeDuration / 2);
                    Instance.FadeMusicIn(clip, fadeDuration / 2, fadeDuration / 2);
                }
                else
                {
                    float delay = 0f;
                    Instance.FadeMusicIn(clip, delay, fadeDuration);
                }
            }
            else
            {
                Instance.m_MusicSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Music")[0];
                Instance.m_MusicSource.mute = GameSettingsData.musicMuted;
                Instance.m_MusicSource.clip = clip;
                Instance.m_MusicSource.Play();
            }
        }

        public static void StopMusic(bool fade, float fadeDuration)
        {
            if (Instance.m_MusicSource.isPlaying)
            {
                if (fade)
                {
                    Instance.FadeMusicOut(fadeDuration);
                }
                else
                {
                    Instance.m_MusicSource.Stop();
                }
            }
        }

        private AudioSource GetSoundSource()
        {
            GameObject go = new GameObject("SoundSource", typeof(AudioSource));
            go.transform.parent = transform;

            AudioSource soundSource = go.GetComponent<AudioSource>();
            soundSource.mute = GameSettingsData.soundMuted;
            soundSource.loop = false;
            soundSource.playOnAwake = false;
            soundSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];

            if (m_SoundSources == null)
                m_SoundSources = new List<AudioSource>();
            m_SoundSources.Add(soundSource);

            return soundSource;
        }

        private IEnumerator RemoveSoundSource(AudioSource sfxSource)
        {
            yield return new WaitForSeconds(sfxSource.clip.length);
            m_SoundSources.Remove(sfxSource);
            Destroy(sfxSource.gameObject);
        }

        private IEnumerator RemoveSoundSourceFixedLength(AudioSource sfxSource, float length)
        {
            yield return new WaitForSeconds(length);
            m_SoundSources.Remove(sfxSource);
            Destroy(sfxSource);
        }

        public static void PlaySound(AudioClip sfxClip)
        {
            AudioSource source = Instance.GetSoundSource();
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
            source.mute = GameSettingsData.soundMuted;
            source.clip = sfxClip;
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSoundSource(source));
        }

        public static void PlaySoundRandomized(AudioClip clip)
        {
            AudioSource source = Instance.GetSoundSource();
            source.mute = GameSettingsData.soundMuted;
            source.clip = clip;
            source.pitch = Random.Range(0.85f, 1.2f);
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSoundSource(source));
        }

        public static void PlaySoundFixedDuration(AudioClip clip, float duration)
        {
            AudioSource source = Instance.GetSoundSource();
            source.mute = GameSettingsData.soundMuted;
            source.clip = clip;
            source.loop = true;
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSoundSourceFixedLength(source, duration));
        }
    }
}
