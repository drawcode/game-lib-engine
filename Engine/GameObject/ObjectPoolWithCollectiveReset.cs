using System;
using System.Collections;
using System.Collections.Generic;

//using UnityEngine;

/*
 * 

class SomeClass : MonoBehaviour
{
    private ObjectPool<List<Vector3>> m_poolOfListOfVector3 =
        new ObjectPool<List<Vector3>>(32,
        (list) => {
            list.Clear();
        },
        (list) => {
            list.Capacity = 1024;
        });

    void Update()
    {
        List<Vector3> listVector3 = m_poolOfListOfVector3.New();

        // do stuff

        m_poolOfListOfVector3.Store(listVector3);
    }
}



 */

public class ObjectPoolWithCollectiveReset<T> where T : class, new() {
    private List<T> m_objectList;
    private int m_nextAvailableIndex = 0;
    private Action<T> m_resetAction;
    private Action<T> m_onetimeInitAction;
    
    public ObjectPoolWithCollectiveReset(int initialBufferSize, Action<T>
                                         ResetAction = null, Action<T> OnetimeInitAction = null) {
        m_objectList = new List<T>(initialBufferSize);
        m_resetAction = ResetAction;
        m_onetimeInitAction = OnetimeInitAction;
    }
    
    public T New() {
        if (m_nextAvailableIndex < m_objectList.Count) {
            // an allocated object is already available; just reset it
            T t = m_objectList[m_nextAvailableIndex];
            m_nextAvailableIndex++;
            
            if (m_resetAction != null)
                m_resetAction(t);
            
            return t;
        }
        else {
            // no allocated object is available
            T t = new T();
            m_objectList.Add(t);
            m_nextAvailableIndex++;
            
            if (m_onetimeInitAction != null)
                m_onetimeInitAction(t);
            
            return t;
        }
    }
    
    public void ResetAll() {
        m_nextAvailableIndex = 0;
    }
}