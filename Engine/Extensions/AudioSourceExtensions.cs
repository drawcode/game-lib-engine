using System;
using System.Collections;
using UnityEngine;

public static class AudioSourceExtensions {

    public static void StopIfPlaying(this AudioSource audioSource) {
        if(audioSource == null) {
            return;
        }

        if(audioSource.audio == null) {
            return;
        }

        if(audioSource.isPlaying) {
            audioSource.Stop();
        }
    }    
    
    public static void FadeOut(this AudioSource audioSource, float duration) {
        CoroutineUtil.Start(audioSource.fadeOut(duration, () => audioSource.StopIfPlaying()));
    }

    public static IEnumerator fadeOut(this AudioSource audioSource, float duration, Action onComplete) {
        float startingVolume = audioSource.volume;

        // fade out the volume
        while (audioSource.volume > 0.0f) {
            audioSource.volume -= Time.deltaTime * startingVolume / duration;
            yield return null;
        }

        // all done fading out
        if (onComplete != null)
            onComplete();
    }

    public static void FadeIn(this AudioSource audioSource, float volumeTo, float duration) {
        CoroutineUtil.Start(audioSource.fadeIn(volumeTo, duration, null));
    }

    public static IEnumerator fadeIn(this AudioSource audioSource, float volumeTo, float duration, Action onComplete) {

        audioSource.volume = 0f;

        audioSource.Play();

        //float startingVolume = audioSource.volume;
        float endingVolume = volumeTo;
        
        // fade out the volume
        while (audioSource.volume < volumeTo) {
            audioSource.volume += Time.deltaTime * endingVolume / duration;
            yield return null;
        }
        
        // all done fading out
        if (onComplete != null)
            onComplete();
    }
	
}