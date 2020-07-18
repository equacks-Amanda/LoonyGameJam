using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Use for classes that should only ever be instantiated once. 
/// Used for :
///     Event Manager
///     Chat Manager
///     Scene Manager
/// We do not want events responed to twice or scenes loaded twice.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            return _instance;
        }
    }

    public virtual IEnumerator WaitForGameLoad(Action callback){
        yield return new WaitWhile(() => true);
        callback();
    }
}