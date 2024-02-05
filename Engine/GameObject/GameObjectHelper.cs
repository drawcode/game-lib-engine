using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Engine.Game.Data;
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

        Transform child = inst.transform.Find(name);

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

    public static bool ContainsChildLike(GameObject inst, string nameLike) {
        if (inst == null) {
            return false;
        }

        foreach (Transform t in inst.transform) {

            if (t.name.Contains(nameLike)) {
                return true;
            }

            //return ContainsChildLike(t.gameObject, nameLike);
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

        if (cam == null) {
            return null;
        }

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
        UnityEngine.Object[] objs = UnityEngine.GameObject.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
        foreach (UnityEngine.Object obj in objs) {
            GameObject go = obj as GameObject;
            if (go.transform.parent == null) {
                DumpGoToLog(go);
            }
        }
    }

    public static void DumpGoToLog(GameObject go) {
        Debug.Log("DUMP: go:" + go.name + "::::" + GameObjectHelper.DumpGo(go));
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

        sb.Append(string.Format("\r\n{0}+{1} - a:{2} - r:{3} - mid:{4} - loadedObj: {5} - scale: x:{6} y:{7} z:{8} - pos: x:{9} y:{10} z:{11} - layer:{12}",
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
                                go.transform.position.z,
                                LayerMask.LayerToName(go.layer)));

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

    public static T SetOnly<T>(GameObject inst) where T : Component {
        if (inst == null) {
            return null;
        }

        T t = inst.GetComponent<T>();

        if (t == default(T)) {
            return inst.AddComponent<T>();
        }

        return t;
    }

    // 

    public static GameObject GetAsGameObject<T>(
        GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        if (inst.Has<T>()) {
            return inst.Get<T>().gameObject;
        }

        return null;
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

    public static List<T> GetList<T>(GameObject inst) where T : Component {
        //if (inst == null) {
        //    return null;
        //}
        //return inst.GetComponentsInChildren<T>(true);

        if (inst == null) {
            return null;
        }

        List<T> list = new List<T>();

        foreach (T obj in inst.GetComponents<T>()) {
            list.Add(obj);
        }

        foreach (T obj in inst.GetComponentsInChildren<T>(true)) {
            list.Add(obj);
        }

        return list;
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

    public static void ShowChildren(GameObject inst, bool applyGameObjectInactive = false) {
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
            t.gameObject.Show();
        }

        transforms.Clear();
        transforms = null;
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

    public static void PlayAnimationBlend(GameObject inst, string name, float speed = 1f, float targetWeight = .9f, float fadeLength = .5f) {
        if (inst == null)
            return;

        Animation anim = inst.GetComponent<Animation>();

        if (anim != null) {
            if (anim[name] != null) {
                if (anim.isPlaying) {
                    anim.Stop();
                }
                if (!anim.isPlaying) {
                    anim[name].speed = speed;
                    anim.Blend(name, targetWeight, fadeLength);
                }
            }
        }

        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source[name] != null) {
                if (source.isPlaying) {
                    source.Stop();
                }
                if (!source.isPlaying) {
                    source[name].speed = speed;
                    anim.Blend(name, targetWeight, fadeLength);
                }
            }
        }
    }

    public static void PlayAnimationCrossFade(
        GameObject inst, string name, float speed = 1f, float fadeLength = .5f,
        PlayMode playMode = PlayMode.StopSameLayer) {
        if (inst == null)
            return;

        Animation anim = inst.GetComponent<Animation>();

        if (anim != null) {
            if (anim[name] != null) {
                if (anim.isPlaying) {
                    anim.Stop();
                }
                if (!anim.isPlaying) {
                    anim[name].speed = speed;
                    anim.CrossFade(name, fadeLength, playMode);
                }
            }
        }

        foreach (Animation source in inst.GetComponentsInChildren<Animation>()) {
            if (source[name] != null) {
                if (source.isPlaying) {
                    source.Stop();
                }
                if (!source.isPlaying) {
                    source[name].speed = speed;
                    anim.CrossFade(name, fadeLength, playMode);
                }
            }
        }
    }

    public static void PlayAnimation(GameObject inst, string name, float speed = 1f) {
        if (inst == null)
            return;

        Animation anim = inst.GetComponent<Animation>();

        if (anim != null) {
            if (anim[name] != null) {
                if (anim.isPlaying) {
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
                if (source.isPlaying) {
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

    public static List<AnimationState> GetAnimationsList(GameObject inst) {

        List<AnimationState> anims = new List<AnimationState>();

        if (inst == null) {
            return anims;
        }

        Animation anim = inst.GetComponent<Animation>();

        if (anim == null) {
            anim = inst.GetComponentInChildren<Animation>();
        }

        foreach (AnimationState state in anim) {
            anims.Add(state);
        }

        return anims;
    }

    public static AnimationState GetAnimationsRandom(GameObject inst) {

        List<AnimationState> anims = GetAnimationsList(inst);

        if (anims == null || anims.Count == 0) {
            return null;
        }

        int animIndex = UnityEngine.Random.Range(0, anims.Count - 1);

        return anims[animIndex];
    }

    public static void PlayAnimationBlendRandom(GameObject inst,
        float speed = 1, float targetWeight = 0.9f, float fadeLength = 0.5f) {

        if (inst == null) {
            return;
        }

        foreach (Animation anim in inst.GetComponentsInChildren<Animation>()) {

            AnimationState animationState = GetAnimationsRandom(inst);

            if (animationState == null) {
                continue;
            }

            anim.gameObject.PlayAnimationBlend(animationState.name, speed, targetWeight, fadeLength);
        }
    }

    public static void PlayAnimationCrossFadeRandom(GameObject inst,
        float speed = 1, float fadeLength = 0.5f) {

        if (inst == null) {
            return;
        }

        foreach (Animation anim in inst.GetComponentsInChildren<Animation>()) {

            AnimationState animationState = GetAnimationsRandom(inst);

            if (animationState == null) {
                continue;
            }

            anim.gameObject.PlayAnimationCrossFade(animationState.name, speed, fadeLength);
        }
    }

    // PARTICLE SYSTEMS

    public static void SetParticleSystemEmission(GameObject inst, bool emissionEnabled, bool includeChildren) {
        if (inst == null)
            return;

        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();

        if (particleSystemCurrent != null) {

            if (!emissionEnabled) {
                particleSystemCurrent.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystemCurrent.gameObject.PlayParticleSystem(false);
            }

            particleSystemCurrent.EnableEmission(emissionEnabled);
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {

            if (!emissionEnabled) {
                particleSystem.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystem.gameObject.PlayParticleSystem(false);
            }

            particleSystem.EnableEmission(emissionEnabled);
        }
    }

    public static void SetParticleSystemEmissionRate(GameObject inst, float emissionRate, bool includeChildren) {
        if (inst == null)
            return;

        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {

            if (emissionRate > 0) {
                particleSystemCurrent.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystemCurrent.gameObject.PlayParticleSystem(false);
            }

            particleSystemCurrent.SetEmissionRate(emissionRate);
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {

            if (emissionRate <= 0) {
                particleSystem.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystem.gameObject.PlayParticleSystem(false);
            }

            particleSystem.SetEmissionRate(emissionRate);
        }
    }

    public static void SetParticleSystemEmissionRateNormalizedFlipped(
        GameObject inst, float emissionRateNormalized, bool includeChildren) {

        SetParticleSystemEmissionRateNormalized(inst, emissionRateNormalized, includeChildren, true);
    }

    public static void SetParticleSystemEmissionRateNormalized(
        GameObject inst, float emissionRateNormalized, bool includeChildren) {

        SetParticleSystemEmissionRateNormalized(inst, emissionRateNormalized, includeChildren, false);
    }

    public static void SetParticleSystemEmissionRateNormalized(
        GameObject inst, float emissionRateNormalized, bool includeChildren, bool flipped) {

        if (inst == null) {
            return;
        }

        GameObjectData goData = null;

        float currentEmissionRate = 0;
        float initialEmissionRate = 0;

        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();

        if (particleSystemCurrent != null) {

            currentEmissionRate = 0;
            initialEmissionRate = 0;

            goData = null;

            currentEmissionRate = particleSystemCurrent.GetEmissionRate();
            initialEmissionRate = particleSystemCurrent.GetEmissionRate();

            goData = particleSystemCurrent.gameObject.Get<GameObjectData>();

            if (goData == null) {
                goData = particleSystemCurrent.gameObject.GetOrSet<GameObjectData>();
                goData.Set(
                    BaseDataObjectKeys.particleEmissionRate,
                    initialEmissionRate);
            }

            object valEmission = goData.Get(BaseDataObjectKeys.particleEmissionRate);

            if (valEmission != null) {
                initialEmissionRate = goData.GetFloat(BaseDataObjectKeys.particleEmissionRate);
            }

            if (flipped) {
                currentEmissionRate = (initialEmissionRate * (1 - emissionRateNormalized));
            }
            else {
                currentEmissionRate = (initialEmissionRate * emissionRateNormalized);
            }

            // (22 * (1 - .1))

            particleSystemCurrent.SetEmissionRate(currentEmissionRate);

            if (emissionRateNormalized <= 0) {
                particleSystemCurrent.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystemCurrent.gameObject.PlayParticleSystem(false);
            }
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {

            currentEmissionRate = 0;
            initialEmissionRate = 0;

            goData = null;

            currentEmissionRate = particleSystem.GetEmissionRate();
            initialEmissionRate = particleSystem.GetEmissionRate();

            goData = particleSystem.gameObject.Get<GameObjectData>();

            if (goData == null) {
                goData = particleSystem.gameObject.GetOrSet<GameObjectData>();
                goData.Set(
                    BaseDataObjectKeys.particleEmissionRate,
                    initialEmissionRate);
            }

            object valEmission = goData.Get(BaseDataObjectKeys.particleEmissionRate);

            if (valEmission != null) {
                initialEmissionRate = goData.GetFloat(BaseDataObjectKeys.particleEmissionRate);
            }

            currentEmissionRate = (initialEmissionRate * (1 - emissionRateNormalized));
            // (22 * (1 - .1))

            particleSystem.SetEmissionRate(currentEmissionRate);

            if (emissionRateNormalized <= 0) {
                particleSystem.gameObject.StopParticleSystem(false);
            }
            else {
                particleSystem.gameObject.PlayParticleSystem(false);
            }
        }
    }

    public static void SetParticleSystemStartColor(GameObject inst, Color startColor, bool includeChildren) {
        if (inst == null)
            return;

        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            //particleSystemCurrent.startColor = startColor;
            ParticleSystem.MainModule main = particleSystemCurrent.main;
            main.startColor = startColor;
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = startColor;
        }
    }

    public static void PlayParticleSystem(GameObject inst, bool includeChildren) {
        if (inst == null)
            return;

        ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
        if (particleSystemCurrent != null) {
            particleSystemCurrent.EnableEmission(true);
            if (!particleSystemCurrent.isPlaying) {
                particleSystemCurrent.Play();//.enableEmission = emissionEnabled;
            }
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.EnableEmission(true);
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
            particleSystemCurrent.EnableEmission(false);
            if (particleSystemCurrent.isPlaying) {
                //particleSystemCurrent.Stop();
            }
        }

        if (!includeChildren) {
            return;
        }

        ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.EnableEmission(false);
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

    // MATERIAL VALUES

    public class MaterialTextureOffset {
        public Vector2 val = new Vector2();
    }

    public class MaterialTextureScale {
        public Vector2 val = new Vector2();
    }

    public static T GetMaterialValue<T>(
        GameObject inst, string key, string materialNameFind = "*") {

        T t = default(T);

        MeshRenderer[] renderers =
            inst.GetComponents<MeshRenderer>();

        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                t = GetMaterialValue<T>(m, key, materialNameFind);

                if (!t.Equals(default(T))) {
                    return t;
                }
            }
        }

        MeshRenderer[] renderersChildren =
            inst.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer mesh in renderersChildren) {
            foreach (Material m in mesh.materials) {
                t = GetMaterialValue<T>(m, key, materialNameFind);

                if (!t.Equals(default(T))) {
                    return t;
                }
            }
        }

        SkinnedMeshRenderer[] skinnedRenderers =
            inst.GetComponents<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in skinnedRenderers) {
            foreach (Material m in mesh.materials) {
                t = GetMaterialValue<T>(m, key, materialNameFind);

                if (!t.Equals(default(T))) {
                    return t;
                }
            }
        }

        SkinnedMeshRenderer[] skinnedRenderersChildren =
            inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer mesh in skinnedRenderersChildren) {
            foreach (Material m in mesh.materials) {
                t = GetMaterialValue<T>(m, key, materialNameFind);

                if (!t.Equals(default(T))) {
                    return t;
                }
            }
        }

        return t;
    }

    public static T GetMaterialValue<T>(
        Material mat, string key, string materialNameFind = "*") {

        T val = default(T);

        if (!mat.HasProperty(key)) {
            return val;
        }

        string matCurrentName = FilterMaterialName(mat.name);
        string matContainsName = FilterMaterialName(materialNameFind);

        if (matCurrentName.Contains(matContainsName) || materialNameFind == "*") {

            // float

            if (typeof(T) == typeof(float)) {

                val = (T)(object)mat.GetFloat(key);
            }

            else if (typeof(T) == typeof(List<Color>)) {

                val = (T)(object)mat.GetFloatArray(key);
            }

            // texture

            else if (typeof(T) == typeof(Texture)) {

                val = (T)(object)mat.GetTexture(key);
            }
            else if (typeof(T) == typeof(MaterialTextureOffset)) {

                MaterialTextureOffset item = new MaterialTextureOffset();
                item.val = (Vector2)(object)mat.GetTextureOffset(key);
                val = (T)(object)item;
            }
            else if (typeof(T) == typeof(MaterialTextureScale)) {

                MaterialTextureScale item = new MaterialTextureScale();
                item.val = (Vector2)(object)mat.GetTextureScale(key);
                val = (T)(object)item;
            }

            // vec4

            else if (typeof(T) == typeof(Vector4)) {

                val = (T)(object)mat.GetVector(key);
            }
            else if (typeof(T) == typeof(List<Vector4>)) {

                val = (T)(object)mat.GetVectorArray(key);
            }

            // int

            else if (typeof(T) == typeof(int)) {

                val = (T)(object)mat.GetInt(key);
            }

            // matrix4x4

            else if (typeof(T) == typeof(Matrix4x4)) {

                val = (T)(object)mat.GetMatrix(key);
            }
            else if (typeof(T) == typeof(List<Matrix4x4>)) {

                val = (T)(object)mat.GetMatrixArray(key);
            }

            // color

            else if (typeof(T) == typeof(Color)) {

                val = (T)(object)mat.GetColor(key);
            }
            else if (typeof(T) == typeof(List<Color>)) {

                val = (T)(object)mat.GetColorArray(key);
            }
        }

        return val;
    }

    // material values set

    public static bool SetMaterialValue<T>(
        Material mat, string key, T val, string matNameLike = "*") {

        if (!mat.HasProperty(key)) {
            return false;
        }

        string matCurrentName = FilterMaterialName(mat.name);
        string matContainsName = FilterMaterialName(matNameLike);

        if (matCurrentName.Contains(matContainsName) || matNameLike == "*") {

            object v = (object)val;

            // float

            if (typeof(T) == typeof(float)) {

                mat.SetFloat(key, (float)v);
            }

            else if (typeof(T) == typeof(List<Color>)) {

                mat.SetFloatArray(key, (List<float>)v);
            }

            // texture

            else if (typeof(T) == typeof(Texture)) {

                mat.SetTexture(key, (Texture)v);
            }
            else if (typeof(T) == typeof(MaterialTextureOffset)) {

                mat.SetTextureOffset(key, ((MaterialTextureOffset)v).val);
            }
            else if (typeof(T) == typeof(MaterialTextureScale)) {

                mat.SetTextureScale(key, ((MaterialTextureScale)v).val);
            }

            // vec4

            else if (typeof(T) == typeof(Vector4)) {

                mat.SetVector(key, (Vector4)v);
            }
            else if (typeof(T) == typeof(List<Vector4>)) {

                mat.SetVectorArray(key, (List<Vector4>)v);
            }

            // compute buffer

            else if (typeof(T) == typeof(ComputeBuffer)) {

                mat.SetBuffer(key, (ComputeBuffer)v);
            }

            // int

            else if (typeof(T) == typeof(int)) {

                mat.SetInt(key, (int)v);
            }

            // matrix4x4

            else if (typeof(T) == typeof(Matrix4x4)) {

                mat.SetMatrix(key, (Matrix4x4)v);
            }
            else if (typeof(T) == typeof(List<Matrix4x4>)) {

                mat.SetMatrixArray(key, (List<Matrix4x4>)v);
            }

            // color

            else if (typeof(T) == typeof(Color)) {

                mat.SetColor(key, (Color)v);
            }
            else if (typeof(T) == typeof(List<Color>)) {

                mat.SetColorArray(key, (List<Color>)v);
            }

            return true;
        }
        return false;
    }

    public static bool SetMaterialValue<T>(
        GameObject inst, string key, T val, string name = "*") {

        MeshRenderer[] renderers =
            inst.GetComponents<MeshRenderer>();

        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                SetMaterialValue<T>(m, key, val, name);
            }
        }

        MeshRenderer[] renderersChildren =
            inst.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer mesh in renderersChildren) {
            foreach (Material m in mesh.materials) {
                SetMaterialValue<T>(m, key, val, name);
            }
        }

        SkinnedMeshRenderer[] skinnedRenderers =
            inst.GetComponents<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in skinnedRenderers) {
            foreach (Material m in mesh.materials) {
                SetMaterialValue<T>(m, key, val, name);
            }
        }

        SkinnedMeshRenderer[] skinnedRenderersChildren =
            inst.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer mesh in skinnedRenderersChildren) {
            foreach (Material m in mesh.materials) {
                SetMaterialValue<T>(m, key, val, name);
            }
        }

        return false;
    }

    public static void SetMaterialSwapItem<T>(
        GameObject inst, string regexNameFind, Material materialTo) where T : Renderer {

        List<T> renderers = inst.GetList<T>();

        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );

        for (int i = 0; i < renderers.Count; i++) {

            for (int j = 0; j < renderers[i].materials.Length; j++) {

                string matName = renderers[i].materials[j].name;

                if (FilterMaterialName(matName).RegexIsMatch(regexNameFind)) {

                    Material[] materialsTo = renderers[i].materials;

                    materialsTo[j] = materialTo;
                    renderers[i].materials = materialsTo;

                    //LogUtil.Log("SetMaterialSwap:" + " materialTo2:" + materialTo.name);
                }
            }
        }
    }

    // material swap/change

    public static void SetMaterialSwap(
        GameObject inst, string nameFind, string materialResourcesPath) {

        //LogUtil.Log("SetMaterialColor name:" + name + " color:" + color );

        Material materialTo =
            MaterialUtil.LoadMaterialFromResources(materialResourcesPath);

        if (materialTo == null) {
            LogUtil.Log("Material not found:" + materialResourcesPath);
            return;
        }

        if (inst.Has<MeshRenderer>()) {
            SetMaterialSwapItem<MeshRenderer>(inst, nameFind, materialTo);
        }

        if (inst.Has<SkinnedMeshRenderer>()) {
            SetMaterialSwapItem<SkinnedMeshRenderer>(inst, nameFind, materialTo);
        }
    }

    /*
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
    */

    // material color

    public static bool SetMaterialColor(
        GameObject inst, string name, Color color,
        bool all, bool includeColor,
        List<string> matProps, bool standard = false) {

        //LogUtil.Log("SetMaterialColor name:" + name + " color:" + color );

        MeshRenderer[] renderers = inst.GetComponents<MeshRenderer>();
        //LogUtil.Log("SetMaterialColor renderers:" + renderers.Length );

        foreach (MeshRenderer mesh in renderers) {
            foreach (Material m in mesh.materials) {
                //LogUtil.Log("SetMaterialColor m:" + m.name);
                string matCurrentName = FilterMaterialName(m.name);
                string matContainsName = FilterMaterialName(name);
                if (matCurrentName.Contains(matContainsName)) {
                    if (includeColor) {
                        m.color = color;
                    }
                    foreach (string matProp in matProps) {
                        m.SetColor(matProp, color);
                        if (standard) {
                            m.SetColor("_MainTex", Color.black);
                            m.SetColor("_Color", Color.black);
                        }
                    }
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
                string matCurrentName = FilterMaterialName(m.name);
                string matContainsName = FilterMaterialName(name);
                if (matCurrentName.Contains(matContainsName)) {
                    if (includeColor) {
                        m.color = color;
                    }
                    foreach (string matProp in matProps) {
                        m.SetColor(matProp, color);
                        if (standard) {
                            m.SetColor("_MainTex", Color.black);
                            m.SetColor("_Color", Color.black);
                        }
                    }
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
                string matCurrentName = FilterMaterialName(m.name);
                string matContainsName = FilterMaterialName(name);
                if (matCurrentName.Contains(matContainsName)) {
                    if (includeColor) {
                        m.color = color;
                    }
                    foreach (string matProp in matProps) {
                        m.SetColor(matProp, color);
                        if (standard) {
                            m.SetColor("_MainTex", Color.black);
                            m.SetColor("_Color", Color.black);
                        }
                    }
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
                string matCurrentName = FilterMaterialName(m.name);
                string matContainsName = FilterMaterialName(name);
                if (matCurrentName.Contains(matContainsName)) {
                    if (includeColor) {
                        m.color = color;
                    }
                    foreach (string matProp in matProps) {
                        m.SetColor(matProp, color);
                        if (standard) {
                            m.SetColor("_MainTex", Color.black);
                            m.SetColor("_Color", Color.black);
                        }
                    }
                    //LogUtil.Log("SetMaterialColor color:" + color);
                    if (!all)
                        return true;
                }
            }
        }

        return false;
    }

    //public static bool SetMaterialColor(GameObject inst, string name, Color color) {
    //    return SetMaterialColor(inst, name, color, true);
    //}

    public static bool SetMaterialColor(GameObject inst, string name, Color color, bool all = true) {

        List<string> mats = new List<string>();
        mats.Add("_MainColor");
        mats.Add("_Color");
        mats.Add("_TintColor");

        //return SetMaterialColor(inst, name, color, all, true, mats, true);
        return SetMaterialColor(inst, name, color, all, true, mats, false);
    }

    public static bool SetMaterialColorStandard(GameObject inst, string name, Color color, bool all = true) {

        List<string> mats = new List<string>();
        mats.Add("_SpecColor");
        //mats.Add("_EmissionColor");

        //return SetMaterialColor(inst, name, color, all, false, mats);
        return SetMaterialColor(inst, name, color, all, false, mats, true);
    }

    // ------------------------------------------------------------------------
    // game object
    public static bool IsPrefab(GameObject inst) {

        if (inst == null) {
            return false;
        }

        return inst.scene.rootCount == 0 || inst.scene.name == null;
    }

    public static bool IsPrefabGhost(Transform inst) {

        if (inst == null) {
            return false;
        }

        var tmp = new GameObject();

        try {
            tmp.transform.parent = inst.parent;

            var index = inst.GetSiblingIndex();

            inst.SetSiblingIndex(int.MaxValue);
            if (inst.GetSiblingIndex() == 0)
                return true;

            inst.SetSiblingIndex(index);
            return false;
        }
        finally {
            //UnityEngine.Object.DestroyImmediate(tmp);
            UnityEngine.Object.Destroy(tmp);
        }
    }


    public static GameObject CleanGameObjectName(
        GameObject go) {

        if (go.name.Contains(" (Clone)")) {
            go.name = go.name.Replace(" (Clone)", "");
        }
        if (go.name.Contains("(Clone)")) {
            go.name = go.name.Replace("(Clone)", "");
        }
        if (go.name.Contains("(clone)")) {
            go.name = go.name.Replace("(clone)", "");
        }

        return go;
    }

    public static GameObject CreateGameObject(
        GameObject go,
        Vector3 pos,
        Quaternion rotate,
        bool pooled) {

        string key = "default";

        if (go != null) {
            key = go.name.ToDelimited();
        }

        return CreateGameObject(key, go, pos, rotate, pooled);
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

            if (obj != null) {

                if (!obj.Has<PoolGameObject>()) {
                    obj.AddComponent<PoolGameObject>();
                }
            }
        }

        obj = CleanGameObjectName(obj);

        return obj;
    }

    public static void DestroyGameObject(GameObject go, bool pooled = true) {
        DestroyGameObject(go, 0f, pooled);
    }

    public static void DestroyGameObject(GameObject go, float delay, bool pooled = true) {

        if (go == null) {
            return;
        }

        if (!pooled || !go.Has<PoolGameObject>()) {
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
                    && !t.gameObject.IsPrefab()
                    ) {
                    //&& !t.IsPrefabGhost()) {
                    t.parent = null;
                    DestroyGameObject(t.gameObject);
                    //GameObject.Destroy(t.gameObject);
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

    // ------------------------------------------------------------------------
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

    public static void ResetRigidBodies(GameObject inst) {

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

    // POSITION RELATIVE

    public static float Distance(GameObject inst, GameObject to) {
        return Vector3.Distance(inst.transform.position, to.transform.position);
    }
}