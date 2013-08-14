using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Utility;
using UnityEngine;

public static class GameObjectExtensions {

    public static void SetLayerRecursively(this GameObject inst, int layer) {
        inst.layer = layer;
        foreach (Transform child in inst.transform)
            child.gameObject.SetLayerRecursively(layer);
    }

    /// <summary>
    /// Adds all the components found on a resource prefab.
    /// </summary>
    /// <param name='inst'>
    /// Instance of game object to add the components to
    /// </param>
    /// <param name='path'>
    /// Path of prefab relative to ANY resource folder in the assets directory
    /// </param>
    ///
    public static void AddComponentsFromResource(this GameObject inst, string path) {
        var go = Resources.Load(path) as GameObject;

        foreach (var src in go.GetComponents<Component>()) {
            var dst = inst.AddComponent(src.GetType()) as Behaviour;
            dst.enabled = false;
            ComponentUtil.Copy(dst, src);
            dst.enabled = true;
        }
    }

    /// <summary>
    /// Adds a component of the specific type found on a resource prefab.
    /// </summary>
    /// <returns>
    /// The newly added component.
    /// </returns>
    /// <param name='inst'>
    /// Instance of game object to add the component to
    /// </param>
    /// <param name='path'>
    /// Path of prefab relative to ANY resource folder in the assets directory
    /// </param>
    /// <typeparam name='T'>
    /// The type of component to find on the prefab and add.
    /// </typeparam>
    /// <exception cref='ArgumentException'>
    /// Is thrown when the path is invalid.
    /// </exception>
    ///
    public static T AddComponentFromResource<T>(this GameObject inst, string path)
        where T : Component {
        var go = Resources.Load(path) as GameObject;
        if (go == null)
            throw new ArgumentException("Invalid component path", "path");

        var src = go.GetComponent<T>();
        var dst = inst.AddComponent<T>();

        ComponentUtil.Copy(dst, src);

        return dst;
    }

    /// <summary>
    /// Gets a component from a game object (supports interfaces)
    /// </summary>
    /// <returns>
    /// The component found in the game object
    /// </returns>
    /// <param name='inst'>
    /// Instance of game object to add the component to
    /// </param>
    /// <typeparam name='T'>
    /// The type of component, or interface, to find
    /// </typeparam>
    ///
    public static T GetComponent<T>(this GameObject inst)
        where T : class {
        return inst.GetComponent(typeof(T)) as T;
    }

    public static GameObject FindTypeAboveObject<T>(this GameObject inst)
        where T : Component {
        if (inst == null) {
            return null;
        }

        return FindTypeAboveObjectRecursive<T>(inst);
    }

    public static GameObject FindTypeAboveObjectRecursive<T>(this GameObject inst)
        where T : Component {
        if (inst == null) {
            return null;
        }

        if (inst != null) {
			var instItem = inst.GetComponent<T>();
            if (instItem != null) {
                return inst;
            }

            if (inst.transform.parent != null) {
                return FindTypeAboveObjectRecursive<T>(inst.transform.parent.gameObject);
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

    public static void Show(this GameObject inst) {

        //LogUtil.Log("Show:" + inst.name);
        if (inst != null) {
            if (!inst.active) {
                inst.SetActiveRecursively(true);
            }
            if (inst.renderer != null) {
                if (!inst.renderer.enabled) {
                    inst.renderer.enabled = true;
                }
            }
        }
    }

    public static void Hide(this GameObject inst) {

        //LogUtil.Log("Hide:" + inst.name);
        if (inst != null) {
            if (inst.renderer != null) {
                inst.renderer.enabled = false;
            }
            inst.SetActiveRecursively(false);
        }
    }

    public static bool IsReady(this UnityEngine.Object inst) {
        return inst != null ? true : false;
    }

    public static void StopSounds(this GameObject inst) {
        if (inst == null)
            return;

        GameObjectHelper.StopSounds(inst);
    }

    public static void PauseSounds(this GameObject inst) {
        if (inst == null)
            return;

        GameObjectHelper.PauseSounds(inst);
    }

    public static void PlaySounds(this GameObject inst) {
        if (inst == null)
            return;

        GameObjectHelper.PlaySounds(inst);
    }

    public static bool IsRenderersVisible(this GameObject inst) {
        if (inst == null)
            return false;

        return GameObjectHelper.IsRenderersVisible(inst);
    }

    public static bool IsAudioSourcePlaying(this GameObject inst) {
        if (inst == null)
            return false;

        return GameObjectHelper.IsAudioSourcePlaying(inst);
    }

    public static void ShowRenderers(this GameObject inst) {
        if (inst == null)
            return;

        GameObjectHelper.ShowRenderers(inst);
    }

    public static void HideRenderers(this GameObject inst) {
        if (inst == null)
            return;

        GameObjectHelper.HideRenderers(inst);
    }


    public static void PlayParticleSystem(this GameObject inst, bool includeChildren) {
        if(inst == null)
                return;

        GameObjectHelper.PlayParticleSystem(inst, includeChildren);
    }
    
    public static void StopParticleSystem(this GameObject inst, bool includeChildren) {
        if(inst == null)
                return;

        GameObjectHelper.StopParticleSystem(inst, includeChildren);
    }       
     
    public static void SetParticleSystemEmissionRate(this GameObject inst, float emissionRate, bool includeChildren) {
        if(inst == null)
                return;

        GameObjectHelper.SetParticleSystemEmissionRate(inst, emissionRate, includeChildren);
    }
    
    public static void SetParticleSystemEmission(this GameObject inst, bool emissionEnabled, bool includeChildren) {
        if(inst == null)
                return;

        GameObjectHelper.SetParticleSystemEmission(inst, emissionEnabled, includeChildren);
    }
    
    public static void DestroyNow(this GameObject inst) {
        if(inst == null)
                return;

        GameObject.Destroy(inst);
    }
    
    public static void DestroyDelayed(this GameObject inst, float delay) {
        if(inst == null)
                return;

        GameObject.Destroy(inst, delay);
    }

    public static void DestroyChildren(this GameObject inst) {
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
            UnityEngine.Object.Destroy(t.gameObject);
        }

        transforms.Clear();
        transforms = null;
    }

    public static void ChangeLayersRecursively(this GameObject inst, string name) {
        if (inst == null)
            return;

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
	
	public static void ResetScale(this GameObject go, float valueTo) {
        GameObjectHelper.ResetScale(go, valueTo);   
	}
	
	public static void ResetRotation(this GameObject go) {
        GameObjectHelper.ResetRotation(go);   
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
	
}