using System;
using System.Collections;
using Engine.Utility;
using UnityEngine;

namespace Engine.Audio {

    public class Sounds : MonoBehaviour {
        private AudioSource loop;
        private AudioSource intro;
        private AudioSource[] effects;

        public GameObject loopSoundsObject;
        public GameObject introSoundsObject;
        public GameObject effectsSoundsObject;

        private double effectSoundVolume = 1.0;
        private double musicSoundVolume = 0.5;

        private bool ambienceActive = false;

        private bool stopping;

        public bool isAudioLoopPlaying {
            get {
                return loop.isPlaying;
            }
        }

        public bool isAudioIntroPlaying {
            get {
                return loop.isPlaying;
            }
        }

        public bool isAudioPlaying {
            get {
                return isAudioLoopPlaying || isAudioIntroPlaying;
            }
        }

        public bool isAudioAmbienceActive {
            get {
                return ambienceActive;
            }
        }

        public void Awake() {
            ambienceActive = false;
            DontDestroyOnLoad(gameObject);
        }

        public void Start() {

            //
        }

        public void CheckAmbiencePlaying() {
            if (loop != null) {
                if (!loop.isPlaying)
                    StartAmbience();
            }

            if (!ambienceActive) {
                StartAmbience();
            }
        }

        public void StartAmbience() {
            ambienceActive = true;
            StartCoroutine(StartAmbienceCoroutine());
        }

        public void SetEffectsVolume(double volume) {
            effectSoundVolume = volume;
        }

        public double GetEffectsVolume() {
            return effectSoundVolume;
        }

        public void SetAmbienceVolume(double volume) {
            musicSoundVolume = volume;

            LogUtil.Log("Sounds::SetAmbienceVolume::" + volume);

            if (intro != null)
                intro.volume = (float)musicSoundVolume;

            if (loop != null)
                loop.volume = (float)musicSoundVolume;
        }

        public double GetAmbienceVolume() {
            return musicSoundVolume;
        }

        public void PlayEffect(int soundNumber) {
            if (effectsSoundsObject != null) {
                if (effects == null) {
                    effects = effectsSoundsObject.GetComponentsInChildren<AudioSource>();
                }

                int effectsLength = effects.Length;
                if (effectsLength > 0) {
                    if (soundNumber <= effectsLength) {
                        AudioSource effectSource = effects[soundNumber - 1];
                        effectSource.loop = false;
                        effectSource.volume = (float)effectSoundVolume;
                        effectSource.Play();
                    }
                }
            }
        }

        public IEnumerator StartAmbienceCoroutine() {

            // Find sound and randomize intros and loops
            // For now just grab them by name but we can randomize them if more than one.

            int loopClipCount = loopSoundsObject.GetComponentsInChildren<AudioSource>().Length;
            int introClipCount = introSoundsObject.GetComponentsInChildren<AudioSource>().Length;

            if (loopClipCount > 0 && introClipCount > 0) {

                // Randomize the selected loop and intro.. if only one it will pick one;

                int randomAmbience = UnityEngine.Random.Range(1, loopClipCount) - 1;

                if (randomAmbience < introSoundsObject.GetComponentsInChildren<AudioSource>().Length) {
                    intro = introSoundsObject.GetComponentsInChildren<AudioSource>()[randomAmbience];
                    intro.loop = false;
                    intro.volume = (float)musicSoundVolume;
                    intro.Play();
                }

                if (randomAmbience < loopSoundsObject.GetComponentsInChildren<AudioSource>().Length) {
                    loop = loopSoundsObject.GetComponentsInChildren<AudioSource>()[randomAmbience];
                    loop.loop = true;
                    loop.volume = (float)musicSoundVolume;
                    stopping = false;

                    yield return new WaitForSeconds(intro.clip.length);

                    //Did somebody call stop while we were waiting?
                    if (stopping)
                        yield break;

                    loop.Play();
                }
            }
        }

        public void StopAmbience() {
            ambienceActive = false;
            StartCoroutine(StopAmbienceRoutine());
        }

        private IEnumerator StopAmbienceRoutine() {
            stopping = true;
            bool wait = false;

            if (loop != null) {

                // Fade out sounds and reset
                if (loop.audio.isPlaying) {

                    //loop.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
                    wait = true;
                }
            }

            if (intro != null) {
                if (intro.audio.isPlaying) {

                    //intro.gameObject.AudioTo(0f, 1f, 1.5f, 0f);
                    wait = true;
                }
            }

            //If nothing was playing exit
            if (!wait)
                yield break;

            //Wait for fade out
            yield return new WaitForSeconds(1.5f);

            //Did somebody call start while we were waiting?
            if (!stopping)
                yield break;

            if (intro != null)
                intro.Stop();

            if (loop != null)
                loop.Stop();
        }
    }
}