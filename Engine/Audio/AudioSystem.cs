#pragma warning disable 0618
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Utility;
using UnityEngine;

public class AudioSetItem {
    public AudioSource audioSource;
    public AudioClip audioClip;
    public string file = "";
    public string type = "";
}

public class AudioActionObject {
    
    public Vector3 pos;
    public Transform parent;
    public AudioClip clip;
    public bool loop;
    public int increment;
    public float volume;
    public bool startPlaying;
    public float panLevel;
    public float minDistance;
    
    public AudioActionObject() {
        Reset();
    }
    
    public void Reset() {
        
        pos = Vector3.zero;        
        parent = null;        
        clip = null;        
        loop = false;        
        increment = 1;     
        volume = 1.0f;        
        startPlaying = true;
        panLevel = 0.0f;
        minDistance = 5.0f;
    }
}

public class AudioSystem : GameObjectBehavior {

    public static AudioSystem Instance;
    public GameObject prefabAudioItem;
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
    public Dictionary<string, AudioSetItem> audioSetItems;
    AudioListener listener;
    public bool dataDrivenLoad = true;

    public string audioRootPath {
        get {           
            return ContentPaths.appCacheVersionSharedAudio; 
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

        //if(!dataDrivenLoad) {
        ambienceActive = false;
        DontDestroyOnLoad(gameObject);

        currentGameClips = new List<AudioClip>();
        currentGameLoops = new List<AudioSource>();
        //}
    }

    public void Start() {

        //
    }

    
    
    // AMBIENCE / GAME NEW
    
    public void PlaySoundByPreset(string presetCode) {
        
        GamePreset preset = GamePresets.Get(presetCode);
        if (preset != null) {
            //preset.getI
        }
            
            
    }
    
    // AMBIENCE OLD

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

        if (currentGameLoops != null) {
            if (currentGameLoops.Count > 0 
                && currentGameLoops.Count > currentLoopIndex
                && currentLoopIndex > -1) {
                if (currentGameLoops[currentLoopIndex] != null) {
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

        PlayEffect(name, volume, false);
    }
    
    public void PlayEffect(string name, float volume, bool loop) {
        
        // TODO, lookup filename from sound list...
        soundIncrement++;
        PlayFileFromResources(
            audioRootPath + name, loop, soundIncrement, volume);
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
    
    public void PlayAudioFile(string file) {
        AudioSetItem item = GetAudioFile(file);
        if (item != null) {
            item.audioSource.Play();
        }
    }
    
    public AudioSetItem GetAudioFile(string file) {
        if (clips.ContainsKey(file)) {
            return clips[file];
        }
        return null;
    }

    public GameObject PrepareFromResources(string type, string file, bool loop, double volume) { // file name without extension

        string path = audioRootPath + file;
        string code = "game-" + type;

        AudioClip clip = LoadAudioClip(path);

        GameObject goClip = GameObject.Find(code);//FindOrCreateSoundContainer().transform.FindChild(code).gameObject;

        if (goClip != null) {
        
        }
        else {
            goClip = new GameObject(code);

            goClip.AddComponent<AudioSource>();

            GameObjectAudio obj = goClip.AddComponent<GameObjectAudio>();
            obj.code = code;
            obj.type = type;
            obj.file = file;
            obj.path = path;
        }
        goClip.transform.parent = FindOrCreateSoundContainer().transform;

        if (Camera.main != null && Camera.main.transform != null) {
            goClip.transform.position = Camera.main.transform.position;
        }
        else {
            goClip.transform.position = Vector3.zero;
        }

        AudioSource audioSource = goClip.GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = (float)volume;
        audioSource.playOnAwake = false;

        return goClip;
    }

    public GameObject FindOrCreateDisposableSoundContainer() { // disposable sounds
        string nameSoundRootName = "_SoundContainerDisposable";
        GameObject goSoundRoot = GameObject.Find(nameSoundRootName);
        if (goSoundRoot == null) {
            goSoundRoot = new GameObject(nameSoundRootName);
        }
        return goSoundRoot;
    }

    public GameObject FindOrCreateSoundContainer() {
        string nameSoundRootName = "_SoundContainer";
        GameObject goSoundRoot = GameObject.Find(nameSoundRootName);
        if (goSoundRoot == null) {
            goSoundRoot = new GameObject(nameSoundRootName);
            goSoundRoot.transform.parent = FindGameGlobal();
        }
        return goSoundRoot;
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
        if (listener == null) {
            listener = UnityObjectUtil.FindObject<AudioListener>();
        }
        if (listener != null) {
            return listener.transform;
        }

        return null;
    }

    public GameObject PlayFileFromResources(string file, bool loop) { // file name without extension
        return PlayFileFromResources(file, loop, 0, 1f);
    }

    public GameObject PlayFileFromResources(Transform parentTransform, string file, bool loop, int increment, float volume) { // file name without extension
        return PlayFileFromResourcesObject(parentTransform, file, loop, increment, volume);
    }

    public GameObject PlayFileFromResourcesObject(Transform parentTransform, string file, bool loop, int increment, float volume) { // file name without extension
        AudioClip clip = LoadAudioClip(file);

        return PlayAudioClip(parentTransform.position, parentTransform, clip, loop, increment, volume);
    }

    public GameObject PlayFileFromResources(string file, bool loop, int increment, float volume) { // file name without extension

        AudioClip clip = LoadAudioClip(file);

        if (clip == null) {
            return null;
        }

        AudioActionObject obj = new AudioActionObject();
        obj.clip = clip;
        obj.loop = loop;
        obj.increment = increment;
        obj.volume = volume;
     
        return PlayAudioClip(obj);
    }
    
    public GameObject PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume) {
        
        if (clip == null) {
            return null;
        }
        
        AudioActionObject obj = new AudioActionObject();
        obj.clip = clip;
        obj.loop = loop;
        obj.increment = increment;
        obj.volume = volume;
        obj.pos = pos;
        obj.parent = parent;
        
        return PlayAudioClip(obj);
    }

    public GameObject PlayAudioClip(AudioClip clip, bool loop, int increment, float volume) {

        if (clip == null) {
            return null;
        }

        AudioActionObject obj = new AudioActionObject();
        obj.clip = clip;
        obj.loop = loop;
        obj.increment = increment;
        obj.volume = volume;
        obj.pos = Camera.main.transform.position;
        obj.parent = FindOrCreateDisposableSoundContainer().transform;

        return PlayAudioClip(obj);
    }

    public GameObject PlayAudioClip(AudioActionObject audioActionObject) {

        if (audioActionObject == null) {
            return null;
        }

        string fileVersion = audioActionObject.clip.name + "-" + audioActionObject.increment.ToString();

        if (prefabAudioItem == null) {
            prefabAudioItem = PrefabsPool.PoolPrefab(audioRootPath + "audio-item");
        }

        GameObject goClip = GameObjectHelper.CreateGameObject(prefabAudioItem, audioActionObject.pos, Quaternion.identity, true);

        //goClip.name = fileVersion;

        if (goClip == null) {
            return null;
        }

        GameObjectData gameObjectData = goClip.GetOrSet<GameObjectData>();

        if (gameObjectData != null) {
            gameObjectData.Set("code", fileVersion);
            gameObjectData.Set("name", audioActionObject.clip.name);
        }

        AudioSource audioSourceObject = goClip.GetOrSet<AudioSource>();

        AudioDestroy audioDestroy = null;

        if (audioActionObject.increment == 0) {
            DontDestroyOnLoad(goClip);
        }
        else {
            audioDestroy = goClip.GetOrSet<AudioDestroy>();
        }

        audioSourceObject.clip = audioActionObject.clip;
        audioSourceObject.loop = audioActionObject.loop;
        audioSourceObject.volume = audioActionObject.volume;
        
        audioSourceObject.minDistance = audioActionObject.minDistance;
        
        goClip.transform.parent = audioActionObject.parent;
        goClip.transform.position = audioActionObject.pos;

        audioSourceObject.playOnAwake = false;

        if (audioDestroy != null) {
            audioDestroy.Reset();
        }

        if (audioActionObject.panLevel > 0.0) {
            audioSourceObject.maxDistance = 25;
            audioSourceObject.spread = 360;
            audioSourceObject.priority = 0;
            audioSourceObject.spatialBlend = audioActionObject.panLevel;
            audioSourceObject.rolloffMode = AudioRolloffMode.Linear;
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

        audioSourceObject.enabled = true;

        if (audioActionObject.startPlaying) {
            audioSourceObject.Play();
        }

        return goClip;
    }

    /*


    public GameObject PlayAudioClip(
        Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume, 
        bool startPlaying = true, float panLevel = 0.0f, float minDistance = 5f) {

        string fileVersion = clip.name + "-" + increment.ToString();

        Transform goClipTransform = parent.FindChild(fileVersion);

        GameObject goClip = null;

        if (goClipTransform != null) {
            goClip = goClipTransform.gameObject;
        }

        if (goClip != null && increment == 0) {
            goClip.audio.clip = clip;
            goClip.audio.loop = loop;
        }
        else {
            if(prefabAudioItem == null) {
                prefabAudioItem = AssetUtil.LoadAsset<UnityEngine.Object>(audioRootPath + "audio-item");
            }

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
            if(panLevel > 0.0) {
                goClip.audio.maxDistance = 25;
                goClip.audio.spread = 360;
                goClip.audio.priority = 0;
                goClip.audio.panLevel = panLevel;
                goClip.audio.rolloffMode = AudioRolloffMode.Linear;
            }
        }goClip.audio.volume = volume;
        }
    goClip.audio.clip = clip;
    goClip.audio.loop = loop;
    goClip.audio.volume = volume;
    
    goClip.audio.minDistance = minDistance;
    
    goClip.transform.parent = parent;
    goClip.transform.position = pos;
    
    goClip.audio.playOnAwake = false;
    
    if(startPlaying) {
        goClip.audio.Play();
    }
    
    return goClip;
}


     * */

    public void PlayAudioClipOneShot(AudioClip clip) {
        audio.PlayOneShot(clip);
    }

    public void PlayAudioAtLocation(AudioClip clip, Vector3 location) {
        AudioSource.PlayClipAtPoint(clip, location);
    }

    public AudioClip LoadLoop(string name) {
        AudioClip clip = LoadAudioClip(audioRootPath + name);
        return clip;
    }
        
    public AudioClip LoadAudioClip(string path) {
        AudioSetItem audioSetItem = LoadAudioSetItem(path);

        if (audioSetItem != null) {
            return audioSetItem.audioClip;
        }

        return null;
    }

    public void ClearAudioItems() {
        if (audioSetItems == null) {
            return;
        }

        audioSetItems.Clear();
    }

    public AudioSetItem LoadAudioSetItem(string path) {

        if (audioSetItems == null) {
            audioSetItems = new Dictionary<string, AudioSetItem>();
        }

        if (audioSetItems.ContainsKey(path)) {
            return audioSetItems[path]; 
        }

        AudioClip clip = AssetUtil.LoadAsset<AudioClip>(path);

        AudioSetItem audioSetItem = new AudioSetItem();
        audioSetItem.audioClip = clip;
        audioSetItem.file = path;
        audioSetItem.type = "audio";

        audioSetItems.Set(path, audioSetItem);

        return audioSetItem;
    }

    /*
    public AudioClip LoadAudioFileAtPath( string file, Action<string> onFailure, Action<AudioClip> onSuccess )
    {
        AudioRecorderBinding.loadAudioFileAtPath(file, onFailure, onSuccess);
    }
    */

    public IEnumerator StartAmbienceCoroutine() {

        if (!ambienceActive) {

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
                if (currentIntro.gameObject != null) {
                    // currentIntro.gameObject.AudioTo(0f, 1f, 1f, 0f);
                }
            }
    
            if (currentLoop != null && ambienceActive) {

                currentLoop.volume = (float)musicSoundVolume;
                currentLoop.Play();
                //currentLoop.gameObject.AudioTo((float)musicSoundVolume, 1f, 1f, 0f);
            }           
            
            ambienceActive = true;
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

        if (currentGameLoops == null) {
            return;
        }

        currentLoopIndex = loop - 1;
        
        LogUtil.Log("StartGameLoop:", " loop:" + loop.ToString());
        LogUtil.Log("StartGameLoop:", " currentLoopIndex:" + currentLoopIndex.ToString());

        float volumeLevel = (float)musicSoundVolume;

        for (int i = 0; i < currentGameLoops.Count; i++) {

            float currentVolumeLevel = 0f;
            bool isCurrent = currentLoopIndex == i ? true : false;

            if (isCurrent) {
                currentVolumeLevel = volumeLevel;
            }

            if (currentGameLoops.Count > i) {

                if (currentGameLoops[i] == null) {
                    return;
                }

                //if (currentGameLoops[i].audio == null) {
                //    return;
                //}

                if (isCurrent) {

                    if (!currentGameLoops[i].isPlaying) {
                        currentGameLoops[i].Play();
                        currentGameLoops[i].time = 0f; // TODO for laps or syned get time of last loop
                        currentGameLoops[i].volume = 0f;
                    }
                    if (currentGameLoops[i].isPlaying) {
                        currentGameLoops[i].volume = currentVolumeLevel;//.gameObject.AudioTo(currentVolumeLevel, 1f, .1f, 0f);
                    }

                    LogUtil.Log("StartGameLoop:", " i:play:" + currentGameLoops[i].name);
                }
                else {

                    currentGameLoops[i].volume = currentVolumeLevel;//.gameObject.AudioTo(currentVolumeLevel, 0f, .1f, 0f);
                    currentGameLoops[i].StopIfPlaying();
                }
            }
        }
    }

    public void StopAmbience() {
        StartCoroutine(StopAmbienceRoutine());
    }

    private IEnumerator StopAmbienceRoutine() {
        if (ambienceActive) {

            if (currentLoop != null) {
                if (currentLoop.isPlaying) {
                    //currentLoop.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
                    if (currentIntro != null) {
                        currentIntro.volume = 0f;
                    }
                }
            }
    
            if (currentIntro != null) {
                if (currentIntro.isPlaying) {
                    currentIntro.volume = 0f;
                }
                //currentIntro.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
            }

            yield return new WaitForSeconds(0.5f);

            if (currentIntro != null) {
                currentIntro.Stop();
            }
    
            if (currentLoop != null) {
                currentLoop.Stop();
            }
            
            ambienceActive = false;
        }
        else {
            yield break;
        }
    }

    // READING / WRITING

    const int HEADER_SIZE = 44;
    
    public static bool Save(string path, AudioClip clip) {
        if (!path.ToLower().EndsWith(".wav")) {
            path += ".wav";
        }

        //var filepath = Path.Combine(Application.persistentDataPath, filename);        
        //LogUtil.Log(filepath);        
        // Make sure directory exists if user is saving to sub dir.
        //Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        
        using (var fileStream = CreateEmpty(path)) {            
            ConvertAndWrite(fileStream, clip);            
            WriteHeader(fileStream, clip);
        }
        
        return true; // TODO: return false if there's a failure saving the file
    }
    
    public static AudioClip TrimSilence(AudioClip clip, float min) {
        var samples = new float[clip.samples];
        
        clip.GetData(samples, 0);
        
        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }
    
    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) {
        return TrimSilence(samples, min, channels, hz, false, false);
    }
    
    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream) {
        int i;
        
        for (i=0; i<samples.Count; i++) {
            if (Mathf.Abs(samples[i]) > min) {
                break;
            }
        }
        
        samples.RemoveRange(0, i);
        
        for (i=samples.Count - 1; i>0; i--) {
            if (Mathf.Abs(samples[i]) > min) {
                break;
            }
        }
        
        samples.RemoveRange(i, samples.Count - i);
        
        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
        
        clip.SetData(samples.ToArray(), 0);
        
        return clip;
    }
    
    static FileStream CreateEmpty(string filepath) {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();
        
        for (int i = 0; i < HEADER_SIZE; i++) { //preparing the header
            fileStream.WriteByte(emptyByte);
        }
        
        return fileStream;
    }
    
    static void ConvertAndWrite(FileStream fileStream, AudioClip clip) {
        
        var samples = new float[clip.samples];
        
        clip.GetData(samples, 0);
        
        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]
        
        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.
        
        int rescaleFactor = 32767; //to convert float to Int16
        
        for (int i = 0; i<samples.Length; i++) {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        
        fileStream.Write(bytesData, 0, bytesData.Length);
    }
    
    static void WriteHeader(FileStream fileStream, AudioClip clip) {
        
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;
        
        fileStream.Seek(0, SeekOrigin.Begin);
        
        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        
        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        
        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        
        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        
        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        
        //UInt16 two = 2;
        UInt16 one = 1;
        
        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);
        
        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);
        
        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);
        
        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);
        
        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);
        
        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);
        
        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);
        
        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
        
        //      fileStream.Close();
    }
}