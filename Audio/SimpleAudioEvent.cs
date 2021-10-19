using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public bool loop = false;

    public AudioClip[] clips;
    
    public AudioClip RandomClip() => clips.Random();

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0 || !source) return;

        source.loop = loop;
        source.clip = RandomClip();
        source.Play();
    }
}