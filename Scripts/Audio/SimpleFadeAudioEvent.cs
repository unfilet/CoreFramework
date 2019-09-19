using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple Fade")]
public class SimpleFadeAudioEvent : AudioEvent
{
    public AudioClip[] clips;

    [MinMaxRange(0, 1)]
    public RangedFloat volume = new RangedFloat(1,1);
    [MinMaxRange(0, 2)]
    public RangedFloat pitch = new RangedFloat(1, 1);

    private IEnumerator enumerator = null;

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        var aClip = clips[Random.Range(0, clips.Length)];
        var aVolume = Random.Range(volume.minValue, volume.maxValue);
        var aPitch = Random.Range(pitch.minValue, pitch.maxValue);

        var mb = source.GetOrAddComponent<MonoBehaviour>();
        if (enumerator != null) mb.StopCoroutine(enumerator);
        mb.StartCoroutine(enumerator = fadeIn(source, aClip, aVolume,aPitch));
    }

    IEnumerator fadeIn(AudioSource source, AudioClip clip, float volume, float pitch) 
    {
        float speed = 1.5f;

        while (source.isPlaying && source.volume > 0)
        {
            source.volume -= speed * Time.deltaTime;
            yield return null;
        }

        if (clip != null)
        {
            source.volume = 0;
            source.clip = clip;
            source.pitch = pitch;
            source.Play();

            while (source.volume < volume)
            {
                source.volume += speed * Time.deltaTime;
                yield return null;
            }
            source.volume = volume;
        }
        else
        {
            source.Stop();
            source.volume = 1;
            yield break;
        }
    }
}