using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using System.Threading;
using UniRx;

namespace Core.Scripts.Audio
{
    using ClipState = AudioQueue.ClipState;

    public class AudioManager : MonoBehaviourExt
    {

        #region Audio Sources

        [SerializeField] AudioSource sourceMusic;
        [SerializeField] AudioSource sourceEffects;
        [SerializeField] AudioSource sourceInstructions;

        #endregion

        private AudioMixer masterMixer = null;
        private IEnumerator fadeCoroutine = null;


        public AudioQueue instructionQueue { get; private set; }

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

        public IObservable<ClipState> PlayInst(AudioClip clip, double delay, Action<ClipState> action = null)
            => instructionQueue.AddToQueue(clip, delay, action);

        public IObservable<ClipState> PlayInst(double delay, params AudioClip[] clips)
            => PlayInst(delay, (IEnumerable<AudioClip>)clips);

        public IObservable<ClipState> PlayInst(double delay, IEnumerable<AudioClip> clips)
            => clips.Select(c => PlayInst(c, delay)).ToArray().Concat();

        public void StopInst()
        {
            instructionQueue?.StopAndClear();
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
        public enum ClipState
        {
            None,
            Wait,
            Start,
            End
        }

        private class ClipInf : IObservable<ClipState>, IObserver<ClipState>
        {
            private Subject<ClipState> subject;
            public ClipState currentState { get; private set; }

            public AudioClip clip { get; }
            public double delay { get; }
            public double duration =>
                clip == null ? 0 : (double)clip.samples / clip.frequency;

            public ClipInf(AudioClip clip, double delay = 0)
            {
                this.clip = clip;
                this.delay = delay;
                this.subject = new Subject<ClipState>();
            }

            public ClipInf(AudioClip clip, double delay = 0, Action<ClipState> action = null) : this(clip, delay)
            {
                if (action != null)
                    subject.Subscribe(action);
            }

            ~ClipInf()
            {
                subject?.Dispose();
            }

            public IDisposable Subscribe(IObserver<ClipState> observer)
                => subject.Subscribe(observer);

            public void OnCompleted() => subject.OnCompleted();
            public void OnError(Exception error) => subject.OnError(error);
            public void OnNext(ClipState value) => subject.OnNext(currentState = value);
        }

        private AudioSource audio;
        private Queue<ClipInf> clipQueue;

        public bool isPlaying => cts != null && !cts.IsCancellationRequested;

        CancellationTokenSource cts;
        object observerLock = new object();


        public AudioQueue(AudioSource source)
        {
            audio = source;
            clipQueue = new Queue<ClipInf>();
        }

        public IObservable<ClipState> AddToQueue(AudioClip clip, double delay = 0, Action<ClipState> action = null)
        {
            var ci = new ClipInf(clip, delay, action);
            lock (observerLock)
                clipQueue.Enqueue(ci);
            Play();
            return ci;
        }

        private async void Play()
        {
            if (cts == null || cts.IsCancellationRequested)
            {
                var ctSource = this.cts = new CancellationTokenSource();
                try
                {
                    await PlayNext(ctSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (this.cts == ctSource)
                        this.cts = null;
                }
            }
        }

        private async Task PlayNext(CancellationToken cancellationToken)
        {
            while (clipQueue != null && clipQueue.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ClipInf inf = null;
                lock (observerLock)
                    inf = clipQueue.Dequeue();

                if (inf == null)
                    continue;

                if (inf.delay > 0)
                {
                    inf.OnNext(ClipState.Wait);

                    await Task.Delay(
                        TimeSpan.FromSeconds(inf.delay),
                        cancellationToken);
                }

                if (inf.clip != null && audio)
                {
                    audio.clip = inf.clip;
                    audio.Play();

                    inf.OnNext(ClipState.Start);
                    await Task.Delay(
                        TimeSpan.FromSeconds(inf.duration),
                        cancellationToken);
                }

                inf.OnNext(ClipState.End);
                inf.OnCompleted();
            }
        }

        public void StopAndClear()
        {
            cts?.Cancel();

            if (audio) audio.Stop();
            lock (observerLock)
            {

                foreach (var item in clipQueue)
                    item.OnCompleted();
                clipQueue.Clear();
            }
        }
    }
}