using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple Fade")]
public class SimpleFadeAudioEvent : SimpleVolumeAudioEvent
{
    [Min(0.01f),]
    public float fadeSpeed = 1f;
    private IEnumerator enumerator = null;

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0 || !source) return;

        var aClip = RandomClip();
        var aVolume = Random.Range(volume.minValue, volume.maxValue);
        var aPitch = Random.Range(pitch.minValue, pitch.maxValue);

        if (source.clip == aClip && source.isPlaying && enumerator == null)
            return;

        var mb = source.GetOrAddComponent<CanvasGroupExt.EmptyMonoBehaviour>();
        mb.StopAllCoroutines();
        mb.StartCoroutine(enumerator = fadeIn(source, aClip, 
            fadeSpeed, aVolume, aPitch, loop));
    }

    IEnumerator fadeIn(AudioSource source, AudioClip clip, float speed, float volume, float pitch, bool loop) 
    {
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
            source.loop = loop;
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
        }
    }
}