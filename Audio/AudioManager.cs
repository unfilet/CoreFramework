using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine.Audio;

namespace Core.Scripts.Audio
{
    public interface IAudioManager 
    {
        AudioSource SFXSource { get; }
        AudioSource MusicSource { get; }

        void PlaySFX(AudioClip clip);
        void PlayMusic(AudioClip clip);

        void PlaySFX(AudioEvent audioEvent, bool force = false);
        void PlayMusic(AudioEvent audioEvent, bool force = false);
    }

    public class AudioManager : MonoBehaviour, IAudioManager
    {
        #region Audio Sources

        [SerializeField] protected AudioSource sourceMusic;
        [SerializeField] protected AudioSource sourceEffects;

        #endregion

        #region Property

        public AudioSource SFXSource => sourceEffects;
        public AudioSource MusicSource => sourceMusic;

        public static event Action<bool> OnSoundsChanged = null;
        public static bool Sounds
        {
            get { return PlayerPrefsX.GetBool("Sounds", true); }
            set
            {
                if (Sounds == value) return;
                PlayerPrefsX.SetBool("Sounds", value);
                OnSoundsChanged?.Invoke(value);
            }
        }

        public static event Action<bool> OnMusicChanged = null;
        public static bool Music
        {
            get { return PlayerPrefsX.GetBool("Music", true); }
            set
            {
                if (Music == value) return;
                PlayerPrefsX.SetBool("Music", value);
                OnMusicChanged?.Invoke(value);
            }
        }

        public bool Mute {
            get { return sourceMusic.mute; }
            set {
                sourceMusic.mute = value;
            }
        }
        
        #endregion

        public static float LinearToDecibel(float linear)
            => Mathf.Log10(Mathf.Max(0.0001f, linear)) * 20f;
        public static float DecibelToLinear(float dB)
            => Mathf.Pow(10.0f, dB / 20.0f);
        
        public static (Func<float> get, Action<float> set) MixerVolume(AudioSource source, string name)
        {
            return (get: () =>
                {
                    source.outputAudioMixerGroup.audioMixer.GetFloat(name, out var value); 
                    return DecibelToLinear(value);
                },
                set: value => source.outputAudioMixerGroup.audioMixer.SetFloat(name, LinearToDecibel(value)));
        }
        
        #region Lifecycle

        protected  void OnEnable()
        {
            OnMusicChanged += OnMusic;
        }

        protected  void OnDisable()
        {
            OnMusicChanged -= OnMusic;
        }

        #endregion

        #region Callback

        private void OnMusic (bool obj)
        {
            if (obj) return;
            var e = ScriptableObject.CreateInstance<SimpleFadeAudioEvent>();
            PlayMusic(e, true);
        }

        #endregion

        #region Play Music

        public void PlayMusic(AudioClip clip)
        {
            var e = ScriptableObject.CreateInstance<SimpleFadeAudioEvent>();
            e.clips = new []{ clip };
            e.loop = false;
            PlayMusic(e);
        }

        public void StopEffects()
            => sourceEffects?.Stop();

        #endregion

        #region Play Short

        public void PlaySFX(AudioClip clip)
        {
            if (!Sounds) return;
            sourceEffects.PlayOneShot(clip);
        }

        #endregion

        #region AudioEvents

        public void PlaySFX(AudioEvent audioEvent, bool force = false)
        {
            if (Sounds || force)
                audioEvent.Play(sourceEffects);
        }

        public void PlayMusic(AudioEvent audioEvent, bool force = false)
        {
            if (Music || force) 
                audioEvent.Play(sourceMusic);
        }

        #endregion
    }
}