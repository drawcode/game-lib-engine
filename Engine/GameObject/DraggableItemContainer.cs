using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DraggableItemContainer : GameObjectBehavior {
    public GameObject dragColliderObject;
    public GameObject gameLevelItemObject;

    public virtual void Awake() {
    }

    public virtual void Start() {
        Init();
    }

    public virtual void Init() {
        if (dragColliderObject != null) {
            dragColliderObject.tag = "drag";
        }

        LoadData();
    }

    public void LoadData() {
        StartCoroutine(LoadDataCo());
    }

    private IEnumerator LoadDataCo() {
        yield break;
    }
}