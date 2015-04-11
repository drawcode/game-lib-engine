using UnityEngine;
using System.Collections;

public class TriggerSound : GameObjectBehavior {
    public string tagName1 = "";
    public string tagName2 = "";
    public AudioClip triggerSound;
    public float soundVolume = 1.0f;
    //private AudioSource triggerAudioSource;

    void Awake() {
        //InitSound(out triggerAudioSource, triggerSound, soundVolume, false);
    }
    
    void InitSound(out AudioSource audioSource, AudioClip clip, float volume, bool looping) {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clip;
        audioSource.loop = looping;
        audioSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        //myAudioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void OnTriggerEnter(Collider other) {
        //if (other.gameObject.tag == tagName1 || other.gameObject.tag == tagName2) //2013-08-02

        // TODO sound trigger
        //if (other.gameObject.CompareTag(tagName1) || other.gameObject.CompareTag(tagName2)) { //2013-08-02
        //    if (other.gameObject.layer != 2) //2011-12-27
        //        triggerAudioSource.Play();
        //}

    }

}
