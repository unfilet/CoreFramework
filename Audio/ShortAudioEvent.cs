using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Short")]
public class ShortAudioEvent : AudioEvent
{
    [SerializeField] AudioClip[] clips;
    [SerializeField] float cooldown = 0;

    [MinMaxRange(0, 10)]
    [SerializeField] private RangedFloat delay = new RangedFloat(0,0);
    
    bool canPlay = true;

    [MinMaxRange(0, 1)]
    public RangedFloat volume = new RangedFloat(1, 1);

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0 || !canPlay) return;

        var clip = clips[Random.Range(0, clips.Length)];
        float volumeScale = Random.Range(volume.minValue, volume.maxValue);

        PlayDelayed(source, clip, delay.Random(), volumeScale);

        if (cooldown > 0.0f)
            StartCooldown();
    }

    async void StartCooldown() {
        canPlay = false;
        await Task.Delay(TimeSpan.FromSeconds(cooldown));
        canPlay = true;
    }

    async void PlayDelayed(AudioSource source, AudioClip clip, float delay, float volumeScale)
    {
        if (delay > 0)
            await Task.Delay(TimeSpan.FromSeconds(delay));
        source?.PlayOneShot(clip, volumeScale);
    }
}
