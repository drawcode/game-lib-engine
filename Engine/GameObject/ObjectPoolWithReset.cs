using System;
using System.Collections;
using System.Collections.Generic;

//using UnityEngine;

public interface IResetable {
    void Reset();
}

public class ObjectPoolWithReset<T> where T : class, IResetable, new() {
    private Stack<T> m_objectStack;
    private Action<T> m_resetAction;
    private Action<T> m_onetimeInitAction;
    
    public ObjectPoolWithReset(int initialBufferSize, Action<T>
                               ResetAction = null, Action<T> OnetimeInitAction = null) {
        m_objectStack = new Stack<T>(initialBufferSize);
        m_resetAction = ResetAction;
        m_onetimeInitAction = OnetimeInitAction;
    }
    
    public T New() {
        if (m_objectStack.Count > 0) {
            T t = m_objectStack.Pop();
            
            t.Reset();
            
            if (m_resetAction != null)
                m_resetAction(t);
            
            return t;
        }
        else {
            T t = new T();
            
            if (m_onetimeInitAction != null)
                m_onetimeInitAction(t);
            
            return t;
        }
    }
    
    public void Store(T obj) {
        m_objectStack.Push(obj);
    }
}