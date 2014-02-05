using UnityEngine;
using System.Collections;

public class MaterialAlphaFader : MonoBehaviour {
    public float fadeSpeed = 1f;
    public float beginTintAlpha = 0.5f;
    public Color colorTo;
    
    public void Start() {

    }

    public void Update() {
        beginTintAlpha -= Time.deltaTime * fadeSpeed;
        renderer.material.SetColor(
            "_TintColor", 
            new Color(colorTo.r, colorTo.g, colorTo.b, beginTintAlpha));
    }
}

