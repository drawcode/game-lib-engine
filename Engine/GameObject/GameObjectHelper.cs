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

    public static void StopSounds(GameObject inst) {
        if (inst == null)
            return;

        if (inst.audio != null) {
            if (inst.audio.isPlaying) {
                inst.audio.Stop();
            }
        }

        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Stop();
        }
    }

    public static void PauseSounds(GameObject inst) {
        if (inst == null)
            return;

        if (inst.audio != null) {
            if (inst.audio.isPlaying) {
                inst.audio.Pause();
            }
        }

        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Pause();
        }
    }

    public static void PlaySounds(GameObject inst) {
        if (inst == null)
            return;

        if (inst.audio != null) {
            if (!inst.audio.isPlaying) {
                inst.audio.Play();
            }
        }

        foreach (AudioSource source in inst.GetComponentsInChildren<AudioSource>()) {
            source.Play();
        }
    }

    public static bool IsAudioSourcePlaying(GameObject inst) {
        if (inst == null)
            return false;

        if (inst.audio != null) {
            if (inst.audio.isPlaying) {
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

    public static bool IsRenderersVisible(GameObject inst) {
        if (inst == null)
            return false;

        if (inst.renderer != null) {
            if (inst.renderer.enabled) {
                return true;
            }
        }

        Renderer[] rendererComponents = inst.GetComponentsInChildren<Renderer>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents) {
            if (component.enabled) {
                return true;
            }
        }

        return false;
    }

    public static void ShowRenderers(GameObject inst) {
        if (inst == null)
            return;

        if (inst.renderer != null) {
            inst.renderer.enabled = true;
        }

        Renderer[] rendererComponents = inst.GetComponentsInChildren<Renderer>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents) {
            component.enabled = true;
        }
    }

    public static void HideRenderers(GameObject inst) {
        if (inst == null)
            return;

        if (inst.renderer != null) {
            inst.renderer.enabled = false;
        }

        Renderer[] rendererComponents = inst.GetComponentsInChildren<Renderer>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents) {
            component.enabled = false;
        }
    }

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
        Debug.Log("DUMP: go:" + go.name + "::::" + GameObjectHelper.DumpGo(go));
    }

    public static string DumpGo(GameObject go) {
        StringBuilder sb = new StringBuilder();
        sb.Append(go.name);
        DumpGameObject(go, sb, "", false);
        return sb.ToString();
    }

    private static void DumpGameObject(GameObject gameObject, StringBuilder sb, string indent, bool includeAllComponents) {
        bool rendererEnabled = false;
        if (gameObject.renderer != null) {
            rendererEnabled = gameObject.renderer.enabled;
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
            indent, gameObject.name,
            gameObject.activeSelf, rendererEnabled,
            markerId, hasLoadedObj,
            gameObject.transform.localScale.x,
            gameObject.transform.localScale.y,
            gameObject.transform.localScale.z,
            gameObject.transform.position.x,
            gameObject.transform.position.y,
            gameObject.transform.position.z));

        if (includeAllComponents) {
            foreach (Component component in gameObject.GetComponents<Component>()) {
                DumpComponent(component, sb, indent + "  ");
            }
        }

        foreach (Transform child in gameObject.transform) {
            DumpGameObject(child.gameObject, sb, indent + "  ", includeAllComponents);
        }
    }

    private static void DumpComponent(Component component, StringBuilder sb, string indent) {
        sb.Append(string.Format("{0}{1}", indent, (component == null ? "(null)" : component.GetType().Name)));
    }
        
    public static void ScaleTweenObjectAbsolute(GameObject go, float absoluteValue) {            
		if (go != null) {
	        float valueTo = (absoluteValue / defaultScale) + .005f;    
	        iTween.ScaleTo(go, Vector3.zero.WithX(valueTo).WithY(valueTo).WithZ(valueTo), .2f);
		}
    }
        
    public static void RotateTweenObjectAbsolute(GameObject go, float absoluteValue) {
        if(go != null) {
            float valueTo = absoluteValue * 360;
            iTween.RotateTo(go, Vector3.zero.WithY(valueTo), .2f);
        }
    }
        
    public static void ScaleObject(GameObject go, float delta) {	
		if(go != null) {        
			// change the scale of the target based on the pinch delta value
			float scaleTo = delta/defaultScale * pinchScaleFactor;
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
	}
	
	public static void ResetScale(GameObject go, float valueTo) {
		if(go != null) {        
			Vector3 currentObjectScale = go.transform.localScale;   
			currentObjectScale.x = valueTo;
			currentObjectScale.y = valueTo;
			currentObjectScale.z = valueTo;                 
			go.transform.localScale = currentObjectScale;
		}
	}
	
	public static void ResetRotation(GameObject go) {
		if(go != null) {        
			Quaternion objectRotation = Quaternion.identity;        
			go.transform.rotation = objectRotation; 
			go.transform.localRotation = objectRotation;
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
		if(go != null) {			
			// apply a rotation around the Z axis by rotationAngleDelta degrees on our target object
			go.transform.Rotate( rotateBy.x, rotateBy.y, rotateBy.z );
		}       
	}
	
	public static void SpinObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
	                
		if(go != null) {
		    if(go.rigidbody == null) {
		            go.AddComponent<Rigidbody>();
		            go.rigidbody.constraints = 
		                    RigidbodyConstraints.FreezePosition 
		                    | RigidbodyConstraints.FreezeRotationX 
		                    | RigidbodyConstraints.FreezeRotationZ;
		            go.rigidbody.useGravity = false;
		            go.rigidbody.angularDrag = .25f;
		    }
		    go.rigidbody.angularVelocity = (new Vector3(0,-delta.x,0));
		}
	}



	public static void ShowObject(GameObject inst) {
		if(inst == null)
			return;
		
		inst.Show();
	}	
	
	public static void HideObject(GameObject inst) {
		if(inst == null)
			return;
		
		inst.Hide();
	}

	public static void ResetAnimations(GameObject inst) {
		if(inst == null)
			return;

		if(inst.animation != null) {
			inst.animation.Stop();
			inst.animation.Rewind();
		}

		foreach(Animation source in inst.GetComponentsInChildren<Animation>()) {
			source.Stop();
			source.Rewind();
		}
	}

	public static void PlayAnimations(GameObject inst) {
		if(inst == null)
			return;

		if(inst.animation != null) {
			if(!inst.animation.isPlaying) {
				inst.animation.Play();
			}
		}
		
		foreach(Animation source in inst.GetComponentsInChildren<Animation>()) {
			source.Play();
		}
	}
	
	public static void StopAnimations(GameObject inst) {
		if(inst == null)
			return;		
		
		if(inst.animation != null) {
			if(inst.animation.isPlaying) {
				inst.animation.Stop();
			}
		}
		
		foreach(Animation source in inst.GetComponentsInChildren<Animation>()) {
			source.Stop();
		}
	}
	
	public static void PauseAnimations(GameObject inst) {
		if(inst == null)
			return;		
		
		if(inst.animation != null) {
			if(inst.animation.isPlaying) {
				inst.animation.Stop();
			}
		}
		
		foreach(Animation source in inst.GetComponentsInChildren<Animation>()) {
			source.Stop();
		}
	}

	public static void SetParticleSystemEmission(GameObject inst, bool emissionEnabled, bool includeChildren) {
		if(inst == null)
			return;

		ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
		if(particleSystemCurrent != null) {
			particleSystemCurrent.enableEmission = emissionEnabled;
		}
		
		if(!includeChildren) {
			return;	
		}
		
		ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
            particleSystem.enableEmission = emissionEnabled;
        }
	}
	
	public static void SetParticleSystemEmissionRate(GameObject inst, float emissionRate, bool includeChildren) {
		if(inst == null)
			return;

		ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
		if(particleSystemCurrent != null) {
			particleSystemCurrent.emissionRate = emissionRate;
		}
		
		if(!includeChildren) {
			return;	
		}
		
		ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
			particleSystem.emissionRate = emissionRate;
        }
	}
	
	public static void PlayParticleSystem(GameObject inst, bool includeChildren) {
		if(inst == null)
			return;

		ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
		if(particleSystemCurrent != null) {
			particleSystemCurrent.enableEmission = true;
			if(!particleSystemCurrent.isPlaying) {
				particleSystemCurrent.Play();//.enableEmission = emissionEnabled;
			}
		}
		
		if(!includeChildren) {
			return;	
		}
		
		ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
			particleSystem.enableEmission = true;
            if(!particleSystem.isPlaying) {
				particleSystem.Play();//.enableEmission = emissionEnabled;
			}
        }
	}
	
	public static void StopParticleSystem(GameObject inst, bool includeChildren) {
		if(inst == null)
			return;

		ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();
		if(particleSystemCurrent != null) {
			particleSystemCurrent.enableEmission = false;
			if(particleSystemCurrent.isPlaying) {
				//particleSystemCurrent.Stop();
			}
		}
		
		if(!includeChildren) {
			return;	
		}
		
		ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems) {
			particleSystem.enableEmission = false;
            if(particleSystem.isPlaying) {
				//particleSystem.Stop();
			}
        }
	}
	
}