using System;
using System.Collections.Generic;

#if !UNITY_WEBPLAYER
using System.Reflection;
#endif
using UnityEngine;

using Engine.Utility;

public static class BaseGameObjectExtensions {

    // GAME OBJECT

    public static bool ContainsChild(this GameObject inst, string name) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.ContainsChild(inst, name);
    }

    public static bool ContainsChildLike(this GameObject inst, string nameLike) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.ContainsChildLike(inst, nameLike);
    }

    public static void SetLayerRecursively(this GameObject inst, int layer) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetLayerRecursively(inst, layer);
    }

    public static void SetLayerRecursively(this GameObject inst, string name) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetLayerRecursively(inst, name);
    }

    public static void AddComponentsFromResource(this GameObject inst, string path) {

        var go = Resources.Load(path) as GameObject;

        foreach (var src in go.GetComponents<Component>()) {

            var dst = inst.AddComponent(src.GetType()) as Behaviour;
            dst.enabled = false;
            ComponentUtil.Copy(dst, src);
            dst.enabled = true;
        }
    }

    public static T AddComponentFromResource<T>(this GameObject inst, string path)
        where T : Component {

        var go = Resources.Load(path) as GameObject;

        if (go == null) {
            throw new ArgumentException("Invalid component path", "path");
        }

        var src = go.GetComponent<T>();
        var dst = inst.AddComponent<T>();

        ComponentUtil.Copy(dst, src);

        return dst;
    }

    public static T GetComponent<T>(this GameObject inst)
        where T : class {

        return inst.GetComponent(typeof(T)) as T;
    }

    public static GameObject FindTypeAboveObject<T>(this GameObject inst)
        where T : Component {

        if (inst == null) {

            return inst;
        }

        return FindTypeAboveObjectRecursive<T>(inst);
    }

    public static GameObject FindTypeAboveObjectRecursive<T>(this GameObject inst)
        where T : Component {

        if (inst == null) {
            return null;
        }

        if (inst != null) {

            T instItem = inst.GetComponent<T>();

            if (instItem != null) {
                return inst;
            }

            if (inst.transform.parent != null) {
                return FindTypeAboveObjectRecursive<T>(inst.transform.parent.gameObject);
            }
        }

        return null;
    }

    public static T FindTypeBelowRecursive<T>(this GameObject inst, string name)
        where T : Component {

        if (inst == null) {
            return null;
        }

        if (inst != null) {

            foreach (T instItem in inst.GetComponents<T>()) {

                if (instItem != null && instItem.name == name) {
                    return instItem;
                }
            }

            foreach (Transform t in inst.transform) {
                return FindTypeAboveRecursive<T>(t.gameObject);
            }
        }

        return default(T);
    }

    public static GameObject FindTypeBelowObjectRecursive<T>(this GameObject inst, string name)
        where T : Component {

        if (inst == null) {
            return null;
        }

        if (inst != null) {

            foreach (T instItem in inst.GetComponents<T>()) {

                if (instItem != null && instItem.name == name) {
                    return inst;
                }
            }

            foreach (Transform t in inst.transform) {
                return FindTypeAboveObjectRecursive<T>(t.gameObject);
            }
        }

        return null;
    }

    public static T FindTypeAbove<T>(this GameObject inst)
        where T : Component {

        if (inst == null) {
            return default(T);
        }

        return FindTypeAboveRecursive<T>(inst);
    }

    public static T FindTypeAboveRecursive<T>(this GameObject inst)
        where T : Component {

        if (inst == null) {
            return null;
        }

        if (inst != null) {

            T instItem = inst.GetComponent<T>();

            if (instItem != null) {
                return instItem;
            }

            if (inst.transform.parent != null) {
                return FindTypeAboveRecursive<T>(inst.transform.parent.gameObject);
            }
        }

        return null;
    }

    public static Transform FindBelow(this GameObject inst, string name) {

        if (inst == null) {
            return null;
        }

        if (inst.transform.childCount == 0) {
            return null;
        }

        var child = inst.transform.Find(name);

        if (child != null) {
            return child;
        }

        foreach (GameObject t in inst.transform) {

            child = FindBelow(t, name);

            if (child != null) {
                return child;
            }
        }
        return null;
    }

    public static bool Remove<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.Remove<T>(inst);
    }

    public static T GetOrSet<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetOrSet<T>(inst);
    }

    public static T Set<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.Set<T>(inst);
    }

    public static T SetOnly<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.SetOnly<T>(inst);
    }

    public static GameObject GetAsGameObject<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetAsGameObject<T>(inst);
    }

    public static T Get<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.Get<T>(inst);
    }

    public static T Get<T>(this GameObject inst, string name) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.Get<T>(inst, name);
    }

    public static List<T> GetList<T>(this GameObject inst, string name) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetList<T>(inst, name);
    }

    public static List<T> GetList<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetList<T>(inst);
    }

    public static bool Has<T>(this GameObject inst) where T : Component {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.Has<T>(inst);
    }

    public static void Show(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.Show(inst);
    }

    public static void Hide(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.Hide(inst);
    }

    public static void ShowObjectDelayed(this GameObject inst, float delay) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.ShowObjectDelayed(inst, delay);
    }

    public static void HideObjectDelayed(this GameObject inst, float delay) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.HideObjectDelayed(inst, delay);
    }

    public static bool IsReady(this UnityEngine.Object inst) {
        return inst != null ? true : false;
    }

    public static void StopSounds(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.StopSounds(inst);
    }

    public static void PauseSounds(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PauseSounds(inst);
    }

    public static void PlaySounds(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlaySounds(inst);
    }

    public static void PlayAnimations(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimations(inst);
    }

    public static void StopAnimations(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.StopAnimations(inst);
    }

    public static void PlayAnimation(this GameObject inst, string name, float speed = 1f) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimation(inst, name, speed);
    }

    public static void PlayAnimationBlend(this GameObject inst, string name, float speed = 1f, float targetWeight = .9f, float fadeLength = .5f) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimationBlend(inst, name, speed, targetWeight, fadeLength);
    }

    public static void PlayAnimationCrossFade(this GameObject inst, string name, float speed = 1f, float fadeLength = .5f) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimationCrossFade(inst, name, speed, fadeLength);
    }

    public static void StepAnimationFrame(
        this GameObject inst, string name, float time,
        bool normalizedTime = true, bool stopPlaying = true) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.StepAnimationFrame(inst, name, time, normalizedTime, stopPlaying);
    }

    public static void StopAnimation(this GameObject inst, string name) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.StopAnimation(inst, name);
    }

    public static List<AnimationState> GetAnimationsList(this GameObject inst) {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetAnimationsList(inst);
    }

    public static AnimationState GetAnimationsRandom(this GameObject inst) {

        if (inst == null) {
            return null;
        }

        return GameObjectHelper.GetAnimationsRandom(inst);
    }

    public static void PlayAnimationBlendRandom(this GameObject inst,
        float speed = 1, float targetWeight = 0.5f, float fadeLength = 0.5f) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimationBlendRandom(inst, speed, targetWeight, fadeLength);
    }

    public static void PlayAnimationCrossFadeRandom(this GameObject inst,
        float speed = 1, float fadeLength = 0.5f) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayAnimationCrossFadeRandom(inst, speed, fadeLength);
    }

    //---------------------------------------------------------------
    // RENDERERS

    public static bool IsRenderersVisible(this GameObject inst) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.IsRenderersVisible(inst);
    }

    public static bool IsRenderersVisibleByCamera(this GameObject inst) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.IsRenderersVisibleByCamera(inst);
    }

    public static bool IsRenderersVisibleByCamera(this GameObject inst, Camera cam) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.IsRenderersVisibleByCamera(inst, cam);
    }

    public static void ShowRenderers(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.ShowRenderers(inst);
    }

    public static void HideRenderers(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.HideRenderers(inst);
    }

    public static void ShowChildren(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.ShowChildren(inst);
    }

    public static void ShowChildren(this GameObject inst, bool applyGameObjectInactive) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.ShowChildren(inst, applyGameObjectInactive);
    }

    public static void HideChildren(this GameObject inst) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.HideChildren(inst);
    }

    public static void HideChildren(this GameObject inst, bool applyGameObjectInactive) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.HideChildren(inst, applyGameObjectInactive);
    }

    public static void HitObject(this GameObject inst, Vector3 pos) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.HitObject(inst, pos);
    }

    //---------------------------------------------------------------
    // AUDIO

    public static bool IsAudioSourcePlaying(this GameObject inst) {

        if (inst == null) {
            return false;
        }

        return GameObjectHelper.IsAudioSourcePlaying(inst);
    }

    //---------------------------------------------------------------
    // PARTICLE SYSTEMS

    public static void SetParticleSystemStartColor(
        this GameObject inst, Color startColor, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetParticleSystemStartColor(inst, startColor, includeChildren);
    }

    public static void PlayParticleSystem(
        this GameObject inst, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.PlayParticleSystem(inst, includeChildren);
    }

    public static void StopParticleSystem(
        this GameObject inst, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.StopParticleSystem(inst, includeChildren);
    }

    public static void SetParticleSystemEmissionRate(
        this GameObject inst, float emissionRate, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetParticleSystemEmissionRate(inst, emissionRate, includeChildren);
    }

    public static void SetParticleSystemEmissionRateNormalized(
        this GameObject inst, float emissionRate, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetParticleSystemEmissionRateNormalized(inst, emissionRate, includeChildren);
    }

    public static void SetParticleSystemEmissionRateNormalizedFlipped(
        this GameObject inst, float emissionRate, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetParticleSystemEmissionRateNormalizedFlipped(inst, emissionRate, includeChildren);
    }

    public static void SetParticleSystemEmission(
        this GameObject inst, bool emissionEnabled, bool includeChildren) {

        if (inst == null) {
            return;
        }

        GameObjectHelper.SetParticleSystemEmission(inst, emissionEnabled, includeChildren);
    }

    //---------------------------------------------------------------
    // AUDIO

    public static void DestroyNow(this GameObject inst) {
        GameObjectHelper.DestroyNow(inst);
    }

    public static void DestroyDelayed(this GameObject inst, float delay) {
        GameObjectHelper.DestroyDelayed(inst, delay);
    }

    public static void DestroyChildren(this GameObject inst) {
        GameObjectHelper.DestroyChildren(inst);
    }

    public static void DestroyChildren(this GameObject inst, bool pooled) {
        GameObjectHelper.DestroyChildren(inst, pooled);
    }

    public static void ChangeLayersRecursively(this GameObject inst, string name) {

        if (inst == null) {
            return;
        }

        foreach (Transform child in inst.transform) {

            child.gameObject.layer = LayerMask.NameToLayer(name);
            ChangeLayersRecursively(child.gameObject, name);
        }
    }

    public static void ScaleTweenObjectAbsolute(this GameObject go, float absoluteValue) {

        GameObjectHelper.ScaleTweenObjectAbsolute(go, absoluteValue);
    }

    public static void RotateTweenObjectAbsolute(this GameObject go, float absoluteValue) {

        GameObjectHelper.RotateTweenObjectAbsolute(go, absoluteValue);
    }

    public static void ScaleObject(this GameObject go, float delta) {

        GameObjectHelper.ScaleObject(go, delta);
    }

    public static void ResetObject(this GameObject go) {

        GameObjectHelper.ResetObject(go);
    }

    public static void ResetObject(this GameObject go, bool includeChildren) {

        GameObjectHelper.ResetObject(go, includeChildren);
    }

    public static void ResetScale(this GameObject go, float valueTo) {

        GameObjectHelper.ResetScale(go, valueTo);
    }

    public static void ResetScale(this GameObject go, float valueTo, bool includeChildren) {

        GameObjectHelper.ResetScale(go, valueTo, includeChildren);
    }

    public static void ResetLocalRotation(this GameObject go) {

        GameObjectHelper.ResetLocalRotation(go);
    }

    public static void ResetRotation(this GameObject go) {

        GameObjectHelper.ResetRotation(go);
    }

    public static void ResetRotation(this GameObject go, bool includeChildren) {

        GameObjectHelper.ResetRotation(go, includeChildren);
    }

    public static void ResetLocalPosition(this GameObject go) {

        GameObjectHelper.ResetLocalPosition(go);
    }

    public static void ResetPosition(this GameObject go) {

        GameObjectHelper.ResetPosition(go);
    }

    public static void ResetPosition(this GameObject go, bool includeChildren) {

        GameObjectHelper.ResetPosition(go, includeChildren);
    }

    public static void RotateObjectX(this GameObject go, float val) {

        GameObjectHelper.RotateObjectX(go, val);
    }

    public static void RotateObjectY(this GameObject go, float val) {

        GameObjectHelper.RotateObjectY(go, val);
    }

    public static void RotateObjectZ(this GameObject go, float val) {

        GameObjectHelper.RotateObjectZ(go, val);
    }

    public static void RotateObject(this GameObject go, Vector3 rotateBy) {

        GameObjectHelper.RotateObject(go, rotateBy);
    }

    //

    public static void TrackObject(this GameObject go, GameObject target) {

        GameObjectHelper.TrackObject(go, target);
    }

    //

    public static Material GetMaterial(this GameObject go, string name) {

        return GameObjectHelper.GetMaterial(go, name);
    }

    public static List<Material> GetMaterials(this GameObject go, string name) {

        return GameObjectHelper.GetMaterials(go, name);
    }

    //

    public static T GetMaterialValue<T>(
        this GameObject go, string key, string materialNameFind = "*") {

        return GameObjectHelper.GetMaterialValue<T>(go, key, materialNameFind);
    }

    public static void SetMaterialValue<T>(
        this GameObject go, string key, T val, string materialNameFind = "*") {

        GameObjectHelper.SetMaterialValue<T>(go, key, val, materialNameFind);
    }

    public static void SetMaterialSwap(
        this GameObject inst, string nameFind, string materialResourcesPath) {

        GameObjectHelper.SetMaterialSwap(inst, nameFind, materialResourcesPath);
    }

    public static void SetMaterialColor(this GameObject go, string name, Color color) {

        GameObjectHelper.SetMaterialColor(go, name, color);
    }

    public static void SetMaterialColorStandard(
        this GameObject go, string name, Color color, bool all = true) {

        GameObjectHelper.SetMaterialColorStandard(go, name, color, all);
    }

    //

    public static bool IsPrefab(this GameObject inst) {

        return GameObjectHelper.IsPrefab(inst);
    }

    public static bool IsPrefabGhost(this Transform inst) {

        return GameObjectHelper.IsPrefabGhost(inst);
    }

    public static GameObject CreateGameObject(
        this GameObject go,
        Vector3 pos,
        Quaternion rotate,
        bool pooled) {

        return GameObjectHelper.CreateGameObject(go, pos, rotate, pooled);
    }

    //

    public static void DestroyGameObject(this GameObject go, float delay = 0f, bool pooled = true) {

        GameObjectHelper.DestroyGameObject(go, delay, pooled);
    }

    public static GameObject LoadFromResources(this GameObject go, string path) {

        return GameObjectHelper.LoadFromResources(path);
    }

    public static GameObject LoadFromBundle(this GameObject go, string path) {

        return GameObjectHelper.LoadFromBundle(path);
    }

    // RIGIDBODIES

    public static void FreezeRigidBodies(this GameObject go) {

        GameObjectHelper.FreezeRigidbodies(go);
    }

    public static void UnFreezeRigidBodies(this GameObject go) {

        GameObjectHelper.UnFreezeRigidbodies(go);
    }

    public static Rigidbody GetRigidbody(this GameObject go) {

        return GameObjectHelper.GetRigidbody(go);
    }

    public static void ResetRigidBodiesVelocity(this GameObject go) {

        GameObjectHelper.ResetRigidBodiesVelocity(go);
    }

    // ASPECT RATIO

    public static void ResizePreservingAspectToScreen(
        this GameObject go, float desiredMaxWidth, float desiredMaxHeight) {

        GameObjectHelper.ResizePreservingAspectToScreen(
            go, desiredMaxWidth, desiredMaxHeight);
    }

    // POSITION RELATIVE

    public static float Distance(this GameObject inst, GameObject to) {

        return GameObjectHelper.Distance(inst, to);
    }

    // CUSTOM

    /*
public static void FadeInObject(this GameObject inst) {
    if (inst != null) {
        //Debug.Log("FadeInObject:" + inst.name);
        iTween.FadeTo(inst, 1f, 1f);
        //inst.ShowObjectDelayed(1f);
    }
}

public static void FadeInObject(this GameObject inst, float time) {
    if (inst != null) {
        //Debug.Log("FadeInObject:" + inst.name);
        iTween.FadeTo(inst, 1f, time);
        //inst.ShowObjectDelayed(time);
    }
}

public static void FadeOutObject(this GameObject inst) {
    if (inst != null) {
        //Debug.Log("FadeOutObject:" + inst.name);

        iTween.FadeTo(inst, 0f, 1f);//(go, iTween.Hash("alpha", 0f, "delay", 0f, "time", 1f));
        //inst.HideObjectDelayed(1f);
    }
}

public static void FadeOutObject(this GameObject inst, bool hide) {
    if (inst != null) {
        //Debug.Log("FadeOutObject:" + inst.name);

        iTween.FadeTo(inst, 0f, 1f);//(go, iTween.Hash("alpha", 0f, "delay", 0f, "time", 1f));

        if (hide) {
            inst.HideObjectDelayed(1f);
        }
    }
}

public static void FadeOutObject(this GameObject inst, float time) {
    if (inst != null) {
        //Debug.Log("FadeOutObject:" + inst.name);
        iTween.FadeTo(inst, 0f, time);//(go, iTween.Hash("alpha", 0f, "delay", 0f, "time", 1f));
        //inst.HideObjectDelayed(time);
    }
}

public static void FadeInObject(this GameObject inst, float time, float delay, float alpha) {
    if (inst != null) {
        //inst.ShowObject();
        iTween.FadeTo(inst, iTween.Hash("alpha", alpha, "time", time, "delay", delay));
    }
}

public static void FadeOutObject(this GameObject inst, float time, float delay, float alpha) {
    if (inst != null) {
        iTween.FadeTo(inst, iTween.Hash("alpha", alpha, "time", time, "delay", delay));
        //inst.ShowObjectDelayed(delay);
    }
}

//iTween.FadeTo(go, iTween.Hash("alpha",alpha,"time",time, "delay", delay));

public static void FadeOutObjectNow(this GameObject inst) {
    if (inst != null) {
        //Debug.Log("FadeOutObjectNow:" + inst.name);
        iTween.FadeTo(inst, 0f, 0f);
        //inst.HideObjectDelayed(.1f);
    }
}

public static void FadeOutObjectNow(this GameObject inst, bool hide) {
    if (inst != null) {
        //Debug.Log("FadeOutObjectNow:" + inst.name);
        iTween.FadeTo(inst, 0f, 0f);
        if (hide) {
            inst.HideObjectDelayed(.1f);
        }
    }
}

// MOVE

public static void MoveObject(this GameObject inst, float time, float delay, float alpha) {
    if (inst != null) {
        iTween.MoveTo(inst, iTween.Hash("alpha", alpha, "time", time, "delay", delay));
    }
}
*/
}