using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/SimpleWithVolume")]
public class SimpleVolumeAudioEvent : SimpleAudioEvent
{
    [MinMaxRange(0, 1)]
    public RangedFloat volume = new RangedFloat(1,1);

    [MinMaxRange(0, 2)]
    public RangedFloat pitch = new RangedFloat(1, 1);

    public override void Play(AudioSource source)
    {
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        base.Play(source);
    }
}