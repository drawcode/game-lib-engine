using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtil : GameObjectBehavior {
    private static CoroutineUtil instance;

    private static CoroutineUtil Instance {
        get {
            if (instance == null) {
                instance = UnityObjectUtil.FindObject<CoroutineUtil>();
                if (instance == null) {
                    instance =
                        new GameObject("_CoroutineUtil", typeof(CoroutineUtil))
                            .GetComponent<CoroutineUtil>();
                }
            }
            return instance;
        }
    }
    /// <summary>
    /// Start the specified coroutine.
    /// </summary>
    /// <param name='coroutine'>
    /// Coroutine.
    /// </param>
    ///
    public static YieldInstruction Start(IEnumerator coroutine) {
        return Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Start the specified action delegate as a coroutine.
    /// </summary>
    /// <param name='action'>
    /// Action.
    /// </param>
    ///
    public static YieldInstruction Start(Action action) {
        return Instance.StartCoroutine(RunCo(action));
    }

    /// <summary>
    /// Start the specified action delegate as a coroutine using t.
    /// </summary>
    /// <param name='action'>
    /// Action.
    /// </param>
    /// <param name='t'>
    /// T.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    public static YieldInstruction Start<T>(Action<T> action, T t) {
        return Instance.StartCoroutine(RunCo(() => { action(t); }));
    }

    /// <summary>
    /// Start the specified action delegate as a coroutine using t and u.
    /// </summary>
    /// <param name='action'>
    /// Action.
    /// </param>
    /// <param name='t'>
    /// T.
    /// </param>
    /// <param name='u'>
    /// U.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    /// <typeparam name='U'>
    /// The 2nd type parameter.
    /// </typeparam>
    public static YieldInstruction Start<T, U>(Action<T, U> action, T t, U u) {
        return Instance.StartCoroutine(RunCo(() => { action(t, u); }));
    }

    /// <summary>
    /// Start the specified action delegate as a coroutine using t, u and v.
    /// </summary>
    /// <param name='action'>
    /// Action.
    /// </param>
    /// <param name='t'>
    /// T.
    /// </param>
    /// <param name='u'>
    /// U.
    /// </param>
    /// <param name='v'>
    /// V.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    /// <typeparam name='U'>
    /// The 2nd type parameter.
    /// </typeparam>
    /// <typeparam name='V'>
    /// The 3rd type parameter.
    /// </typeparam>
    public static YieldInstruction Start<T, U, V>(Action<T, U, V> action, T t, U u, V v) {
        return Instance.StartCoroutine(RunCo(() => { action(t, u, v); }));
    }

    /// <summary>
    /// Start the specified action delegate as a coroutine using t, u, v and w.
    /// </summary>
    /// <param name='action'>
    /// Action.
    /// </param>
    /// <param name='t'>
    /// T.
    /// </param>
    /// <param name='u'>
    /// U.
    /// </param>
    /// <param name='v'>
    /// V.
    /// </param>
    /// <param name='w'>
    /// W.
    /// </param>
    /// <typeparam name='T'>
    /// The 1st type parameter.
    /// </typeparam>
    /// <typeparam name='U'>
    /// The 2nd type parameter.
    /// </typeparam>
    /// <typeparam name='V'>
    /// The 3rd type parameter.
    /// </typeparam>
    /// <typeparam name='W'>
    /// The 4th type parameter.
    /// </typeparam>
    public static YieldInstruction Start<T, U, V, W>(Action<T, U, V, W> action, T t, U u, V v, W w) {
        return Instance.StartCoroutine(RunCo(() => { action(t, u, v, w); }));
    }

    //Driver for Start
    private static IEnumerator RunCo(Action action) {
        action();
        yield break;
    }

    public static YieldInstruction Wait(float seconds) {
        return new WaitForSeconds(seconds);
    }

    public static YieldInstruction WaitForFixedUpdate() {
        return new WaitForFixedUpdate();
    }

    public static YieldInstruction WaitForEndOfFrame() {
        return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Wait for all the specified yield instructions to complete using
    /// a wrapper coroutine.
    /// </summary>
    /// <param name='routines'>
    /// Routines.
    /// </param>
    public static YieldInstruction Wait(IList<object> yields) {
        return Instance.StartCoroutine(WaitCo(yields));
    }

    public static YieldInstruction Wait(params object[] yields) {
        return Instance.StartCoroutine(WaitCo(yields));
    }

    //Driver for Wait
    private static IEnumerator WaitCo(IEnumerable<object> yields) {
        foreach (var y in yields) {
            yield return y;
        }
    }
}