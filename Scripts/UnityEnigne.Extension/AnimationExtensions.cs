using UnityEngine;
using System.Collections;

public static class AnimationExtensions
{
    public static IEnumerator WhilePlaying( this Animation animation, System.Action animationComplete = null, string clipName = null )
    {
        do
        {
			yield return new WaitForEndOfFrame();
		} while ( ( clipName != null ? animation.IsPlaying(clipName) : animation.isPlaying ) );

        if (animationComplete != null) animationComplete();
        animationComplete = null;
        
    }

    public static IEnumerator WhilePlaying( this AudioSource audio, System.Action audioComplete = null, string clipName = null )
    {
        if (clipName == null) clipName = audio.clip.name;
        
        do
        {
			yield return new WaitForEndOfFrame();
        } while ( audio.isPlaying && audio.clip.name == clipName );

        audioComplete?.Invoke();
        
    }
}