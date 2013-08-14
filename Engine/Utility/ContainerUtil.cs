//  Copyright © 2011 Impossible Interactive LLC
//
using System;
using UnityEngine;

public class Container : MonoBehaviour {
    public object contained;

    private void Awake() {
        DontDestroyOnLoad(this);
    }
}

public class Destroyer : MonoBehaviour {
    public UnityEngine.Object target;

    public void OnDestroy() {
        UnityEngine.Object.Destroy(target);
    }
}

public static class Containers {

    public static void Store<T>(string name, T val) {
        foreach (var obj in ObjectUtil.FindObjects<GameObject>()) {
            if ((obj.name == name + " Container"
               || obj.name == name + " Container (Clone)")
               && obj.GetComponent<Container>() != null) {
                obj.GetComponent<Container>().contained = val;
                return;
            }
        }

        var go = new GameObject(name + " Container", typeof(Container));
        go.GetComponent<Container>().contained = val;
    }

    public static T Retrieve<T>(string name)
        where T : class {
        foreach (var obj in ObjectUtil.FindObjects<GameObject>()) {
            if ((obj.name == name + " Container"
               || obj.name == name + " Container (Clone)")
               && obj.GetComponent<Container>() != null) {
                return (T)obj.GetComponent<Container>().contained;
            }
        }

        return null;
    }

    public static void Mark(UnityEngine.Object target) {
        var destroyer = new GameObject(target.name + " Destroyer").AddComponent<Destroyer>();
        destroyer.target = target;
    }
}