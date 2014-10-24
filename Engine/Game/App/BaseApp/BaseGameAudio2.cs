/*using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public class BaseGameAudioEffects2 {

    // UI sounds
    public static string audio_loop_intro_1 = "audio_loop_intro_1";

    public static string audio_loop_main_1 = "audio_loop_main_1";

    public static string audio_effect_splash = "audio_effect_splash";
    public static string audio_effect_start = "audio_effect_start";
    public static string audio_effect_main = "audio_effect_main";

    public static string audio_effect_ui_button_1 = "audio_effect_ui_button_1";
    public static string audio_effect_ui_button_2 = "audio_effect_ui_button_2";
    public static string audio_effect_ui_button_3 = "audio_effect_ui_button_3";
    public static string audio_effect_ui_button_4 = "audio_effect_ui_button_4";

    // new
    public static string audio_loop_ui_music = "audio_loop_ui_music";

    public static string audio_loop_logo_music = "audio_loop_logo_music";
    public static string audio_loop_lap1_final = "audio_loop_lap1_final";
    public static string audio_loop_lap2_final = "audio_loop_lap2_final";
    public static string audio_loop_lap3_final = "audio_loop_lap3_final";

    // Game sounds
    public static string audio_effect_bike_1 = "audio_effect_bike_1";

    public static string audio_effect_bike_1_boost = "audio_effect_bike_1_boost";
    public static string audio_effect_bike_2 = "audio_effect_bike_2";
    public static string audio_effect_bike_2_boost = "audio_effect_bike_2_boost";
    public static string audio_effect_bike_rpm3000 = "audio_effect_bike_rpm3000";
    public static string audio_effect_bike_rpm4000 = "audio_effect_bike_rpm4000";
    public static string audio_effect_bike_rpm5000 = "audio_effect_bike_rpm5000";
    public static string audio_effect_bike_jump2 = "audio_effect_bike_jump2";
    public static string audio_effect_bike_jump_250_1 = "audio_effect_bike_jump_250_1";
    public static string audio_effect_bike_jump_250_2 = "audio_effect_bike_jump_250_2";
    public static string audio_effect_bike_rev_2 = "audio_effect_bike_rev_2";
    public static string audio_effect_bike_rev_constant = "audio_effect_bike_rev_constant";
    public static string audio_effect_bike_revs_idle = "audio_effect_bike_revs_idle";
    public static string audio_effect_bike_high_gear = "audio_effect_bike_high_gear";
    public static string audio_effect_bike_low_gear = "audio_effect_bike_low_gear";
    public static string audio_effect_bike_medium_gear = "audio_effect_bike_medium_gear";
    public static string audio_effect_bike_medium_gear2 = "audio_effect_bike_medium_gear2";

    public static string audio_effect_boo_funny = "audio_effect_boo_funny";
    public static string audio_effect_boo_medium = "audio_effect_boo_medium";
    public static string audio_effect_ohhh_1 = "audio_effect_ohhh_1";
    public static string audio_effect_woohoo = "audio_effect_woohoo";
    public static string audio_effect_crowd_cheer_constant_1 = "audio_effect_crowd_cheer_constant_1";
    public static string audio_effect_crowd_cheer_1 = "audio_effect_crowd_cheer_1";
    public static string audio_effect_crowd_cheer_boost_1 = "audio_effect_crowd_cheer_boost_1";
}

public class BaseGameAudio2 {
    private static volatile BaseGameAudio2 instance;
    private static System.Object syncRoot = new System.Object();

    public static BaseGameAudio2 Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAudio2();
                }
            }
            return instance;
        }
    }

    public BaseGameAudio2() {
    }

    public static string GetFileName(string key) {
        return key + ".wav"; // all currently saved as wav for high quality and on SD/persistence so room to.
    }

    public static AudioClip GetShuffledSound(List<AudioClip> clips) {
        if (clips != null) {
            if (clips.Count > 0) {
                clips.Shuffle();
                return clips[0];
            }
        }
        return null;
    }

    //var onSuccess = new Action<AudioClip>( clip =>
    //{
    //  AudioSystem.Instance.PlayAudioClipOneShot(clip);
    //});

    public bool CheckIfEffectHasCustom(string audioEffectName) {
        bool hasCustomAudio = false;
        bool hasCustomAudioItem = false;

        //string audioEffectName, float volume

        if (BaseGameProfiles.Current.CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CUSTOM_AUDIO)) {
            hasCustomAudio = true;
        }

        CustomPlayerAudio customPlayerAudio;
        CustomPlayerAudioItem customPlayerAudioItem;

        if (hasCustomAudio) {
            customPlayerAudio = BaseGameProfiles.Current.GetCustomAudio();

            customPlayerAudioItem = customPlayerAudio.GetAudioItem(audioEffectName);

            if (customPlayerAudioItem != null) {
                hasCustomAudioItem = customPlayerAudioItem.useCustom;
            }
        }

        return hasCustomAudioItem;
    }

    public static float GetCurrentVolume() {
        return (float)BaseGameProfiles.Current.GetAudioEffectsVolume();
    }

    public static float GetCurrentVolumeAdjust() {
        return GetCurrentVolumeAdjust(1.3f);
    }

    public static float GetCurrentVolumeAdjust(float adjust) {
        return GetCurrentVolume() * adjust;
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, bool hasCustomAudioItem) {
        float volume = GetCurrentVolumeAdjust();
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName) {
        bool hasCustomAudioItem = BaseGameAudio.CheckIfEffectHasCustom(audioEffectName);
        float volume = GetCurrentVolumeAdjust();
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, float volume) {
        bool hasCustomAudioItem = BaseGameAudio.CheckIfEffectHasCustom(audioEffectName);
        PlayCustomOrDefaultEffect(audioEffectName, volume, hasCustomAudioItem);
    }

    public static void PlayCustomOrDefaultEffect(string audioEffectName, float volume, bool hasCustomAudioItem) {
        if (hasCustomAudioItem) {
            if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeBoosting.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeBoosting),
                    GetCurrentVolumeAdjust(2f));

                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeBoosting,
                //  (float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRacing.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeRacing),
                    GetCurrentVolumeAdjust(2f));

                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeRacing,
                //  (float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRevving.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioBikeRevving),
                    GetCurrentVolumeAdjust(2f));

                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioBikeRevving,
                //  (float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdBoo.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdBoo),
                    GetCurrentVolumeAdjust(2f));

                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdBoo,
                //  (float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
            else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdCheer.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdCheer),
                    GetCurrentVolumeAdjust(2f));
                BaseGameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdCheer,
                    GetCurrentVolumeAdjust(.25f));
            }
            else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdJump.ToLower()) {
                BaseGameAudioRecorder.Instance.Play(GetFileName(CustomPlayerAudioKeys.audioCrowdJump),
                    GetCurrentVolumeAdjust(2f));

                //GameAudio.PlayDefaultEffect(CustomPlayerAudioKeys.audioCrowdJump,
                //  (float)GameProfiles.Current.GetAudioEffectsVolume()*.3f);
            }
        }
        else {
            PlayDefaultEffect(audioEffectName, volume);
        }
    }

    public static void PlayDefaultEffect(string audioEffectName) {
        BaseGameAudio.PlayCustomOrDefaultEffect(audioEffectName, (float)BaseGameProfiles.Current.GetAudioEffectsVolume());
    }

    public static void PlayDefaultEffect(string audioEffectName, float volume) {
        if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeBoosting.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_bike_jump2, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .4f);
        }
        else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRacing.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_bike_medium_gear);
        }
        else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioBikeRevving.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_bike_revs_idle);
        }
        else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdBoo.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_ohhh_1, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .42f);
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_boo_funny, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .5f);

            //GameAudio.PlayEffect(GameAudioEffects.audio_effect_boo_medium, (float)GameProfiles.Current.GetAudioEffectsVolume()*1.2f);
        }
        else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdCheer.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_crowd_cheer_boost_1, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .7f);
        }
        else if (audioEffectName.ToLower() == CustomPlayerAudioKeys.audioCrowdJump.ToLower()) {
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_crowd_cheer_1, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .5f);
            BaseGameAudio.PlayEffect(BaseGameAudioEffects.audio_effect_woohoo, (float)BaseGameProfiles.Current.GetAudioEffectsVolume() * .5f);
        }
        else {
            LogUtil.Log("No sound found with that key:" + audioEffectName);
        }
    }

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

    public static void StartGameLoop(int lap) {
        if (AudioSystem.Instance != null) {
            AudioSystem.Instance.StartGameLoop(lap);
            if (lap > 1) {

                //GamePlayerProgress.Instance.SetAchievement(BaseGameAchievements.ACHIEVE_MIX_IT_UP, true);
            }
        }
    }

    public static void PlayAudioClip(AudioClip clip, bool loop, int increment, float volume) {
        if (AudioSystem.Instance) {
            AudioSystem.Instance.PlayAudioClip(clip, loop, increment, volume);
        }
    }

    public static void PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume) {
        if (AudioSystem.Instance) {
            AudioSystem.Instance.PlayAudioClip(pos, parent, clip, loop, increment, volume);
        }
    }

    public static void PlayAudioClip(Vector3 pos, Transform parent, AudioClip clip, bool loop, int increment, float volume, float panLevel) {
        if (AudioSystem.Instance) {
            AudioSystem.Instance.PlayAudioClip(pos, parent, clip, loop, increment, volume, panLevel);
        }
    }

    public static AudioClip LoadLoop(string name) {
        if (AudioSystem.Instance) {
            return AudioSystem.Instance.LoadLoop(name);
        }
        return null;
    }
}
*/