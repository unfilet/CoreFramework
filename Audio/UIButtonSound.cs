﻿using Zenject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Core.Scripts.Audio;

[DisallowMultipleComponent]
public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    private Button button;

    [SerializeField] AudioEvent audioEvent;

    private IAudioManager _audioManager;

    [Inject]
    private void Construct(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake() => 
        this.button = GetComponent<Button>();

    private void OnEnable() =>
        button?.onClick.AddListener(PlaySound);

    private void OnDisable() =>
        button?.onClick.RemoveListener(PlaySound);


    public void OnPointerClick(PointerEventData eventData)
    {
        if (button == null) PlaySound();
    }

    private void PlaySound()
    {
        _audioManager.PlaySFX(audioEvent);
    }
}
