using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameObjectHelper {
    
    public static float defaultScale = 1f;
    public static float pinchScaleFactor = .05f;
    public static float scaleMin = .25f;
    public static float scaleMax = 3.5f;
    public static bool deferTap = false;
    
    // GAME OBJECT
    
    public static bool ContainsChild(GameObject inst, string name) {
        if (inst == null) {
            return false;
        }
        
        Transform child = inst.transform.FindChild(name);
        
        if (child != null) {
            return true;
        }
        else {
            foreach (Transform t in inst.transform) {
                return ContainsChild(t.gameObject, name);
            }
        }
        
        return false;
    }
    
    // LAYER
    
    public static void SetLayerRecursively(GameObject inst, int layer) {
        if (inst == null)
            return;
        
        inst.layer = layer;
        
        foreach (Transform child in inst.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
    
    public static void SetLayerRecursively(GameObject inst, string name) {
        if (inst == null)
            return;
        
        inst.layer = LayerMask.NameToLayer(name);
        
        foreach (Transform child in inst.transform)
            SetLayerRecursively(child.gameObject, LayerMask.NameToLayer(name));
    }
    
    // AUDIO
    
    public static void StopSounds(GameObject inst) {
        if (inst == null)
            return;

        AudioSource audioSource = inst.GetComponent<AudioSource>();
        
        if (audioSource != null) {
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
        }
        
        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Stop();
        }
    }
    
    public static void PauseSounds(GameObject inst) {
        if (inst == null)
            return;
        
        AudioSource audioSource = inst.GetComponent<AudioSource>();
        
        if (audioSource != null) {
            if (audioSource.isPlaying) {
                audioSource.Pause();
            }
        }
        
        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Pause();
        }
    }
    
    public static void PlaySounds(GameObject inst) {
        if (inst == null)
            return;
        
        AudioSource audioSource = inst.GetComponent<AudioSource>();

        if (audioSource != null) {
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        }
        
        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Play();
        }
    }
    
    public static bool IsAudioSourcePlaying(GameObject inst) {
        if (inst == null)
            return false;
        
        AudioSource audioSource = inst.GetComponent<AudioSource>();

        if (audioSource != null) {
            if (audioSource.isPlaying) {
                return true;
            }
        }
        
        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            if (source.isPlaying) {
                return true;
            }
        }
        
        return false;
    }
    
    // RENDERERS
    
    public static bool IsRenderersVisibleByCamera(GameObject inst) {
        if (inst == null)
            return false;
        
        if (!inst.IsRenderersVisible()) {
            return false;
        }

        Renderer render = inst.GetComponent<Renderer>();
        
        if (render != null) {
            if (render.enabled) {
                if (render.isVisible) {
                    return true;
                }
            }
        }
        
        // Enable rendering:
        foreach (Renderer component in inst.GetComponentsInChildren<Renderer>()) {
            if (component.enabled) {
                if (component.isVisible) {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public static bool IsRenderersVisibleByCamera(GameObject inst, Camera cam) {
        if (inst == null)
            return false;
        
        if (!inst.IsRenderersVisible()) {
            return false;
        }
        
        Renderer render = inst.GetComponent<Renderer>();
        
        if (render != null) {
            if (render.enabled) {
                if (render.isVisible
                    && render.IsVisibleFrom(cam)) {
                    return true;
                }
            }
        }
        
        // Enable rendering:
        foreach (Renderer component in inst.GetComponentsInChildren<Renderer>()) {
            if (component.enabled) {
                if (component.isVisible
                    && component.IsVisibleFrom(cam)) {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public static bool IsRenderersVisible(GameObject inst) {
        if (inst == null)
            return false;
        
        Renderer render = inst.GetComponent<Renderer>();

        if (render != null) {
            if (render.enabled) {
                return true;
            }
        }
        
        // Enable rendering:
        foreach (Renderer component in inst.GetComponentsInChildren<Renderer>()) {
            if (component.enabled) {
                return true;
            }
        }
        
        return false;
    }
    
    public static void ShowRenderers(GameObject inst) {
        if (inst == null)
            return;
        
        Renderer render = inst.GetComponent<Renderer>();

        if (render != null) {
            render.enabled = true;
        }
        
        // Enable rendering:
        foreach (Renderer component in inst.GetComponentsInChildren<Renderer>()) {
            component.enabled = true;
        }
    }
    
    public static void HideRenderers(GameObject inst) {
        if (inst == null)
            return;
        
        Renderer render = inst.GetComponent<Renderer>();
        
        if (render != null) {
            render.enabled = false;
        }
        
        // Enable rendering:
        foreach (Renderer component in inst.GetComponentsInChildren<Renderer>()) {
            component.enabled = false;
        }
    }

    // HIT OBJECT

    public static bool HitObject(GameObject inst, Vector3 pos) {
        
        if (inst == null)
            return false;

        Ray screenRay = Camera.main.ScreenPointToRay(pos);

        RaycastHit hit;
        
        if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
            if (hit.transform.gameObject == inst) {
                return true;
            }
        }
        return false;
    }

    public static GameObject HitObject(Vector3 pos, string containsNameTo) {
                
        return HitObject(Camera.main, pos, containsNameTo);
    }

    public static GameObject HitObject(Camera cam, Vector3 pos, string containsNameTo) {
        
        Ray screenRay = cam.ScreenPointToRay(pos);
        
        RaycastHit hit;
        
        if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
            GameObject go = hit.transform.gameObject;
            
            //Debug.Log("HitObject:" + " name:" + go.name + " containsNameTo:" + containsNameTo + " pos:" + pos);
            
            if (go.name.ToLower().Contains(containsNameTo.ToLower())) {
                return go;
            }
        }
        return null;
    }
        
    // DEBUG
    
    public static void DumpRootTransforms() {
        UnityEngine.Object[] objs = UnityEngine.GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (UnityEngine.Object obj in objs) {
            GameObject go = obj as GameObject;
            if (go.transform.parent == null) {
                DumpGoToLog(go);
            }
        }
    }
    
    public static void DumpGoToLog(GameObject go) {
        LogUtil.Log("DUMP: go:" + go.name + "::::" + GameObjectHelper.DumpGo(go));
    }
    
    public static string DumpGo(GameObject go) {
        StringBuilder sb = new StringBuilder();
        sb.Append(go.name);
        DumpGameObject(go, sb, "", false);
        return sb.ToString();
    }
    
    private static void DumpGameObject(GameObject go, StringBuilder sb, string indent, bool includeAllComponents) {
        bool rendererEnabled = false;

        if (go == null) {
            return;
        }

        Renderer render = go.GetComponent<Renderer>();

        if (render != null) {
            rendererEnabled = render.enabled;
        }
        
        int markerId = -1;
        
        /*
         if(gameObject.GetComponent<MarkerBehaviour>() != null) {
            markerId = gameObject.GetComponent<MarkerBehaviour>().MarkerID;
        }
        */
        bool hasLoadedObj = false;
        /*
        if(gameObject.GetComponent<ARLoadedActionObject>() != null) {
            hasLoadedObj = true;
        }
        */
        
        sb.Append(string.Format("\r\n{0}+{1} - a:{2} - r:{3} - mid:{4} - loadedObj: {5} - scale: x:{6} y:{7} z:{8} - pos: x:{9} y:{10} z:{11}",
                                indent, 
                                go.name,
                                go.activeSelf, 
                                rendererEnabled,
                                markerId, 
                                hasLoadedObj,
                                go.transform.localScale.x,
                                go.transform.localScale.y,
                                go.transform.localScale.z,
                                go.transform.position.x,
                                go.transform.position.y,
                                go.transform.position.z));
        
        if (includeAllComponents) {
            foreach (Component component in go.GetComponents<Component>()) {
                DumpComponent(component, sb, indent + "  ");
            }
        }
        
        foreach (Transform child in go.transform) {
            DumpGameObject(child.gameObject, sb, indent + "  ", includeAllComponents);
        }
    }
    
    private static void DumpComponent(Component component, StringBuilder sb, string indent) {
        sb.Append(string.Format("{0}{1}", indent, (component == null ? "(null)" : component.GetType().Name)));
    }
    
    // MOVEMENT
    
    public static void ScaleTweenObjectAbsolute(GameObject go, float absoluteValue) {
        if (go != null) {
            //float valueTo = (absoluteValue / defaultScale) + .005f;
            // TODO tween scale
            //iTween.ScaleTo(go, Vector3.zero.WithX(valueTo).WithY(valueTo).WithZ(valueTo), .2f);

        }
    }
    
    public static void RotateTweenObjectAbsolute(GameObject go, float absoluteValue) {
        if (go != null) {
            //float valueTo = absoluteValue * 360;
            // TODO tween rotate
            //UITweenerUtil.RotateTo(go, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, 0f, .2f);
        }
    }
    
    public static void ScaleObject(GameObject go, float delta) {
        if (go != null) {
            // change the scale of the target based on the pinch delta value
            float scaleTo = delta / defaultScale * pinchScaleFactor;
            Vector3 currentObjectScale = go.transform.localScale;
            currentObjectScale += scaleTo * Vector3.one;
            
            currentObjectScale.x = Mathf.Clamp(currentObjectScale.x, scaleMin, scaleMax);
            currentObjectScale.y = Mathf.Clamp(currentObjectScale.y, scaleMin, scaleMax);
            currentObjectScale.z = Mathf.Clamp(currentObjectScale.z, scaleMin, scaleMax);
            
            go.transform.localScale = currentObjectScale;
        }
    }
    
    public static void ResetObject(GameObject go) {
        ResetScale(go, 1);
        ResetRotation(go);
        ResetPosition(go);
    }
    
    public static void ResetObject(GameObject go, bool includeChildren) {
        if (go != null) {
            ResetObject(go);
            
            if (includeChildren) {
                foreach (Transform t in go.transform) {
                    ResetObject(t.gameObject);
                }
            }
        }
    }
    
    public static void ResetScale(GameObject go, float valueTo) {
        if (go != null) {
            Vector3 currentObjectScale = go.transform.localScale;
            currentObjectScale.x = valueTo;
            currentObjectScale.y = valueTo;
            currentObjectScale.z = valueTo;
            go.transform.localScale = currentObjectScale;
        }
    }
    
    public static void ResetScale(GameObject go, float valueTo, bool includeChildren) {
        if (go != null) {
            ResetScale(go, valueTo);
            
            if (includeChildren) {
                foreach (Transform t in go.transform) {
                    ResetScale(t.gameObject, valueTo);
                }
            }
        }
    }
    
    public static void ResetRotation(GameObject go) {
        if (go != null) {
            Quaternion objectRotation = Quaternion.identity;
            go.transform.rotation = objectRotation;
            go.transform.localRotation = objectRotation;
        }
    }
    
    public static void ResetLocalRotation(GameObject go) {
        if (go != null) {
            Quaternion objectRotation = Quaternion.identity;
            go.transform.localRotation = objectRotation;
        }
    }
    
    public static void ResetRotation(GameObject go, bool includeChildren) {
        if (go != null) {
            ResetRotation(go);
            
            if (includeChildren) {
                foreach (Transform t in go.transform) {
                    ResetRotation(t.gameObject);
                }
            }
        }
    }
    
    public static void ResetPosition(GameObject go) {
        if (go != null) {
            Vector3 pos = Vector3.zero;
            go.transform.position = pos;
            go.transform.localPosition = pos;
        }
    }

    public static void ResetLocalPosition(GameObject go) {
        if (go != null) {
            Vector3 pos = Vector3.zero;
            go.transform.localPosition = pos;
        }
    }
    
    public static void ResetPosition(GameObject go, bool includeChildren) {
        if (go != null) {
            ResetPosition(go);
            
            if (includeChildren) {
                foreach (Transform t in go.transform) {
                    ResetPosition(t.gameObject);
                }
            }
        }
    }

    public static void TrackObject(GameObject go, GameObject target) {
        if (go != null && target != null) {
            go.transform.localPosition = Vector3.zero;
            go.transform.position = target.transform.position;
            
            go.transform.localRotation = Quaternion.identity;
            go.transform.rotation = target.transform.rotation;
        }
    }
    
    public static void RotateObjectX(GameObject go, float val) {
        RotateObject(go, Vector3.zero.WithX(val));
    }
    
    public static void RotateObjectY(GameObject go, float val) {
        RotateObject(go, Vector3.zero.WithY(val));
    }
    
    public static void RotateObjectZ(GameObject go, float val) {
        RotateObject(go, Vector3.zero.WithZ(val));
    }
    
    public static void RotateObject(GameObject go, Vector3 rotateBy) {
        if (go != null) {
            // apply a rotation around the Z axis by rotationAngleDelta degrees on our target object
            go.transform.Rotate(rotateBy.x, rotateBy.y, rotateBy.z);
        }
    }
    
    public static void SpinObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
        
        if (go != null) {
            Rigidbody rigid = go.GetComponent<Rigidbody>();

            if (rigid == null) {
                rigid = go.AddComponent<Rigidbody>();
                rigid.constraints =
                    RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationZ;
                rigid.useGravity = false;
                rigid.angularDrag = .25f;
            }
            rigid.angularVelocity = (new Vector3(0, -delta.x, 0));
        }
    }
    
    // COMPONENTS

    public static bool Remove<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return false;
        }

        T t = inst.Get<T>();
        
        if (t != default(T)) {
            UnityEngine.Object.Destroy(t);
            return true;
        }
        
        return false;
    }

    public static T GetOrSet<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return null;
        }
        
        if (!inst.Has<T>()) {
            return inst.AddComponent<T>();
        }
        else {
            return inst.Get<T>();
        }
    }

    public static T Set<T>(GameObject inst) where T : Component {
        return GetOrSet<T>(inst);
    }
    
    public static T Get<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return null;
        }
        
        foreach (T obj in inst.GetComponents<T>()) {
            return obj;
        }
        
        
        foreach (T obj in inst.GetComponentsInChildren<T>(true)) {
            return obj;
        }
        
        return null;
    }
    
    public static T Get<T>(GameObject inst, string name) where T : Component {
        if (inst == null) {
            return null;
        }
        
        foreach (T obj in inst.GetComponents<T>()) {
            if (obj.name == name) {
                return obj;
            }
        }
        
        
        foreach (T obj in inst.GetComponentsInChildren<T>(true)) {
            if (obj.name == name) {
                return obj;
            }
        }
        
        return null;
    }

    public static List<T> GetList<T>(GameObject inst, string name) where T : Component {
        if (inst == null) {
            return null;
        }

        List<T> list = new List<T>();
        
        foreach (T obj in inst.GetComponents<T>()) {
            if (obj.name == name) {
                list.Add(obj);
            }
        }
                
        foreach (T obj in inst.GetComponentsInChildren<T>(true)) {
            if (obj.name == name) {
                list.Add(obj);
            }
        }
        
        return list;
    }
    
    public static T[] GetList<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return null;
        }
        return inst.GetComponentsInChildren<T>(true);
    }
    
    public static bool Has<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return false;
        }
        
        if (inst.GetComponentsInChildren<T>(true).Length > 0
            || inst.GetComponents<T>().Length > 0) {
            return true;
        }
        
        return false;
    }
    
    // VISIBILITY
    
    public static void Show(GameObject inst) {
        //LogUtil.Log("Show:" + inst.name);
        if (inst != null) {
            if (!inst.activeSelf) {
                inst.SetActive(true);                
                ShowRenderers(inst);
            }
        }
    }
    
    public static void Hide(GameObject inst) {
        //LogUtil.Log("Hide:" + inst.name);
        if (inst != null) {
            if (inst.activeSelf || inst.activeInHierarchy) {
                HideRenderers(inst);
                inst.SetActive(false);
            }
        }
    }
        
    public static void ShowObjectDelayed(GameObject inst, float delay) {
        if (inst == null)
            return;

        CoroutineUtil.Start(ShowObjectDelayedCo(inst, delay));
    }
    
    public static void HideObjectDelayed(GameObject inst, float delay) {
        if (inst == null)
            return;
        
        CoroutineUtil.Start(HideObjectDelayedCo(inst, delay));
    }

    public static IEnumerator ShowObjectDelayedCo(GameObject inst, float delay) {
        if (inst == null)
            yield break;

        yield return new WaitForSeconds(delay);

        inst.Show();
    }
    
    public static IEnumerator HideObjectDelayedCo(GameObject inst, float delay) {
        if (inst == null)
            yield break;
        
        yield return new WaitForSeconds(delay);
        
        inst.Hide();
    }
    
    public static void ShowObject(GameObject inst) {
        if (inst == null)
            return;
        
        inst.Show();
    }
    
    public static void HideObject(GameObject inst) {
        if (inst == null)
            return;
        
        inst.Hide();
    }
    
    public static void HideChildren(GameObject inst, bool applyGameObjectInactive) {
        if (inst == null)
            return;
        
        List<Transform> transforms = new List<Transform>();// inst.transform.childCount;
        
        int b = 0;
        foreach (Transform t in inst.transform) {
            transforms.Add(t);// = t;
            b++;
        }
        
        foreach (Transform t in transforms) {
            //t.parent = null;
            if (applyGameObjectInactive) {
                if (t.gameObject.GetComponent<GameObjectInactive>()) {
                    // already has it
                }
                else {
                    t.gameObject.AddComponent<GameObjectInactive>();
                }
            }
            t.gameObject.Hide();
        }
        
        transforms.Clear();
        transforms = null;
    }
    
    public static void HideChildren(GameObject inst) {
        HideChildren(inst, false);
    }
    
    // ANIMATIONS
    
    public static void ResetAnimations(GameObject inst) {
        if (inst == null)
            return;

        Animation anim = inst.GetComponent<Animation>();

        if (anim != null) {
            anim.Stop();
            anim.Rewind();
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            source.Stop();
            source.Rewind();
        }
    }
    
    public static void PlayAnimation(GameObject inst, string name, float speed = 1f) {
        if (inst == null)
            return;
        
        Animation anim = inst.GetComponent<Animation>();

        if (anim != null) {
            if (anim[name] != null) {
                if(anim.isPlaying) {
                    anim.Stop();
                }
                if (!anim.isPlaying) {
                    anim[name].speed = 1;
                    anim.Play(name);
                }
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source[name] != null) {
                if(source.isPlaying) {
                    source.Stop();
                }
                if (!source.isPlaying) {
                    source[name].speed = 1;
                    source.Play(name);
                }
            }
        }
    }
    
    public static void StopAnimation(GameObject inst, string name) {
        if (inst == null)
            return;

        Animation anim = inst.GetComponent<Animation>();
        
        if (anim != null) {
            if (anim[name] != null) {
                if (anim.isPlaying) {
                    anim.Stop(name);
                }
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source[name] != null) {
                if (source.isPlaying) {
                    source.Stop(name);
                }
            }
        }
    }
    
    public static void StepAnimationFrame(
        GameObject inst, string name, float time, bool normalizedTime = true, bool stopPlaying = true) {
        if (inst == null)
            return;
        
        Animation anim = inst.GetComponent<Animation>();
        
        if (anim != null) {
            if (anim[name] != null) {
                if (anim.isPlaying) {
                    anim.Stop();                    
                    anim.Play(name);
                }
                if (normalizedTime) {
                    anim[name].normalizedTime = time;
                }
                else {
                    anim[name].time = time;
                }
                if (stopPlaying) {
                    anim[name].speed = 0.0f;
                }
                if (!anim.isPlaying) {
                    anim.Play(name);
                }
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source[name] != null) {
                if (source.isPlaying) {
                    source.Stop();                    
                    source.Play(name);
                }
                if (normalizedTime) {
                    source[name].normalizedTime = time;
                }
                else {
                    source[name].time = time;
                }
                if (stopPlaying) {
                    source[name].speed = 0.0f;
                }
                if (!source.isPlaying) {
                    source.Play(name);
                }
            }
        }
    }
        
    public static void PlayAnimations(GameObject inst) {
        if (inst == null)
            return;
        
        Animation anim = inst.GetComponent<Animation>();
        
        if (anim != null) {
            if (!anim.isPlaying) {
                anim.Play();
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (!source.isPlaying) {
                source.Play();
            }
        }
    }
    
    public static void StopAnimations(GameObject inst) {
        if (inst == null)
            return;
        
        Animation anim = inst.GetComponent<Animation>();
        
        if (anim != null) {
            if (anim.isPlaying) {
                anim.Stop();
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source.isPlaying) {
                source.Stop();
            }
        }
    }
    
    public static void PauseAnimations(GameObject inst) {
        if (inst == null)
            return;
        
        Animation anim = inst.GetComponent<Animation>();
        
        if (anim != null) {
            if (anim.isPlaying) {
                anim.Stop();
            }
        }
        
        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source.isPlaying) {
                source.Stop();
            }
        }
    }
    
    // PARTICLE SYSTEMS
    
    public static void SetParticleSystemEmission(GameObject inst, bool emissionEnabled, bool includeChildren) {
        if (inst == null)
            return;
        
        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.enableEmission = emissionEnabled;
        }
        
        if (!includeChildren) {
            return;
        }
        
        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
        
        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.enableEmission = emissionEnabled;
        }
    }
    
    public static void SetParticleSystemEmissionRate(GameObject inst, float emissionRate, bool includeChildren) {
        if (inst == null)
            return;
        
        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.emissionRate = emissionRate;
        }
        
        if (!includeChildren) {
            return;
        }
        
        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
        
        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.emissionRate = emissionRate;
        }
    }
    
    public static void SetParticleSystemStartColor(GameObject inst, Color startColor, bool includeChildren) {
        if (inst == null)
            return;
        
        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.startColor = startColor;
        }
        
        if (!includeChildren) {
            return;
        }
        
        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
        
        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.startColor = startColor;
        }
    }
    
    public static void PlayParticleSystem(GameObject inst, bool includeChildren) {
        if (inst == null)
            return;
        
        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.enableEmission = true;
            if (!particleSystemCurrent.isPlaying) {
                particleSystemCurrent.Play();//.enableEmission = emissionEnabled;
            }
        }
        
        if (!includeChildren) {
            return;
        }
        
        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
        
        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.enableEmission = true;
            if (!particleSystem.isPlaying) {
                particleSystem.Play();//.enableEmission = emissionEnabled;
            }
        }
    }
    
    public static void StopParticleSystem(GameObject inst, bool includeChildren) {
        if (inst == null)
            return;
        
        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.enableEmission = false;
            if (particleSystemCurrent.isPlaying) {
                //particleSystemCurrent.Stop();
            }
        }
        
        if (!includeChildren) {
            return;
        }
        
        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
        
        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.enableEmission = false;
            if (particleSystem.isPlaying) {
                //particleSystem.Stop();
            }
        }
    }
    
    // TRAIL RENDERERS
    
    public static void SetTrailRendererColors(GameObject inst, Color color, bool includeChildren) {
        List<Color> colors = new List<Color>();
        colors.Add(color);
        SetTrailRendererColors(inst, colors, includeChildren);
        
    }
    
    public static void SetTrailRendererColors(GameObject inst, List<Color> colors, bool includeChildren) {
        if (inst == null)
            return;
        
        //TrailRenderer trailRendererCurrent = inst.GetComponent<TrailRenderer>();
        //if (trailRendererCurrent != null) {
        // foreach (Color color in colors) {
        //trailRendererCurrent..color = color;
        //}
        //}
        
        if (!includeChildren) {
            return;
        }
        
        TrailRenderer[] trailRenderers = inst.GetComponentsInChildren<TrailRenderer>(true);
        
        foreach (TrailRenderer trailRenderer in trailRenderers) {
            
            foreach (Color color in colors) {
                foreach (Material material in trailRenderer.materials) {
                    material.SetColor("_MainColor", color);
                }
            }
        }
    }
    
    // MATERIALS
    
    public static Material GetMaterial(GameObject inst, string name) {
        
        MeshRenderer[] renderers
            = inst.GetComponents<MeshRenderer>();
        
        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                if (FilterMaterialName(m.name) == FilterMaterialName(name)) {
                    return m;
                }
            }
        }
        
        MeshRenderer[] renderersChildren
            = inst.GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer mesh in renderersChildren) {
            foreach (Material m in mesh.materials) {
                if (FilterMaterialName(m.name) == FilterMaterialName(name)) {
                    return m;
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderers
            = inst.GetComponents<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderers) {
            foreach (Material m in mesh.materials) {
                if (FilterMaterialName(m.name) == FilterMaterialName(name)) {
                    return m;
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderersChildren
            = inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderersChildren) {
            foreach (Material m in mesh.materials) {
                if (FilterMaterialName(m.name) == FilterMaterialName(name)) {
                    return m;
                }
            }
        }
        
        return null;
    }
    
    public static List<Material> GetMaterials(GameObject inst, string name) {
        
        List<Material> materials = new List<Material>();
        
        MeshRenderer[] renderers
            = inst.GetComponents<MeshRenderer>();
        
        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                //if(FilterMaterialName(m.name) == FilterMaterialName(name)) {
                if (m.name.Contains(name)) {
                    materials.Add(m);
                }
            }
        }
        
        MeshRenderer[] renderersChildren
            = inst.GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer mesh in renderersChildren) {
            foreach (Material m in mesh.materials) {
                //if(FilterMaterialName(m.name) == FilterMaterialName(name)) {
                if (m.name.Contains(name)) {
                    materials.Add(m);
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderers
            = inst.GetComponents<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderers) {
            foreach (Material m in mesh.materials) {
                //if(FilterMaterialName(m.name) == FilterMaterialName(name)) {
                if (m.name.Contains(name)) {
                    materials.Add(m);
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderersChildren
            = inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderersChildren) {
            foreach (Material m in mesh.materials) {
                //if(FilterMaterialName(m.name) == FilterMaterialName(name)) {
                if (m.name.Contains(name)) {
                    materials.Add(m);
                }
            }
        }
        
        return materials;
    }
    
    public static string FilterMaterialName(string name) {
        //return name.ToLower().Replace(" (instance)","").Replace(" (clone)","");
        return name;
    }
    
    public static void SetMaterialSwap(GameObject inst, string nameFind, string materialResourcesPath) {
        
        //LogUtil.Log("SetMaterialColor name:" + name + " color:" + color );
        
        Material materialTo = MaterialUtil.LoadMaterialFromResources(materialResourcesPath);
        if (materialTo == null) {
            LogUtil.Log("Material not found:" + materialResourcesPath);
            return;
        }
        
        //LogUtil.Log("SetMaterialSwap:" + " materialTo:" + materialTo.name);
        
        MeshRenderer[] renderers = inst.GetComponents<MeshRenderer>();
        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );
        
        for (int i = 0; i < renderers.Length; i++) {
            for (int j = 0; j < renderers[i].materials.Length; j++) {
                if (FilterMaterialName(renderers[i].materials[j].name).Contains(nameFind)) {
                    Material[] materialsTo = renderers[i].materials;
                    materialsTo[j] = materialTo;
                    renderers[i].materials = materialsTo;
                    //LogUtil.Log("SetMaterialSwap:" + " materialTo2:" + materialTo.name);
                }
            }
        }
        
        MeshRenderer[] renderersChildren = inst.GetComponentsInChildren<MeshRenderer>(true);
        //LogUtil.Log("SetMaterialColor renderersChildren:" + renderersChildren.Length );
        
        for (int i = 0; i < renderersChildren.Length; i++) {
            for (int j = 0; j < renderersChildren[i].materials.Length; j++) {
                if (FilterMaterialName(renderersChildren[i].materials[j].name).Contains(nameFind)) {
                    Material[] materialsTo = renderersChildren[i].materials;
                    materialsTo[j] = materialTo;
                    renderersChildren[i].materials = materialsTo;
                    //LogUtil.Log("SetMaterialSwap:" + " materialTo3:" + materialTo.name);
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderers = inst.GetComponents<SkinnedMeshRenderer>();
        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );
        
        for (int i = 0; i < skinnedRenderers.Length; i++) {
            for (int j = 0; j < skinnedRenderers[i].materials.Length; j++) {
                if (FilterMaterialName(skinnedRenderers[i].materials[j].name).Contains(nameFind)) {
                    Material[] materialsTo = skinnedRenderers[i].materials;
                    materialsTo[j] = materialTo;
                    skinnedRenderers[i].materials = materialsTo;
                    //LogUtil.Log("SetMaterialSwap:" + " materialTo4:" + materialTo.name);
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderersChildren = inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        //LogUtil.Log("SetMaterialColor renderersChildren:" + renderersChildren.Length );
        
        for (int i = 0; i < skinnedRenderersChildren.Length; i++) {
            for (int j = 0; j < skinnedRenderersChildren[i].materials.Length; j++) {
                if (FilterMaterialName(skinnedRenderersChildren[i].materials[j].name).Contains(nameFind)) {
                    Material[] materialsTo = skinnedRenderersChildren[i].materials;
                    materialsTo[j] = materialTo;
                    skinnedRenderersChildren[i].materials = materialsTo;
                    //LogUtil.Log("SetMaterialSwap:" + " materialTo5:" + materialTo.name);
                }
            }
        }
    }
    
    public static bool SetMaterialColor(GameObject inst, string name, Color color, bool all) {
        
        //LogUtil.Log("SetMaterialColor name:" + name + " color:" + color );
        
        MeshRenderer[] renderers = inst.GetComponents<MeshRenderer>();
        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );
        
        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                //LogUtil.Log("SetMaterialColor m:" + m.name);
                if (FilterMaterialName(m.name).Contains(FilterMaterialName(name))) {
                    m.color = color;
                    m.SetColor("_MainColor", color);
                    m.SetColor("_Color", color);
                    m.SetColor("_TintColor", color);
                    //LogUtil.Log("SetMaterialColor color:" + color);
                    if (!all)
                        return true;
                }
            }
        }
        
        MeshRenderer[] renderersChildren = inst.GetComponentsInChildren<MeshRenderer>(true);
        //LogUtil.Log("SetMaterialColor renderersChildren:" + renderersChildren.Length );
        
        foreach (MeshRenderer mesh in renderersChildren) {
            foreach (Material m in mesh.materials) {
                //LogUtil.Log("SetMaterialColor m:" + m.name);
                if (FilterMaterialName(m.name).Contains(FilterMaterialName(name))) {
                    m.color = color;
                    m.SetColor("_MainColor", color);
                    m.SetColor("_Color", color);
                    m.SetColor("_TintColor", color);
                    //LogUtil.Log("SetMaterialColor color:" + color);
                    if (!all)
                        return true;
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderers = inst.GetComponents<SkinnedMeshRenderer>();
        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderers) {
            foreach (Material m in mesh.materials) {
                //LogUtil.Log("SetMaterialColor m:" + m.name);
                if (FilterMaterialName(m.name).Contains(FilterMaterialName(name))) {
                    m.color = color;
                    m.SetColor("_MainColor", color);
                    m.SetColor("_Color", color);
                    m.SetColor("_TintColor", color);
                    //LogUtil.Log("SetMaterialColor color:" + color);
                    if (!all)
                        return true;
                }
            }
        }
        
        SkinnedMeshRenderer[] skinnedRenderersChildren = inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        //LogUtil.Log("SetMaterialColor renderersChildren:" + renderersChildren.Length );
        
        foreach (SkinnedMeshRenderer mesh in skinnedRenderersChildren) {
            foreach (Material m in mesh.materials) {
                //LogUtil.Log("SetMaterialColor m:" + m.name);
                if (FilterMaterialName(m.name).Contains(FilterMaterialName(name))) {
                    m.color = color;
                    m.SetColor("_MainColor", color);
                    m.SetColor("_Color", color);
                    m.SetColor("_TintColor", color);
                    //LogUtil.Log("SetMaterialColor color:" + color);
                    if (!all)
                        return true;
                }
            }
        }
        
        return false;
    }
    
    public static bool SetMaterialColor(GameObject inst, string name, Color color) {
        return SetMaterialColor(inst, name, color, true);
    }
    
    public static GameObject CreateGameObject(
        GameObject go,
        Vector3 pos,
        Quaternion rotate,
        bool pooled) {
        
        GameObject obj = null;
        
        if (!pooled) {
            obj = GameObject.Instantiate(go, pos, rotate) as GameObject;
        }
        else {
            obj = ObjectPoolManager.createPooled(go, pos, rotate);
        }
        
        if (obj != null) {
            
            if (!obj.Has<PoolGameObject>()) {
                obj.AddComponent<PoolGameObject>();
            }
        }
        
        return obj;
    }

    // Pool keyed
        
    public static GameObject CreateGameObject(
        string key, 
        GameObject go,
        Vector3 pos,
        Quaternion rotate,
        bool pooled) {
        
        GameObject obj = null;
        
        if (!pooled) {
            obj = GameObject.Instantiate(go, pos, rotate) as GameObject;
        }
        else {
            obj = ObjectPoolKeyedManager.createPooled(key, go, pos, rotate);
        }
        
        if (obj != null) {
            
            if (!obj.Has<PoolGameObject>()) {
                obj.AddComponent<PoolGameObject>();
            }
        }   
        
        return obj;
    }
    
    public static void DestroyGameObject(GameObject go, bool pooled = true) {
        DestroyGameObject(go, 0f, pooled);
    }

    public static void DestroyGameObject(GameObject go, float delay, bool pooled = true) {
        if (!pooled && !go.Has<PoolGameObject>()) {
            DestroyDelayed(go, delay);
        }
        else if (go.Has<ObjectPoolKeyed>()) {
            ObjectPoolKeyedManager.destroyPooled(go, delay);
        }
        else {
            ObjectPoolManager.destroyPooled(go, delay);
        }
    }

    public static void DestroyNow(GameObject inst) {
        if (inst == null)
            return;
        
        GameObject.Destroy(inst);
    }
    
    public static void DestroyDelayed(GameObject inst, float delay) {
        if (inst == null)
            return;
        
        GameObject.Destroy(inst, delay);
    }
    
    public static void DestroyChildren(GameObject inst) {
        if (inst == null)
            return;
        
        List<Transform> transforms = new List<Transform>();// inst.transform.childCount;
        int b = 0;
        foreach (Transform t in inst.transform) {
            transforms.Add(t);// = t;
            b++;
        }
        
        foreach (Transform t in transforms) {
            try {
                if (t.gameObject.GetType() == typeof(GameObject)
                    && !t.IsPrefabGhost()) {
                    t.parent = null;
                    GameObject.Destroy(t.gameObject);
                }
            }
            catch (Exception e) {
                Debug.Log("ERROR:" + e);
            }
        }
        
        transforms.Clear();
        transforms = null;
    }
    
    public static void DestroyChildren(GameObject inst, bool pooled) {
        if (inst == null)
            return;
        
        List<Transform> transforms = new List<Transform>();// inst.transform.childCount;
        int b = 0;
        foreach (Transform t in inst.transform) {
            transforms.Add(t);// = t;
            b++;
        }
        
        foreach (Transform t in transforms) {
            t.parent = null;
            DestroyGameObject(t.gameObject, pooled);
        }
        
        transforms.Clear();
        transforms = null;
    }
    
    public static GameObject LoadFromResources(string path) {
        
        LogUtil.Log("LoadFromResources: path:" + path);
                
        UnityEngine.Object prefabObject = AssetUtil.LoadAsset<UnityEngine.Object>(path);

        if (prefabObject != null) {
            GameObject go = UnityEngine.GameObject.Instantiate(prefabObject) as GameObject;
            if (go != null) {
                return go;
            }
        }
        
        return null;
    }
    
    public static GameObject LoadFromBundle(string path) {
        
        return null;
    }
    
    // RIGIDBODY
    
    public static void FreezeRigidbodies(GameObject inst) {
        
        if (inst == null) {
            return;
        }
        
        Rigidbody[] rigidbodies
            = inst.GetComponents<Rigidbody>();
        
        foreach (Rigidbody r in rigidbodies) {
            r.Freeze();
        }
        
        Rigidbody[] rigidbodiesChildren
            = inst.GetComponentsInChildren<Rigidbody>(true);
        
        foreach (Rigidbody r in rigidbodiesChildren) {
            r.Freeze();
        }
    }

    public static void UnFreezeRigidbodies(GameObject inst) {
        
        if (inst == null) {
            return;
        }
        
        Rigidbody[] rigidbodies
            = inst.GetComponents<Rigidbody>();
        
        foreach (Rigidbody r in rigidbodies) {
            r.UnFreeze();
        }
        
        Rigidbody[] rigidbodiesChildren
            = inst.GetComponentsInChildren<Rigidbody>(true);
        
        foreach (Rigidbody r in rigidbodiesChildren) {
            r.UnFreeze();
        }
    }
    
    public static Rigidbody GetRigidbody(GameObject inst) {
        
        if (inst == null) {
            return null;
        }
        
        Rigidbody[] rigidbodies
            = inst.GetComponents<Rigidbody>();
        
        foreach (Rigidbody r in rigidbodies) {
            return r;
        }
        
        Rigidbody[] rigidbodiesChildren
            = inst.GetComponentsInChildren<Rigidbody>(true);
        
        foreach (Rigidbody r in rigidbodiesChildren) {
            return r;
        }

        return null;
    }

    public static void ResetRigidBodiesVelocity(GameObject inst, Vector3 angularVelocity) {
        
        if (inst == null) {
            return;
        }
        
        Rigidbody[] rigidbodies
            = inst.GetComponents<Rigidbody>();
        
        foreach (Rigidbody r in rigidbodies) {
            r.angularVelocity = angularVelocity;
        }
        
        Rigidbody[] rigidbodiesChildren
            = inst.GetComponentsInChildren<Rigidbody>(true);
        
        foreach (Rigidbody r in rigidbodiesChildren) {
            r.angularVelocity = angularVelocity;
        }
    }
    
    public static void ResetRigidBodiesVelocity(GameObject inst) {
        
        if (inst == null) {
            return;
        }
        
        ResetRigidBodiesVelocity(inst, Vector3.zero);
    }

    public static void Reset(GameObject inst) {
        
        if (inst == null) {
            return;
        }
        
        Rigidbody[] rigidbodies
            = inst.GetComponents<Rigidbody>();
        
        foreach (Rigidbody r in rigidbodies) {
            r.UnFreeze();
        }
        
        Rigidbody[] rigidbodiesChildren
            = inst.GetComponentsInChildren<Rigidbody>(true);
        
        foreach (Rigidbody r in rigidbodiesChildren) {
            r.UnFreeze();
        }
    }

    // ASPECT RATIO for textures

    public static void ResizePreservingAspectToScreen(
        GameObject inst, float desiredMaxWidth, float desiredMaxHeight) {
        
        if (inst == null) {
            return;
        }

        // current size 250x250
        
        float currentWidth = Screen.width;
        float currentHeight = Screen.height;
        
        float photoWidth = desiredMaxWidth;
        float photoHeight = desiredMaxHeight;
        
        float currentRatioWidth = photoWidth / currentWidth;
        float currentRatioHeight = photoHeight / currentHeight;
        
        if (currentRatioHeight < currentRatioWidth) {
            currentWidth *= currentRatioHeight;
            currentHeight *= currentRatioHeight;
        }
        else if (currentRatioWidth < currentRatioHeight) {
            currentWidth *= currentRatioWidth;
            currentHeight *= currentRatioWidth;
        }
        
        inst.transform.localScale 
            = inst.transform.localScale
                .WithX(currentWidth).WithY(currentHeight);          
    }
    
}