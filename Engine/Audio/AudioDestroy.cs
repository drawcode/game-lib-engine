using System;
using System.Collections;
using Engine.Utility;
using UnityEngine;

public class AudioDestroy : GameObjectBehavior {
    public float afterTimeDefault = 5.5f;
    public float clipLength = 5.5f;
    AudioSource audioSource;

    // Use this for initialization
    private void Start() {
        
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = true;
        gameObject.SetActive(true);

        Reset();
    }

    public void Reset() {
        float audioLength = afterTimeDefault;
        if (audioSource != null) {
            if (audioSource.clip != null 
                && audioSource.isActiveAndEnabled){
                clipLength = audioSource.clip.length;
                //LogUtil.LogAudio("DestroySound audio.clip.length:" + audio.clip.length);
                if (audioSource.clip.length > 0) {
                    audioLength = audioSource.clip.length + 1;
                    StartCoroutine(DestroySound(audioLength));    
                }
            }
        }
    }

    private IEnumerator DestroySound(float afterTime) {
        //LogUtil.LogAudio("DestroySound afterTime:" + afterTime);
        yield return new WaitForSeconds(afterTime);
        if (audioSource != null) {
            if (audioSource.clip != null) {
                if (audioSource.isPlaying) {
                    audioSource.Stop();

                    //DestroyImmediate(gameObject.audio.clip);
                }
            }
        }
        audioSource.enabled = false;
        gameObject.SetActive(false);
        GameObjectHelper.DestroyGameObject(gameObject);
    }
}