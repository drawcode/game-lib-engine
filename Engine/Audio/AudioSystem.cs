using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public class AudioSetItem {
	public AudioSource audioSource;
	public AudioClip audioClip;
	public string file = "";
}

public class AudioSystem : MonoBehaviour {
    public static AudioSystem Instance;

    public GameObject globalTranform;

    public AudioSource currentLoop;
    public AudioSource currentGameLoop;
    public AudioSource currentIntro;


    public AudioClip currentLoopClip;
    public AudioClip currentGameLoopClip;
    public AudioClip currentIntroClip;
    
    public List<AudioSource> currentGameLoops;
    public List<AudioClip> currentGameClips;

    public Dictionary<string, AudioSetItem> clips;

    public double effectSoundVolume = 1.0;
    public double musicSoundVolume = 0.5;

    public bool ambienceActive = false;

    public int soundIncrement = 0;
    
    int currentLoopIndex = 0;
	
	public string audioRootPath {
		get {			
			return Contents.appCacheVersionSharedAudio; 
		}
	}

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

        currentGameClips = new List<AudioClip>();
        currentGameLoops = new List<AudioSource>();
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
        musicSoundVolume = volume;

        LogUtil.Log("Sounds::SetAmbienceVolume::" + musicSoundVolume);

        if (currentIntro != null) {
            currentIntro.volume = (float)musicSoundVolume;
        }
         
        if (currentLoop != null) {
            currentLoop.volume = (float)musicSoundVolume;
        }

        if (currentGameLoop != null) {
            currentGameLoop.volume = (float)musicSoundVolume;
        }

        if(currentGameLoops != null) {
            if(currentGameLoops.Count > 0 
               && currentGameLoops.Count > currentLoopIndex) {
                if(currentGameLoops[currentLoopIndex] != null) {
                    currentGameLoops[currentLoopIndex].volume = (float)musicSoundVolume;
                }
            }
        }
    }

    public double GetAmbienceVolume() {
        return musicSoundVolume;
    }

    public GameObject PlayEffectObject(Transform parentTransform, string name, bool loop, float volume) {
       return PlayFileFromResourcesObject(parentTransform, 
                              audioRootPath + name, loop, soundIncrement, volume);
    }

    public void PlayEffect(string name) {
        PlayEffect(name, 1f);
    }

	public void PlayEffect(Transform parentTransform, string name, bool loop, float volume) {
		PlayFileFromResources(parentTransform, 
			audioRootPath + name, loop, soundIncrement, volume);
	}

	public void PlayEffect(Transform parentTransform, string name, float volume) {
		PlayFileFromResources(parentTransform, 
			audioRootPath + name, false, soundIncrement, volume);
	}

    public void PlayEffect(string name, float volume) {

        // TODO, lookup filename from sound list...
        soundIncrement++;
        PlayFileFromResources(
			audioRootPath + name, false, soundIncrement, volume);
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
        PlayFileFromResources(audioRootPath + name, false);
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
	
	public void PlayAudioFile(string file) {
		AudioSetItem item = GetAudioFile(file);
		if(item != null) {
			item.audioSource.Play();
		}
	}
	
	public AudioSetItem GetAudioFile(string file) {
		if(clips.ContainsKey(file)) {
			return clips[file];
		}
		return null;
	}
	
    public AudioSetItem PrepareAudioFileFromResources(string file, bool loop, double volume, bool destroyOnComplete) { // file name without extension
        file = audioRootPath + file;
		
        AudioClip audioClip = LoadClipFromResources(file);
        GameObject goClip = GameObject.Find(file);
        if (goClip == null) {
						
            goClip = new GameObject(file);
            AudioSource audioSource = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            DontDestroyOnLoad(goClip);
			
	        goClip.transform.parent = FindOrCreateSoundContainer().transform;
	        goClip.transform.position = Camera.main.transform.position;
	        goClip.audio.clip = audioClip;
	        goClip.audio.loop = loop;
	        goClip.audio.volume = (float)volume;
	        goClip.audio.playOnAwake = false;
			
			if(destroyOnComplete) {
	        	goClip.AddComponent<AudioDestroy>();
			}
			
			if(clips == null) {
				clips = new Dictionary<string, AudioSetItem>();
			}
			
			AudioSetItem item = new AudioSetItem();			
			item.audioClip = audioClip;
			item.audioSource = audioSource;
			item.file = file;
							
			if(clips.ContainsKey(file)) {
				item = clips[file];
				clips[file] = item;
			}
			else {
				clips.Add(file, item);
			}
			
			return item;
        }
		
		return null;
    }

    public void PrepareIntroFileFromResources(string file, bool loop, double volume) { // file name without extension
        file = audioRootPath + file;
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
        goClip.transform.position = Camera.main.transform.position;
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
        file = audioRootPath + file;
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
        goClip.transform.position = Camera.main.transform.position;
        goClip.audio.clip = currentGameLoopClip;
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
    }

    public void PrepareLoopFileFromResources(string file, bool loop, float volume) { // file name without extension
        file = audioRootPath + file;
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
        goClip.transform.position = Camera.main.transform.position;
        goClip.audio.clip = currentLoopClip;
        goClip.audio.loop = loop;
        goClip.audio.volume = (float)volume;
        goClip.audio.playOnAwake = false;
    }

    public void PrepareGameLapLoopFileFromResources(int index, string file, bool loop, double volume) { // file name without extension
        file = audioRootPath  + file;
        currentGameClips.Insert(index, LoadClipFromResources(file));
        GameObject goClip = GameObject.Find(file);
        if (goClip != null) {
        }
        else {
            goClip = new GameObject(file);
            currentGameLoops[index] = goClip.AddComponent<AudioSource>();
            goClip.transform.parent = FindGameGlobal();
            DontDestroyOnLoad(gameObject);
        }
        goClip.transform.parent = FindOrCreateSoundContainer().transform;
        goClip.transform.position = Camera.main.transform.position;
        goClip.audio.clip = currentGameClips[index];
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
        PlayFileFromResourcesObject(parentTransform, file, loop, increment, volume);
    }

    public GameObject PlayFileFromResourcesObject(Transform parentTransform, string file, bool loop, int increment, float volume) { // file name without extension
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
        goClip.audio.Play();

        return goClip;
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
        //goClip.AddComponent<AudioDestroy>();
        goClip.audio.Play();
    }

    public void PlayAudioClip(AudioClip clip, bool loop, int increment, float volume) {
        PlayAudioClip(Camera.main.transform.position, FindOrCreateDisposableSoundContainer().transform, clip, loop, increment, volume);
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
    
    public GameObject PlayAudioClipGameObject(Transform parent, AudioClip clip, bool loop, float volume) {
        return PlayAudioClipGameObject(parent.transform.position, parent, clip, loop, 0, volume);
    }

    public GameObject PlayAudioClipGameObject(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume) {
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
        goClip.audio.volume = volume;
        goClip.audio.playOnAwake = true;
        goClip.audio.Play();

        return goClip;
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
        AudioClip clip = LoadClipFromResources(audioRootPath + name);
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

	        if (currentGameLoop != null) {
	            if (currentGameLoop.isPlaying) {
	                currentGameLoop.volume = 0;
	                currentGameLoop.Stop();
	            }
	        }
	
	        if (currentIntro != null) {
		        currentIntro.volume = (float)musicSoundVolume;
		        currentIntro.Play();
				//if(currentIntro.clip != null) { 
	            	yield return new WaitForSeconds(currentIntro.clip.length - 1f);
				//}
	            currentIntro.gameObject.AudioTo(0f, 1f, 1f, 0f);
			}
			
			ambienceActive = true;
	
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
        foreach (AudioSource source in currentGameLoops) {
            if (source != null) {
                source.volume = 0f;
                if (!source.isPlaying) {
                    source.Play();
                }
            }
        }

        yield return new WaitForSeconds(.3f);
    }

    public void StartGameLoop(int loop) {

        // TODO fix this hack..,

        currentLoopIndex = loop - 1;

        float volumeLevel = (float)musicSoundVolume;


        for(int i = 0; i < currentGameLoops.Count; i++) {

            float currentVolumeLevel = 0f;
            bool isCurrent = currentLoopIndex == i ? true : false;

            if(isCurrent) {
                currentVolumeLevel = volumeLevel;
            }

            if(isCurrent) {
                if (!currentGameLoops[i].isPlaying) {
                    currentGameLoops[i].Play();
                    currentGameLoops[i].time = 0f; // TODO for laps or syned get time of last loop
                    currentGameLoops[i].volume = 0f;
                }
                if(currentGameLoops[i].isPlaying) {
                    currentGameLoops[i].gameObject.AudioTo(currentVolumeLevel, 1f, .1f, 0f);
                }
            }
            else  {
                currentGameLoops[i].gameObject.AudioTo(currentVolumeLevel, 0f, .1f, 0f);
                currentGameLoops[i].StopIfPlaying();
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