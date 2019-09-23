using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;

namespace Core.Scripts.Audio
{
    public class AudioManager : MonoBehaviourExt
    {

        #region Audio Sources

        [SerializeField] AudioSource sourceMusic;
        [SerializeField] AudioSource sourceEffects;
        [SerializeField] AudioSource sourceInstructions;

        #endregion

        private AudioMixer masterMixer = null;
        private IEnumerator fadeCoroutine = null;


        AudioQueue instructionQueue;

        #region Property

        public static event System.Action<bool> OnSoundsChanged = null;
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

        public static event System.Action<bool> OnMusicChanged = null;
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

        public float MixerVolume
        {
            get
            {
                float retval = 0;
                masterMixer?.GetFloat("Volume", out retval);
                return retval;
            }
            set
            {
                masterMixer?.SetFloat("Volume", value);
            }
        }

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();
            instructionQueue = new AudioQueue(sourceInstructions);
        }

        protected override void OnEnable()
        {
            OnMusicChanged += OnMusic;
        }

        protected override void OnDisable()
        {
            OnMusicChanged -= OnMusic;
        }


        #endregion

        #region Callback

        void OnMusic(bool obj)
        {
            StopAllCoroutines();
            if (!obj) StartCoroutine(FadeOutIn(sourceMusic, null, 0.7f));
        }

        #endregion

        #region Ienumerator

        private IEnumerator FadeOutIn(AudioSource audioSource, AudioClip nextClip, float FadeTime)
        {

            float speed = 1;
            if (audioSource.isPlaying)
            {
                //            audioSource.volume = 1;
                while (audioSource.volume > 0)
                {
                    audioSource.volume -= speed * Time.deltaTime / FadeTime;
                    yield return null;
                }
            }

            if (nextClip != null)
            {
                audioSource.volume = 0;
                audioSource.clip = nextClip;
                audioSource.Play();

                while (audioSource.volume < 1)
                {
                    audioSource.volume += speed * Time.deltaTime / FadeTime;
                    yield return null;
                }
            }
            else
            {
                audioSource.Stop();
                audioSource.volume = 1;
                yield break;
            }
        }

        #endregion

        #region Play Music

        public void PlayMusic(AudioClip clip)
        {
            if (sourceMusic.clip == clip && sourceMusic.isPlaying) return;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            if (Music)
            {
                fadeCoroutine = FadeOutIn(sourceMusic, clip, 0.7f);
                StartCoroutine(fadeCoroutine);
            }
            else
                sourceMusic.clip = clip;
        }

        public void PlayInst(AudioClip clip, double delay, Action action = null) {
            instructionQueue.AddToQueue(clip, delay, action);
        }

        public void PlayInst(IEnumerable<AudioClip> clip, double delay, Action action = null)
        {
            instructionQueue.AddToQueue(clip, delay, action);
        }

        public void StopInst()
        {
            instructionQueue.StopAndClear();
        }

        #endregion

        #region Play Short

        public void PlayShort(AudioClip clip)
        {
            if (!Sounds)
                return;

            float vol = 1 - Mathf.Abs(MixerVolume / 80f);
            sourceMusic.PlayOneShot(clip, vol);
        }

        #endregion

        #region AudioEvents

        public void PlayShort(AudioEvent audioEvent)
        {
            if (!Sounds) return;
            audioEvent.Play(sourceMusic);
        }

        public void PlayMusic(AudioEvent audioEvent, bool loop = true)
        {
            sourceMusic.loop = loop;
            if (Music) audioEvent.Play(sourceMusic);
        }

        #endregion
    }

    public class AudioQueue
    {
		private struct ClipInf
		{
			public AudioClip clip { get; }
			public double delay { get; }
            public Action OnComplete { get; }

			public ClipInf(AudioClip clip, double delay = 0, Action action = null)
			{
				this.clip = clip;
				this.delay = delay;
                this.OnComplete = action;

            }
		}

        private AudioSource audio;
        private Queue<ClipInf> clipQueue;

        public bool isPlaying { get; private set; }

        public AudioQueue(AudioSource source)
        {
            audio = source;
            clipQueue = new Queue<ClipInf>();
        }

        public void AddToQueue(AudioClip clip, double delay = 0, Action action = null)
        {
            clipQueue.Enqueue(new ClipInf(clip, delay, action));
            Play();
        }

        public void AddToQueue(IEnumerable<AudioClip> clips, double delay = 0, Action action = null)
        {
            foreach (AudioClip clip in clips)
                AddToQueue(clip, delay);
            AddToQueue((AudioClip)null, 0, action);
        }
    
        private void Play()
        {
            if (isPlaying) return;
            _ = PlayNext().ConfigureAwait(true);
		}

		private async Task PlayNext() {

			if (clipQueue.Count <= 0)
			{
				StopAndClear();
				return;
			}

			var inf = clipQueue.Dequeue();
			isPlaying = true;

			await Delay(inf.delay);

            if (inf.clip != null)
            {
                audio.clip = inf.clip;
                audio.Play();

                double duration = (double)audio.clip.samples / audio.clip.frequency;

                await Delay(duration);
            }

            inf.OnComplete?.Invoke();

            _ = PlayNext();
		}

        private async Task Delay(double delay, Action next = null)
        {
            if (delay > 0)
			    await Task.Delay(TimeSpan.FromSeconds(delay));
            if (next != null)
			    SyncContextUtil.RunOnUnityScheduler(next);
        }

        public void StopAndClear()
        {
            if (audio) audio.Stop();
            clipQueue.Clear();
            isPlaying = false;
        }
    }
}