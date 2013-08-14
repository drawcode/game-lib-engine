using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public class AudioSystem : MonoBehaviour {
    public static AudioSystem Instance;

    public GameObject globalTranform;

    public AudioSource currentLoop;
    public AudioSource currentGameLoop;
    public AudioSource currentIntro;

    public AudioSource[] currentGameLoopLap;

    public AudioClip currentLoopClip;
    public AudioClip currentGameLoopClip;
    public AudioClip currentIntroClip;

    public AudioClip[] currentGameClipLap;

    public Dictionary<string, AudioSource> effects;
    public Dictionary<string, AudioClip> clips;

    public double effectSoundVolume = 1.0;
    public double musicSoundVolume = 0.5;

    public bool ambienceActive = false;

    public int soundIncrement = 0;

    public bool isAudiocurrentLoopPlaying {
        get {
            return currentLoop.isPlaying;
        }
    }

    public bool isAudiocurrentIntroPlaying {
        get {
            return currentLoop.isPlaying;
        }
    }

    public bool isAudioPlaying {
        get {
            return isAudiocurrentLoopPlaying || isAudiocurrentIntroPlaying;
        }
    }

    public bool isAudioAmbienceActive {
        get {
            return ambienceActive;
        }
    }

    public void Awake() {
        if (Instance != null && this != Instance) {

            //There is already a copy of this script running
            Destroy(this);
            return;
        }

        Instance = this;

        ambienceActive = false;
        DontDestroyOnLoad(gameObject);

        currentGameClipLap = new AudioClip[4];
        currentGameLoopLap = new AudioSource[4];
    }

    public void Start() {

        //
    }

    public void CheckAmbiencePlaying() {
        if (currentLoop != null) {
            if (!currentLoop.isPlaying)
                StartAmbience();
        }

        if (!ambienceActive) {
            StartAmbience();
        }
    }

    public void StartAmbience() {
        StartCoroutine(StartAmbienceCoroutine());
    }

    public void StartGameLoopsForLaps() {
        StartCoroutine(StartGameLoopsForLapsCoroutine());
    }

    public void SetEffectsVolume(double volume) {
        effectSoundVolume = volume;
    }

    public double GetEffectsVolume() {
        return effectSoundVolume;
    }

    public void SetAmbienceVolume(double volume) {
        musicSoundVolume = volume / 3;

        LogUtil.Log("Sounds::SetAmbienceVolume::" + musicSoundVolume);

        if (currentIntro != null)
            currentIntro.volume = (float)musicSoundVolume;

        if (currentLoop != null)
            currentLoop.volume = (float)musicSoundVolume;

        if (currentGameLoop != null)
            currentGameLoop.volume = (float)musicSoundVolume;
    }

    public double GetAmbienceVolume() {
        return musicSoundVolume;
    }

    public void PlayEffect(string name) {
        PlayEffect(name, 1f);
    }

	public void PlayEffect(Transform parentTransform, string name, bool loop, float volume) {
		PlayFileFromResources(parentTransform, "Audio/" + name, loop, soundIncrement, volume);
	}

	public void PlayEffect(Transform parentTransform, string name, float volume) {
		PlayFileFromResources(parentTransform, "Audio/" + name, false, soundIncrement, volume);
	}

    public void PlayEffect(string name, float volume) {

        // TODO, lookup filename from sound list...
        soundIncrement++;
        PlayFileFromResources("Audio/" + name, false, soundIncrement, volume);
    }

    public void PlayEffectIncrement(AudioClip clip, float volume, int increment) {
        PlayAudioClip(clip, false, increment, volume);
    }

    public void PlayEffect(AudioClip clip, float volume) {
        soundIncrement++;
        PlayAudioClip(clip, false, soundIncrement, volume);
    }

    public void PlayEffect(AudioClip clip, bool loop, float volume) {
        soundIncrement++;
        PlayAudioClip(clip, loop, soundIncrement, volume);
    }

    public void PlayEffectSingle() {
        PlayFileFromResources("Audio/" + name, false);
    }

    public void PlayUIMainLoop(string name, float volume) {

        // TODO, lookup filename from sound list...
        PrepareLoopFileFromResources(name, true, volume);
        currentLoop.volume = volume;
        currentLoop.Play();
    }

    public void PlayGameMainLoop(string name, float volume) {

        // TODO, lookup filename from sound list...
        PrepareGameLoopFileFromResources(name, true, volume);
        currentGameLoop.volume = volume;
        currentGameLoop.Play();
    }

    public void PlayIntro(string name, float volume) {

        // TODO, lookup filename from sound list...
        PrepareIntroFileFromResources(name, true, volume);
        currentIntro.Play();
    }

    public void PrepareIntroFileFromResources(string file, bool loop, double volume) { // file name without extension
        if (file.IndexOf("Audio") == -1)
            file = "Audio/" + file;
        currentIntroClip = LoadClipFromResources(file);
        GameObject goClip = GameObject.Find(file);
        if (goClip != null) {
        }
        else {
            goClip = new GameObject(file);
            currentIntro = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            DontDestroyOnLoad(goClip);
        }
        goClip.transform.parent = FindOrCreateSoundContainer().transform;
        goClip.transform.position = Camera.mainCamera.transform.position;
        goClip.audio.clip = currentIntroClip;
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
        goClip.AddComponent<AudioDestroy>();
    }

    public GameObject FindOrCreateDisposableSoundContainer() { // disposable sounds
        string nameSoundRootName = "SoundContainerDisposable";
        GameObject goSoundRoot = GameObject.Find(nameSoundRootName);
        if (goSoundRoot == null) {
            goSoundRoot = new GameObject(nameSoundRootName);
        }
        return goSoundRoot;
    }

    public GameObject FindOrCreateSoundContainer() {
        string nameSoundRootName = "SoundContainer";
        GameObject goSoundRoot = GameObject.Find(nameSoundRootName);
        if (goSoundRoot == null) {
            goSoundRoot = new GameObject(nameSoundRootName);
            goSoundRoot.transform.parent = FindGameGlobal();
        }
        return goSoundRoot;
    }

    public void PrepareGameLoopFileFromResources(string file, bool loop, double volume) { // file name without extension
        if (file.IndexOf("Audio") == -1)
            file = "Audio/" + file;
        currentGameLoopClip = LoadClipFromResources(file);
        GameObject goClip = GameObject.Find(file);
        if (goClip != null) {
        }
        else {
            goClip = new GameObject(file);
            currentGameLoop = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
        }
        goClip.transform.parent = FindOrCreateDisposableSoundContainer().transform;
        goClip.transform.position = Camera.mainCamera.transform.position;
        goClip.audio.clip = currentGameLoopClip;
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
    }

    public void PrepareLoopFileFromResources(string file, bool loop, float volume) { // file name without extension
        if (file.IndexOf("Audio") == -1)
            file = "Audio/" + file;
        currentLoopClip = LoadClipFromResources(file);
        GameObject goClip = GameObject.Find(file);
        if (goClip != null) {
        }
        else {
            goClip = new GameObject(file);
            currentLoop = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            DontDestroyOnLoad(gameObject);
        }
        goClip.transform.parent = FindOrCreateSoundContainer().transform;
        goClip.transform.position = Camera.mainCamera.transform.position;
        goClip.audio.clip = currentLoopClip;
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
    }

    public void PrepareGameLapLoopFileFromResources(int index, string file, bool loop, double volume) { // file name without extension
        if (file.IndexOf("Audio") == -1)
            file = "Audio/" + file;
        currentGameClipLap[index] = LoadClipFromResources(file);
        GameObject goClip = GameObject.Find(file);
        if (goClip != null) {
        }
        else {
            goClip = new GameObject(file);
            currentGameLoopLap[index] = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            DontDestroyOnLoad(gameObject);
        }
        goClip.transform.parent = FindOrCreateSoundContainer().transform;
        goClip.transform.position = Camera.mainCamera.transform.position;
        goClip.audio.clip = currentGameClipLap[index];
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
        LogUtil.Log("AudioLoaded: index:" + index + " file: " + file);
    }

    public Transform FindGameGlobal() {
        if (globalTranform == null) {
            globalTranform = GameObject.Find("_GameGlobalAll");
            if (globalTranform == null) {
                globalTranform = GameObject.Find("GameGlobal");
            }
        }

        if (globalTranform != null) {
            return globalTranform.transform;
        }

        return null;
    }

    public Transform FindAudioListenerPosition() {
        AudioListener listener = ObjectUtil.FindObject<AudioListener>();
        if (listener != null) {
            return listener.transform;
        }

        return null;
    }

    public void PlayFileFromResources(string file, bool loop) { // file name without extension
        PlayFileFromResources(file, loop, 0, 1f);
    }

	 public void PlayFileFromResources(Transform parentTransform, string file, bool loop, int increment, float volume) { // file name without extension
        AudioClip clip = LoadClipFromResources(file);
        string fileVersion = file + "-" + increment.ToString();
        GameObject goClip = GameObject.Find(fileVersion);
        if (goClip != null && increment == 0) {
        }
        else {
            goClip = new GameObject(fileVersion);
            goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            if (increment == 0) {
                DontDestroyOnLoad(goClip);
            }
            else {
                goClip.AddComponent<AudioDestroy>();
            }
        }
        goClip.transform.parent = parentTransform;
		goClip.transform.position = parentTransform.position;
        goClip.audio.clip = clip;
        goClip.audio.loop = loop;
		goClip.audio.minDistance = 5f;

        goClip.audio.volume = volume;
        goClip.audio.playOnAwake = false;
        goClip.AddComponent<AudioDestroy>();
        goClip.audio.Play();
    }

    public void PlayFileFromResources(string file, bool loop, int increment, float volume) { // file name without extension
        AudioClip clip = LoadClipFromResources(file);
        string fileVersion = file + "-" + increment.ToString();
        GameObject goClip = GameObject.Find(fileVersion);
        if (goClip != null && increment == 0) {
        }
        else {
            goClip = new GameObject(fileVersion);
            goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            if (increment == 0) {
                DontDestroyOnLoad(goClip);
            }
            else {
                goClip.AddComponent<AudioDestroy>();
            }
        }
        goClip.transform.parent = FindOrCreateDisposableSoundContainer().transform;
        Transform positionSound = FindAudioListenerPosition();
        if (positionSound != null) {
            goClip.transform.position = positionSound.position;
        }
        goClip.audio.clip = clip;
        goClip.audio.loop = loop;
        /*
        float profileVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        if(volume == -1) {
            goClip.audio.volume = profileVolume;
        }
        else {
            if(volume > profileVolume) {
                volume = profileVolume;
            }
            goClip.audio.volume = volume;
        }*/
        goClip.audio.volume = volume;
        goClip.audio.playOnAwake = false;
        goClip.AddComponent<AudioDestroy>();
        goClip.audio.Play();
    }

    public void PlayAudioClip(AudioClip clip, bool loop, int increment, float volume) {
        PlayAudioClip(Camera.mainCamera.transform.position, FindOrCreateDisposableSoundContainer().transform, clip, loop, increment, volume);
    }

    public void PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume) {
        string fileVersion = clip.name + "-" + increment.ToString();
        Transform goClipTransform = parent.FindChild(fileVersion);
        GameObject goClip = null;
        if (goClipTransform != null) {
            goClip = goClipTransform.gameObject;
        }
        if (goClip != null && increment == 0) {
        }
        else {
            goClip = new GameObject(fileVersion);
            goClip.AddComponent<AudioSource>();
            if (increment == 0) {
                DontDestroyOnLoad(goClip);
            }
            else {
                goClip.AddComponent<AudioDestroy>();
            }
            goClip.audio.clip = clip;
        }
        goClip.transform.parent = parent;
        goClip.transform.position = pos;
        goClip.audio.loop = loop;
        /*
        float profileVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        if(volume == -1) {
            goClip.audio.volume = profileVolume;
        }
        else {
            if(volume > profileVolume) {
                volume = profileVolume;
            }
            goClip.audio.volume = volume;
        }*/
        goClip.audio.volume = volume;
        goClip.audio.playOnAwake = false;
        goClip.audio.Play();
    }

    public void PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume, float panLevel) {
        string fileVersion = clip.name + "-" + increment.ToString();
        Transform goClipTransform = parent.FindChild(fileVersion);
        GameObject goClip = null;
        if (goClipTransform != null) {
            goClip = goClipTransform.gameObject;
        }
        if (goClip != null && increment == 0) {
        }
        else {
            goClip = new GameObject(fileVersion);
            goClip.AddComponent<AudioSource>();
            if (increment == 0) {
                DontDestroyOnLoad(goClip);
            }
            else {
                goClip.AddComponent<AudioDestroy>();
            }

            goClip.audio.clip = clip;
            goClip.audio.loop = loop;
            goClip.audio.panLevel = panLevel;
            goClip.audio.maxDistance = 25;
            goClip.audio.spread = 360;
            goClip.audio.priority = 0;
            goClip.audio.rolloffMode = AudioRolloffMode.Linear;
            goClip.audio.playOnAwake = false;
        }
        /*
        float profileVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        if(volume == -1) {
            goClip.audio.volume = profileVolume;
        }
        else {
            if(volume > profileVolume) {
                volume = profileVolume;
            }
            goClip.audio.volume = volume;
        }*/
        goClip.audio.volume = volume;
        goClip.transform.parent = parent;
        goClip.transform.position = pos;
        goClip.audio.Play();
    }

    public void PlayAudioClipOneShot(AudioClip clip) {
        audio.PlayOneShot(clip);
    }

    public void PlayAudioAtLocation(AudioClip clip, Vector3 location) {
        AudioSource.PlayClipAtPoint(clip, location);
    }

    public AudioClip LoadLoop(string name) {
        AudioClip clip = LoadClipFromResources("Audio/" + name);
        return clip;
    }

    public AudioClip LoadClipFromResources(string fileName) {
        AudioClip clip = Resources.Load(fileName) as AudioClip;
        return clip;
    }

    /*
    public AudioClip LoadAudioFileAtPath( string file, Action<string> onFailure, Action<AudioClip> onSuccess )
    {
        AudioRecorderBinding.loadAudioFileAtPath(file, onFailure, onSuccess);
    }
    */

    public IEnumerator StartAmbienceCoroutine() {

		if(!ambienceActive) {

			ambienceActive = true;

	        if (currentGameLoop != null) {
	            if (currentGameLoop.isPlaying) {
	                currentGameLoop.volume = 0;
	                currentGameLoop.Stop();
	            }
	        }
	
	        if (currentIntro != null && ambienceActive) {
		        currentIntro.volume = (float)musicSoundVolume;
		        currentIntro.Play();
	            yield return new WaitForSeconds(currentIntro.clip.length - 1f);
	            currentIntro.gameObject.AudioTo(0f, 1f, 1f, 0f);
			}
	
	        if (currentLoop != null && ambienceActive) {

	            currentLoop.volume = (float)musicSoundVolume;
	            currentLoop.Play();
	            currentLoop.gameObject.AudioTo((float)musicSoundVolume, 1f, 1f, 0f);
	        }
		}
		else {
			yield break;
		}
    }

    public IEnumerator StartGameLoopsForLapsCoroutine() {
        foreach (AudioSource source in currentGameLoopLap) {
            if (source != null) {
                source.volume = 0f;
                if (!source.isPlaying) {
                    source.Play();
                }
            }
        }

        yield return new WaitForSeconds(.3f);
    }

    public void StartGameLoopForLap(int lap) {

        // TODO fix this hack..,

        float volumeLevel = (float)musicSoundVolume * 1.7f;

        if (lap == 1) {
            if (currentGameLoopLap[0].isPlaying) {
                currentGameLoopLap[0].gameObject.AudioTo(volumeLevel, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[1].isPlaying) {
                currentGameLoopLap[1].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[2].isPlaying) {
                currentGameLoopLap[2].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[3].isPlaying) {
                currentGameLoopLap[3].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }
        }
        else if (lap == 2) {
            if (currentGameLoopLap[0].isPlaying) {
                currentGameLoopLap[0].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[1].isPlaying) {
                currentGameLoopLap[1].gameObject.AudioTo(volumeLevel, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[2].isPlaying) {
                currentGameLoopLap[2].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[3].isPlaying) {
                currentGameLoopLap[3].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }
        }
        else if (lap == 3) {
            if (currentGameLoopLap[0].isPlaying) {
                currentGameLoopLap[0].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[1].isPlaying) {
                currentGameLoopLap[1].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[2].isPlaying) {
                currentGameLoopLap[2].gameObject.AudioTo(volumeLevel, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[3].isPlaying) {
                currentGameLoopLap[3].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }
        }
        else if (lap == 4) {
            if (currentGameLoopLap[0].isPlaying) {
                currentGameLoopLap[0].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[1].isPlaying) {
                currentGameLoopLap[1].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[2].isPlaying) {
                currentGameLoopLap[2].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }

            if (currentGameLoopLap[3].isPlaying) {
                currentGameLoopLap[3].gameObject.AudioTo(volumeLevel, 1f, .5f, 0f);
            }
        }
        else {
            if (currentGameLoopLap[0].isPlaying) {
                currentGameLoopLap[0].gameObject.AudioTo(0f, 1f, 2f, 0f);
            }

            if (currentGameLoopLap[1].isPlaying) {
                currentGameLoopLap[1].gameObject.AudioTo(0f, 1f, 2f, 0f);
            }

            if (currentGameLoopLap[2].isPlaying) {
                currentGameLoopLap[2].gameObject.AudioTo(0f, 1f, 2f, 0f);
            }

            if (currentGameLoopLap[3].isPlaying) {
                currentGameLoopLap[3].gameObject.AudioTo(0f, 1f, .5f, 0f);
            }
        }
    }

    public void StopAmbience() {
        StartCoroutine(StopAmbienceRoutine());
    }

    private IEnumerator StopAmbienceRoutine() {
		if(ambienceActive) {

			ambienceActive = false;

	        if (currentLoop != null) {
	            if (currentLoop.audio.isPlaying) {
	                currentLoop.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
	            }
	        }
	
	        if (currentIntro != null) {
	            if (currentIntro.audio.isPlaying) {
	                currentIntro.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
	            }
	        }

	        yield return new WaitForSeconds(1.5f);

	        if (currentIntro != null) {
	            currentIntro.Stop();
			}
	
	        if (currentLoop != null) {
	            currentLoop.Stop();
			}

		}
		else {
			yield break;
		}
    }
}