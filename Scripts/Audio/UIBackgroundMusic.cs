using Zenject;
using Core.Scripts.Audio;
using UnityEngine;

public class UIBackgroundMusic : MonoBehaviour
{
    private enum Functions
    {
        Awake,
        OnEnable,
        OnDisable,
        Start
    }

    [Inject] AudioManager _audioManager;

    [SerializeField] AudioEvent backgroundMusic = null;
    [SerializeField] Functions executeFunction = Functions.Start;


    private void Awake()
    {
        if (executeFunction == Functions.Awake)
            _audioManager.PlayMusic(backgroundMusic);
    }

    private void Start()
    {
        if (executeFunction == Functions.Start)
            _audioManager.PlayMusic(backgroundMusic);
    }

    private void OnEnable()
    {
        if (executeFunction == Functions.OnEnable)
            _audioManager.PlayMusic(backgroundMusic);
    }

    private void OnDisable()
    {
        if (executeFunction == Functions.OnDisable)
            _audioManager.PlayMusic(backgroundMusic);
    }
}
