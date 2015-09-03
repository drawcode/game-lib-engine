
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
                _gameObject = base.gameObject;
            }
            return _gameObject;
        }
    }
    
    private Transform _transform;

    public new Transform transform {
        get {
            if (!_transform) {
                _transform = base.transform;
            }
            return _transform;
        }
    }
    
    private Rigidbody _rigidbody;
    
    public new Rigidbody rigidbody {
        get {
            if (!_rigidbody) {
                _rigidbody = base.GetComponent<Rigidbody>();
            }
            return _rigidbody;
        }
    }
        
    private Renderer _renderer;

    public new Renderer renderer {
        get {
            if (!_renderer) {
                _renderer = base.GetComponent<Renderer>();
            }
            return _renderer;
        }
    }
    
    private AudioSource _audio;

    public new AudioSource audio {
        get {
            if (!_audio) {
                _audio = base.GetComponent<AudioSource>();
            }
            return _audio;
        }
    }
        
    private Camera _camera;

    public new Camera camera {
        get {
            if (!_camera) {
                _camera = base.GetComponent<Camera>();
            }
            return _camera;
        }
    }
    
    private Collider _collider;
    
    public new Collider collider {
        get {
            if (!_collider) {
                _collider = base.GetComponent<Collider>();
            }
            return _collider;
        }
    }
        
    private SpriteRenderer _rendererSprite;

    public SpriteRenderer rendererSprite {
        get {
            if (!_rendererSprite) {
                _rendererSprite = base.GetComponent<SpriteRenderer>();
            }
            return _rendererSprite;
        }
    }

    private SkinnedMeshRenderer _rendererSkinnedMesh;

    public SkinnedMeshRenderer rendererSkinnedMesh {
        get {
            if (!_rendererSkinnedMesh) {
                _rendererSkinnedMesh = base.GetComponent<SkinnedMeshRenderer>();
            }
            return _rendererSkinnedMesh;
        }
    }
        
    private MeshRenderer _rendererMesh;

    public MeshRenderer rendererMesh {
        get {
            if (!_rendererMesh) {
                _rendererMesh = base.GetComponent<MeshRenderer>();
            }
            return _rendererMesh;
        }
    }
    
    private TrailRenderer _rendererTrail;

    public TrailRenderer rendererTrail {
        get {
            if (!_rendererTrail) {
                _rendererTrail = base.GetComponent<TrailRenderer>();
            }
            return _rendererTrail;
        }
    }

    private LineRenderer _rendererLine;

    public LineRenderer rendererLine {
        get {
            if (!_rendererLine) {
                _rendererLine = base.GetComponent<LineRenderer>();
            }
            return _rendererLine;
        }
    }
    
    private ParticleRenderer _rendererParticle;

    public ParticleRenderer rendererParticle {
        get {
            if (!_rendererParticle) {
                _rendererParticle = base.GetComponent<ParticleRenderer>();
            }
            return _rendererParticle;
        }
    }
    
    private ParticleSystem _particleSystem;
    
    public new ParticleSystem particleSystem {
        get {
            if (!_particleSystem) {
                _particleSystem = base.GetComponent<ParticleSystem>();
            }
            return _particleSystem;
        }
    }
    
    private ParticleSystemRenderer _rendererParticleSystem;

    public ParticleSystemRenderer rendererParticleSystem {
        get {
            if (!_rendererParticleSystem) {
                _rendererParticleSystem = base.GetComponent<ParticleSystemRenderer>();
            }
            return _rendererParticleSystem;
        }
    }
    
    private Light _light;
    
    public new Light light {
        get {
            if (!_light) {
                _light = base.GetComponent<Light>();
            }
            return _light;
        }
    }

    public void RemoveMe() {
        GameObjectHelper.DestroyGameObject(gameObject);
    }



}