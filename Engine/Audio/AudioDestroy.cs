using System;
using System.Collections;
using Engine.Utility;
using UnityEngine;

public class AudioDestroy : GameObjectBehavior {
    public float afterTimeDefault = 5.5f;
    public float clipLength = 5.5f;

    // Use this for initialization
    private void Start() {
        float audioLength = afterTimeDefault;
        if (audio != null) {
            if (audio.clip != null) {
                clipLength = audio.clip.length;
                //LogUtil.LogAudio("DestroySound audio.clip.length:" + audio.clip.length);
                if (audio.clip.length > 0) {
                    audioLength = audio.clip.length + 1;
                }
            }
        }
        StartCoroutine(DestroySound(audioLength));
    }

    private IEnumerator DestroySound(float afterTime) {
        //LogUtil.LogAudio("DestroySound afterTime:" + afterTime);
        yield return new WaitForSeconds(afterTime);
        if (gameObject.audio != null) {
            if (gameObject.audio.clip != null) {
                if (gameObject.audio.isPlaying) {
                    gameObject.audio.Stop();

                    //DestroyImmediate(gameObject.audio.clip);
                }
            }
        }
        gameObject.audio.enabled = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}