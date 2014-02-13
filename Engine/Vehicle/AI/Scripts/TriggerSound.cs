using UnityEngine;
using System.Collections;

public class TriggerSound : MonoBehaviour
{
    public string tagName1="";
    public string tagName2 = "";
    public AudioClip triggerSound;
    public float soundVolume = 1.0f;

    private AudioSource triggerAudioSource;

    void Awake()
    {
        InitSound(out triggerAudioSource, triggerSound, soundVolume, false);
    }

	
    void InitSound(out AudioSource myAudioSource, AudioClip myClip, float myVolume, bool looping)
    {
        myAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
        myAudioSource.playOnAwake = false;
        myAudioSource.clip = myClip;
        myAudioSource.loop = looping;
        myAudioSource.volume = myVolume;
        //myAudioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void OnTriggerEnter(Collider other) 
    {
        //if (other.gameObject.tag == tagName1 || other.gameObject.tag == tagName2) //2013-08-02
        if (other.gameObject.CompareTag(tagName1) || other.gameObject.CompareTag(tagName2)) //2013-08-02
        {
			if (other.gameObject.layer != 2) //2011-12-27
            	triggerAudioSource.Play();
        }

    }

}
