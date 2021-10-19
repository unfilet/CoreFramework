using Zenject;
using Core.Scripts.Audio;
using UnityEngine;

public class UIBackgroundMusic : MonoBehaviour
{
    private enum Functions
    {
        None,
        Awake,
        OnEnable,
        OnDisable,
        Start,
    }

    [SerializeField] AudioEvent backgroundMusic = null;
    [SerializeField] Functions executeFunction = Functions.Start;
    [SerializeField] bool isSFX = false;
    
    private IAudioManager _audioManager;

    [Inject]
    private void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake() => Play(Functions.Awake);
    private void Start() => Play(Functions.Start);
    private void OnEnable() => Play(Functions.OnEnable);
    private void OnDisable() => Play(Functions.OnDisable);

    private void Play(Functions caller)
    {
        if (executeFunction != caller) return;
        Play();
    }

    public void Play() => Play(backgroundMusic);

    public void Play(AudioEvent audio)
    {
        if (isSFX)
            _audioManager.PlaySFX(audio);
        else
            _audioManager.PlayMusic(audio);
    }
}
