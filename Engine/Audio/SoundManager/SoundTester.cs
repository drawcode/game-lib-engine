using System.Collections;
using UnityEngine;

public class SoundTester : GameObjectBehavior {
    public AudioClip explosionSound;
    public AudioClip rocketSound;
    public AudioClip bgSound;

    // Use this for initialization
    private void Start() {
        So.und.playSound(explosionSound);
        So.und.playBGMusic(bgSound, 1.0f, true);
    }

    // Update is called once per frame
    private void Update() {
        bool hasTouchBegan = false;
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch(i).phase == TouchPhase.Began) {
                LogUtil.Log("playing sound");
                hasTouchBegan = true;
                break;
            }
        }

        if (hasTouchBegan || Input.GetKeyUp(KeyCode.A)) {
            if (Random.Range(0, 100) % 2 == 0) {
                So.und.playSound(explosionSound);
            }
            else {
                So.und.playSound(rocketSound);
            }
        }
    }
}