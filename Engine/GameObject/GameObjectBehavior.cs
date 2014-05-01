using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameObjectBehavior : MonoBehaviour {

    // Optimized base game object/mono behaviour for cached calls

    private GameObject _gameObject;
    public new GameObject gameObject {
        get {
            if (!_gameObject) {
                if(base.gameObject != null)
                    _gameObject = base.gameObject;
            }
            return _gameObject;
        }
    }
    
    private Transform _transform;
    public new Transform transform {
        get {
            if (!_transform) {
                if(base.transform != null)
                    _transform = base.transform;
            }
            return _transform;
        }
    }
        
    private Renderer _renderer;
    public new Renderer renderer {
        get {
            if (!_renderer) {
                if(base.renderer != null)
                    _renderer = base.renderer;
            }
            return _renderer;
        }
    }
        
    private SpriteRenderer _rendererSprite;
    public SpriteRenderer rendererSprite {
        get {
            if (!_rendererSprite) {
                if(base.renderer != null)
                    _rendererSprite = base.renderer as SpriteRenderer;
            }
            return _rendererSprite;
        }
    }

    private SkinnedMeshRenderer _rendererSkinnedMesh;
    public SkinnedMeshRenderer rendererSkinnedMesh {
        get {
            if (!_rendererSkinnedMesh) {
                if(base.renderer != null)
                    _rendererSkinnedMesh = base.renderer as SkinnedMeshRenderer;
            }
            return _rendererSkinnedMesh;
        }
    }
        
    private MeshRenderer _rendererMesh;
    public MeshRenderer rendererMesh {
        get {
            if (!_rendererMesh) {
                if(base.renderer != null)
                    _rendererMesh = base.renderer as MeshRenderer;
            }
            return _rendererMesh;
        }
    }
    
    private TrailRenderer _rendererTrail;
    public TrailRenderer rendererTrail {
        get {
            if (!_rendererTrail) {
                if(base.renderer != null)
                    _rendererTrail = base.renderer as TrailRenderer;
            }
            return _rendererTrail;
        }
    }

    private LineRenderer _rendererLine;
    public LineRenderer rendererLine {
        get {
            if (!_rendererLine) {
                if(base.renderer != null)
                    _rendererLine = base.renderer as LineRenderer;
            }
            return _rendererLine;
        }
    }
    
    private ParticleRenderer _rendererParticle;
    public ParticleRenderer rendererParticle {
        get {
            if (!_rendererParticle) {
                if(base.renderer != null)
                    _rendererParticle = base.renderer as ParticleRenderer;
            }
            return _rendererParticle;
        }
    }
    
    private ParticleSystemRenderer _rendererParticleSystem;
    public ParticleSystemRenderer rendererParticleSystem {
        get {
            if (!_rendererParticleSystem) {
                if(base.renderer != null)
                    _rendererParticleSystem = base.renderer as ParticleSystemRenderer;
            }
            return _rendererParticleSystem;
        }
    }



}