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
    bool canPlay = true;

    [MinMaxRange(0, 1)]
    public RangedFloat volume = new RangedFloat(1, 1);

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0 || !canPlay) return;

        var clip = clips[Random.Range(0, clips.Length)];
        float volumeScale = Random.Range(volume.minValue, volume.maxValue);
        source.PlayOneShot(clip, volumeScale);

        if (cooldown > 0.0f)
            StartCooldown();
    }

    async void StartCooldown() {
        canPlay = false;
        await Task.Delay(TimeSpan.FromSeconds(cooldown));
        canPlay = true;
    }
}
