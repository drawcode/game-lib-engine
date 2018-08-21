using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(ParticleSystem))]
public class ParticleDestroy : MonoBehaviour {

    public bool OnlyDeactivate;

    void OnEnable() {

        StartCoroutine(CheckIfAlive());
    }

    IEnumerator CheckIfAlive() {

        while(true) {

            yield return new WaitForSeconds(0.5f);

            if(!gameObject.Get<ParticleSystem>().IsAlive(true)) {

                if(OnlyDeactivate) {

                    gameObject.Hide();
                }
                else {

                    GameObjectHelper.DestroyGameObject(this.gameObject);
                }

                break;
            }
        }
    }
}