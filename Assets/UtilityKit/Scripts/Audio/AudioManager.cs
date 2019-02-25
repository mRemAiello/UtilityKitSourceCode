using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MCFramework
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : GenericGameSettingsManager<AudioManager, AudioGameSettingsData>
    {
        /// <summary>
        /// Reference to audio mixer for volume changing
        /// </summary>
        public AudioMixer audioMixer;

        private AudioSource m_BGMSource;
        private List<AudioSource> m_SFXSources;

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

            GameObject go = new GameObject("BGMSource", typeof(AudioSource));
            go.transform.parent = transform;
            m_BGMSource = go.GetComponent<AudioSource>();
            m_BGMSource.loop = true;
            m_BGMSource.playOnAwake = false;
            m_BGMSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];

            SetVolumes(GameSettingsData.masterVolume, GameSettingsData.SFXVolume, GameSettingsData.BGMVolume);
        }

        /// <summary>
        /// Set and persist master volume
        /// </summary>
        public static void SetMasterVolume(float masterVolume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            Instance.audioMixer.SetFloat("MasterVolume", LogarithmicDbTransform(Mathf.Clamp01(masterVolume)));

            if (save)
            {
                GameSettingsData.masterVolume = masterVolume;
                Instance.SaveData();
            }
        }

        /// <summary>
        /// Set and persist music volumes
        /// </summary>
        public static void SetSFXVolume(float sfxVolume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            Instance.audioMixer.SetFloat("SFXVolume", LogarithmicDbTransform(Mathf.Clamp01(sfxVolume)));

            if (save)
            {
                GameSettingsData.SFXVolume = sfxVolume;
                Instance.SaveData();
            }
        }

        /// <summary>
        /// Set and persist music volumes
        /// </summary>
        public static void SetBGMVolume(float bgmVolume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            Instance.audioMixer.SetFloat("BGMVolume", LogarithmicDbTransform(Mathf.Clamp01(bgmVolume)));

            if (save)
            {
                GameSettingsData.BGMVolume = bgmVolume;
                Instance.SaveData();
            }
        }

        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void SetVolumes(float masterVolume, float bgmVolume, float sfxVolume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            SetMasterVolume(masterVolume, save);
            SetBGMVolume(bgmVolume, save);
            SetSFXVolume(sfxVolume, save);
        }

        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void SetVolumes(bool muteBGM, bool muteSFX, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;



            if (muteBGM)
                Instance.audioMixer.SetFloat("BGMVolume", 0.0f);
            if (muteSFX)
                Instance.audioMixer.SetFloat("SFXVolume", 0.0f);

            if (save)
            {
                if (muteBGM)
                    GameSettingsData.BGMVolume = 0.0f;
                if (muteSFX)
                    GameSettingsData.SFXVolume = 0.0f;

                GameSettingsData.muteBGM = muteBGM;
                GameSettingsData.muteSFX = muteSFX;

                Instance.SaveData();
            }
        }

        /// <summary>
        /// Transform volume from linear to logarithmic
        /// </summary>
        protected static float LogarithmicDbTransform(float volume)
        {
            volume = Mathf.Log(89 * volume + 1) / Mathf.Log(90) * 80;
            return volume - 80;
        }

        private void FadeBGMOut(float duration)
        {
            float delay = 0f;
            float volume = 0f;
            StartCoroutine(FadeBGM(volume, delay, duration));
        }

        private void FadeBGMIn(AudioClip clip, float delay, float duration)
        {          
            float volume = GameSettingsData.BGMVolume;
            Instance.m_BGMSource.clip = clip;
            Instance.m_BGMSource.Play();

            StartCoroutine(FadeBGM(volume, delay, duration));
        }

        private IEnumerator FadeBGM(float fadeToVolume, float delay, float duration)
        {
            yield return new WaitForSeconds(delay);

            float elapsed = 0f;
            while (duration > 0)
            {
                float t = elapsed / duration;
                float volume = Mathf.Lerp(0f, fadeToVolume, t);
                Instance.m_BGMSource.volume = volume;

                elapsed += Time.deltaTime;
                yield return 0;
            }
        }

        public static void PlayBGM(AudioClip clip)
        {
            PlayBGM(clip, false, 0.0f);
        }

        public static void PlayBGM(AudioClip clip, bool fade, float fadeDuration)
        {
            if (fade)
            {
                if (Instance.m_BGMSource.isPlaying)
                {
                    Instance.FadeBGMOut(fadeDuration / 2);
                    Instance.FadeBGMIn(clip, fadeDuration / 2, fadeDuration / 2);
                }
                else
                {
                    float delay = 0f;
                    Instance.FadeBGMIn(clip, delay, fadeDuration);
                }
            }
            else
            {
                Instance.m_BGMSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("BGM")[0];
                Instance.m_BGMSource.clip = clip;
                Instance.m_BGMSource.Play();
            }
        }

        public static void StopBGM(bool fade, float fadeDuration)
        {
            if (Instance.m_BGMSource.isPlaying)
            {
                if (fade)
                {
                    Instance.FadeBGMOut(fadeDuration);
                }
                else
                {
                    Instance.m_BGMSource.Stop();
                }
            }
        }

        private AudioSource GetSFXSource()
        {
            GameObject go = new GameObject("SFXSource", typeof(AudioSource));
            go.transform.parent = transform;
            AudioSource sfxSource = go.GetComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("SFX")[0];

            if (m_SFXSources == null)
                m_SFXSources = new List<AudioSource>();
            m_SFXSources.Add(sfxSource);

            return sfxSource;
        }

        private IEnumerator RemoveSFXSource(AudioSource sfxSource)
        {
            yield return new WaitForSeconds(sfxSource.clip.length);
            m_SFXSources.Remove(sfxSource);
            Destroy(sfxSource.gameObject);
        }

        private IEnumerator RemoveSFXSourceFixedLength(AudioSource sfxSource, float length)
        {
            yield return new WaitForSeconds(length);
            m_SFXSources.Remove(sfxSource);
            Destroy(sfxSource);
        }

        public static void PlaySFX(AudioClip sfxClip)
        {
            AudioSource source = Instance.GetSFXSource();
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("SFX")[0];
            source.clip = sfxClip;
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSFXSource(source));
        }

        public static void PlaySFXRandomized(AudioClip clip)
        {
            AudioSource source = Instance.GetSFXSource();
            source.clip = clip;
            source.pitch = Random.Range(0.85f, 1.2f);
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("SFX")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSFXSource(source));
        }

        public static void PlaySFXFixedDuration(AudioClip clip, float duration)
        {
            AudioSource source = Instance.GetSFXSource();
            source.clip = clip;
            source.loop = true;
            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("SFX")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSFXSourceFixedLength(source, duration));
        }
    }
}
