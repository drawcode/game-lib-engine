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
	
	
}