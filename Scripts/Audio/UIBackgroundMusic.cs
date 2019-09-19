using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Core.Scripts.Audio;

public class UIBackgroundMusic : MonoBehaviour
{
    [Inject] AudioManager _audioManager;

    [SerializeField] AudioEvent backgroundMusic;

    private void Start()
    {
        _audioManager.PlayMusic(backgroundMusic);
    }

    private void OnEnable()
    {
        AudioManager.OnMusicChanged += AudioManager_OnMusicChanged;
    }

    private void OnDisable()
    {
        AudioManager.OnMusicChanged -= AudioManager_OnMusicChanged;
    }

    void AudioManager_OnMusicChanged(bool obj)
    {
        if (obj) Start();

    }

}
