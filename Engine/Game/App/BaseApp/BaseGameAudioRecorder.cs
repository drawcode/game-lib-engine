#define ANDROID_AMAZONN

using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public class BaseGameAudioRecordItems {

    // UI sounds

    public static string audio_effect_ui_button_1 = "audio_effect_ui_button_1";
    public static string audio_effect_ui_button_2 = "audio_effect_ui_button_2";
    public static string audio_effect_ui_button_3 = "audio_effect_ui_button_3";
    public static string audio_effect_ui_button_4 = "audio_effect_ui_button_4";

    // new

    // Game sounds
    public static string audio_effect_bike_1 = "audio_effect_bike_1";

    public static string audio_effect_bike_1_boost = "audio_effect_bike_1_boost";

    public static string audio_effect_bike_2 = "audio_effect_bike_2";
    public static string audio_effect_bike_2_boost = "audio_effect_bike_2_boost";
    public static string audio_effect_bike_rpm3000 = "audio_effect_bike_rpm3000";
    public static string audio_effect_bike_rpm4000 = "audio_effect_bike_rpm4000";
    public static string audio_effect_bike_rpm5000 = "audio_effect_bike_rpm5000";

    public static string audio_effect_bike_jump2 = "audio_effect_bike_jump2";

    public static string audio_effect_bike_revs_idle = "audio_effect_bike_revs_idle";
    public static string audio_effect_bike_high_gear = "audio_effect_bike_high_gear";
    public static string audio_effect_bike_low_gear = "audio_effect_bike_low_gear";
    public static string audio_effect_bike_medium_gear = "audio_effect_bike_medium_gear";

    public static string audio_effect_boo_funny = "audio_effect_boo_funny";
    public static string audio_effect_ohhh_1 = "audio_effect_ohhh_1";
    public static string audio_effect_crowd_cheer_1 = "audio_effect_crowd_cheer_1";
    public static string audio_effect_crowd_cheer_boost_1 = "audio_effect_crowd_cheer_boost_1";
}

public class BaseGameAudioRecorder {
    private static volatile BaseGameAudioRecorder instance;
    private static System.Object syncRoot = new System.Object();

    public string currentFileName = "";

    public bool currentlyRecording = false;
    public bool lastRecordingSucceeded = false;

    public Dictionary<string, AudioClip> loadedClips;

    public static BaseGameAudioRecorder Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAudioRecorder();
                }
            }
            return instance;
        }
    }

    public BaseGameAudioRecorder() {
        loadedClips = new Dictionary<string, AudioClip>();
    }

    private void OnEnable() {
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//AudioRecorderAndroidManager.startRecordingFailedEvent += startRecordingFailedEvent;
		//AudioRecorderAndroidManager.stopRecordingFinishedEvent += stopRecordingFinishedEvent;
		//AudioRecorderAndroidManager.stopRecordingFailedEvent += stopRecordingFailedEvent;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
#elif UNITY_EDITOR
#endif
    }

    private void OnDisable() {
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//AudioRecorderAndroidManager.startRecordingFailedEvent -= startRecordingFailedEvent;
		//AudioRecorderAndroidManager.stopRecordingFinishedEvent -= stopRecordingFinishedEvent;
		//AudioRecorderAndroidManager.stopRecordingFailedEvent -= stopRecordingFailedEvent;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
#elif UNITY_EDITOR
#endif
    }

#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
	void startRecordingFailedEvent(string param) {
		LogUtil.Log("GameAudioRecorder:: startRecordingFailedEvent:" + param);
		currentlyRecording = false;
		lastRecordingSucceeded = false;
	}

	void stopRecordingFinishedEvent(string file) {
		LogUtil.Log("GameAudioRecorder:: stopRecordingFinishedEvent:" + file);
		currentlyRecording = false;
		lastRecordingSucceeded = true;
	}

	void stopRecordingFailedEvent(string error) {
		LogUtil.Log("GameAudioRecorder:: stopRecordingFailedEvent:" + error);
		currentlyRecording = false;
		lastRecordingSucceeded = false;
	}
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
#elif UNITY_EDITOR
#endif

    public Dictionary<string, AudioClip> GetLoadedClips() {
        return loadedClips;
    }

    public void ClearLoadedClips() {
        if (loadedClips != null) {
            foreach (KeyValuePair<string, AudioClip> pair in loadedClips) {
                MonoBehaviour.Destroy(pair.Value);
            }

            loadedClips.Clear();
        }
    }

    public void PrepareAudioFilename(string filename) {
        string error = "No recorder";
        currentFileName = filename;
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//error = AudioRecorderAndroid.prepareToRecordFile( filename );
		currentFileName = GetPersistentPath(filename);

		//error = "";
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//error = AudioRecorderBinding.prepareToRecordFile( filename );
#elif UNITY_EDITOR
#endif
        if (error.Length > 0) {
            LogUtil.Log("failed to prepare audio recorder: " + error);
        }
        else {
            LogUtil.Log("File Prepared:" + filename);
        }
    }

    public string GetFileName(string file) {
        if (file.IndexOf(".wav") == -1) {
            file = file + ".wav";
        }
        return file;
    }

    public string GetPersistentPath(string file) {
        string filepath = file;
        if (filepath.IndexOf(Application.persistentDataPath) == -1) {
            filepath = Application.persistentDataPath + "/" + filepath;
        }
        if (filepath.IndexOf("/mnt/sdcard/mnt/sdcard") > -1) {
            filepath = filepath.Replace("/mnt/sdcard/mnt/sdcard", "/mnt/sdcard");
        }
        return filepath;
    }

    public string FilterPersistentPathAndroid(string file) {
        string filepath = file;
        if (filepath.IndexOf("/mnt/sdcard") > -1) {
            filepath = filepath.Replace("/mnt/sdcard", "");
        }
        return filepath;
    }

    public bool CheckIfSoundExistsByKey(string key) {
        return CheckIfSoundExists(Application.persistentDataPath + "/" + GetFileName(key));
    }

    public bool CheckIfSoundExists(string file) {
        bool exists = false;

#if UNITY_STANDALONE_OSX
		exists = System.IO.File.Exists(file);
#elif UNITY_STANDALONE_WIN
		exists = System.IO.File.Exists(file);
#elif UNITY_ANDROID
		exists = System.IO.File.Exists(file);
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
		exists = System.IO.File.Exists(file);
#elif UNITY_EDITOR
		exists = System.IO.File.Exists(file);
#endif
        LogUtil.Log("SoundExists: " + file + " - " + exists);
        return exists;
    }

    public void Play() {
        Play(currentFileName, (float)BaseGameProfiles.Current.GetAudioEffectsVolume());
    }

    public void Play(string filename) {
        Play(filename, (float)BaseGameProfiles.Current.GetAudioEffectsVolume());
    }

    public void Play(string filename, float volume) {
        var onSuccess = new Action<AudioClip>(clip => {
            AudioSystem.Instance.PlayEffect(clip, volume);

            //AudioSystem.Instance.PlayEffectIncrement(clip, volume);
        });

        //,(float)GameProfiles.Current.GetAudioEffectsVolume()*.7f)
        Load(filename, onSuccess);
    }

    /*

    // loads up the audio file and returns an AudioClip ready for use in the callback
    public IEnumerator loadAudioFileAtPathVehicles( string file, VehicleSounds vehicleSounds, Action<string> onFailure, Action<AudioClip, VehicleSounds> onSuccess )
    {
        // TODO swap out to better pool
        AudioClip audioClip = null;

        if(GameAudioRecorder.Instance.loadedClips.ContainsKey(file)) {
            audioClip = GameAudioRecorder.Instance.loadedClips[file];
            if(audioClip == null) {
                GameAudioRecorder.Instance.loadedClips.Remove(file);
            }
        }

        if(audioClip == null) {
            var www = new WWW( file );

            LogUtil.Log( "loadAudioFileAtPathVehicles: file: " + file );

            yield return www;

            if( www.error != null ) {
                if( onFailure != null ) {
                    onFailure( www.error );

                    LogUtil.Log( "loadAudioFileAtPathVehicles: www.error: " + www.error );
                }
            }

            if( www.audioClip ) {
                audioClip = www.GetAudioClip(true);

                GameAudioRecorder.Instance.loadedClips.Add(file, audioClip);

                LogUtil.Log( "loadAudioFileAtPathVehicles: www.audioClip: " + audioClip );
                LogUtil.Log( "loadAudioFileAtPathVehicles: www.audioClip.name: " + audioClip.name );
            }

            www.Dispose();
            GC.Collect();

            //Resources.UnloadUnusedAssets();
        }

        if(audioClip != null) {
            if( onSuccess != null ) {
                onSuccess(audioClip, vehicleSounds);
            }
        }
    }
    */

    // loads up the audio file and returns an AudioClip ready for use in the callback
    public IEnumerator loadAudioFileAtPath(string file, Action<string> onFailure, Action<AudioClip> onSuccess) {
        AudioClip audioClip = null;

        // TODO swap out to better pool
        if (BaseGameAudioRecorder.Instance.loadedClips.ContainsKey(file)) {
            audioClip = BaseGameAudioRecorder.Instance.loadedClips[file];
            if (audioClip == null) {
                BaseGameAudioRecorder.Instance.loadedClips.Remove(file);
            }
        }

        if (audioClip == null) {
            var www = new WWW(file);

            yield return www;

            if (www.error != null) {
                if (onFailure != null) {
                    onFailure(www.error);
                }
            }

            if (www.audioClip) {
                audioClip = www.GetAudioClip(false);

                BaseGameAudioRecorder.Instance.loadedClips.Add(file, audioClip);
            }

            www.Dispose();
            GC.Collect();

            //Resources.UnloadUnusedAssets();
        }

        if (audioClip != null) {
            if (onSuccess != null) {
                onSuccess(audioClip);
            }
        }
    }

    /*
    public string LoadVehicleSounds(string filename, VehicleSounds vehicleSounds, Action<AudioClip, VehicleSounds> onSuccess) {
        filename = GetFileName(filename);

        var filePath = GetPersistentPath(filename);
        var file = "file://" + filePath;

        currentFileName = filename;

        LogUtil.Log( "Load: onSuccess: " + onSuccess );
        LogUtil.Log( "Load: filePath: " + filename );

        var onFailure = new Action<string>( error => LogUtil.Log( error ) );

#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
        if(CheckIfSoundExists(filePath)) {
            CoroutineUtil.Start( loadAudioFileAtPathVehicles( file, vehicleSounds, onFailure, onSuccess ) );
        }
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
        if(CheckIfSoundExists(filePath)) {
            CoroutineUtil.Start( loadAudioFileAtPathVehicles( file, vehicleSounds, onFailure, onSuccess ) );
        }
#elif UNITY_EDITOR
#endif
        LogUtil.Log( "Load: " + file );
        LogUtil.Log( "Load checked: " + filePath );
        return file;
    }
    */

    public string Load(string filename, Action<AudioClip> onSuccess) {
        filename = GetFileName(filename);

        var filePath = GetPersistentPath(filename);

        var file = "file://" + filePath;

        currentFileName = filename;

        LogUtil.Log("Load: onSuccess: " + onSuccess);
        LogUtil.Log("Load: filePath: " + filename);
#if !UNITY_WEBPLAYER
        var onFailure = new Action<string>(error => LogUtil.Log(error));
#endif
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
		if(CheckIfSoundExists(filePath)) {
			CoroutineUtil.Start( loadAudioFileAtPath( file, onFailure, onSuccess ) );
		}
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE
		if(CheckIfSoundExists(filePath)) {
			CoroutineUtil.Start( loadAudioFileAtPath( file, onFailure, onSuccess ) );
		}
#elif UNITY_EDITOR
#endif
        LogUtil.Log("Load: " + file);
        LogUtil.Log("Load checked: " + filePath);
        return file;
    }

    public bool Record() {
        currentlyRecording = true;
        bool didRecord = false;
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
		string pathAndroid = FilterPersistentPathAndroid(GetPersistentPath(currentFileName));
		LogUtil.Log( "currentFileName pathAndroid: " + pathAndroid);

		//AudioRecorderAndroid.startRecording(pathAndroid);
		didRecord = true;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//didRecord = AudioRecorderBinding.record();
		currentlyRecording = false;
		lastRecordingSucceeded = didRecord;
#elif UNITY_EDITOR
#endif
        LogUtil.Log("Record: " + didRecord);
        return didRecord;
    }

    public bool Record(float duration) {
        currentlyRecording = true;
        bool didRecord = false;
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
		string pathAndroid = FilterPersistentPathAndroid(GetPersistentPath(currentFileName));
		LogUtil.Log( "currentFileName pathAndroid: " + pathAndroid);

		//AudioRecorderAndroid.startRecording(pathAndroid);
		didRecord = true;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//didRecord = AudioRecorderBinding.recordForDuration( duration );
		currentlyRecording = false;
		lastRecordingSucceeded = didRecord;
#elif UNITY_EDITOR
#endif
        LogUtil.Log("Record: " + didRecord);
        return didRecord;
    }

    public void Pause() {
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//AudioRecorderAndroid.pause();
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//AudioRecorderBinding.pause();
#elif UNITY_EDITOR
#endif
    }

    public void Stop(bool finish) {
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//AudioRecorderAndroid.stopRecording();
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//AudioRecorderBinding.stop( finish );
#elif UNITY_EDITOR
#endif
        LogUtil.Log("Stop finish: " + finish);
    }

    public bool IsRecording() {
        bool isRecording = false;
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//isRecording = currentlyRecording;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//isRecording = AudioRecorderBinding.isRecording();
#elif UNITY_EDITOR
#endif
        LogUtil.Log("AudioRecorder IsRecording: " + isRecording);
        return isRecording;
    }

    public float GetRecordedTime() {
        float duration = 0f;
#if UNITY_STANDALONE_OSX
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID

		//duration = 5.5f;
#elif UNITY_WEBPLAYER
#elif UNITY_IPHONE

		//duration = AudioRecorderBinding.getCurrentTime();
#elif UNITY_EDITOR
#endif
        LogUtil.Log("AudioRecorder GetRecordedTime duration: " + duration);
        return duration;
    }

    // util

    public static void PlayEffect(string audioEffectName) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName);
    }

    public static void PlayEffect(string audioEffectName, float volume) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.PlayEffect(audioEffectName, volume);
    }

    public static void PlayUIMainLoop(string soundName, float volume) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.PlayUIMainLoop(soundName, volume);
    }

    public static void PlayGameMainLoop(string soundName, float volume) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.PlayGameMainLoop(soundName, volume);
    }

    public static void SetAmbienceVolume(double volume) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.SetAmbienceVolume(volume);
    }

    public static void SetEffectsVolume(double volume) {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.SetEffectsVolume(volume);
    }

    public static double GetAmbienceVolume() {
        if (AudioSystem.Instance != null)
            return AudioSystem.Instance.GetAmbienceVolume();
        else
            return 0.0;
    }

    public static double GetEffectsVolume() {
        if (AudioSystem.Instance != null)
            return AudioSystem.Instance.GetEffectsVolume();
        else
            return 0.0;
    }

    public static void StartAmbience() {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.StartAmbience();
    }

    public static void StopAmbience() {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.StopAmbience();
    }

    public static void SetVolumeForRace(bool inRace) {
        LogUtil.Log("AudioListener SetVolumeForRace:" + inRace);

        //if(GameGlobal.Instance != null) {
        if (inRace) {
            AudioListener.volume = (float)(BaseGameProfiles.Current.GetAudioEffectsVolume() * .9);
            LogUtil.Log("AudioListener setting for race:" + AudioListener.volume);
        }
        else {
            AudioListener.volume = (float)BaseGameProfiles.Current.GetAudioEffectsVolume();
            LogUtil.Log("AudioListener setting for UI:" + AudioListener.volume);
        }

        //}
    }

    public static void StartGameLapLoops() {
        if (AudioSystem.Instance != null)
            AudioSystem.Instance.StartGameLoopsForLaps();
    }

    public static void StartGameLoopForLap(int lap) {
        if (AudioSystem.Instance != null) {
            AudioSystem.Instance.StartGameLoopForLap(lap);
            if (lap > 1) {

                //GamePlayerProgress.Instance.SetAchievement(GameAchievements.ACHIEVE_MIX_IT_UP, true);
            }
        }
    }
}