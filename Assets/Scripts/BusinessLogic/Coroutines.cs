using System.Collections;
using UnityEngine;

public sealed class Coroutines : MonoBehaviour
{
    private static Coroutines instance
    {
        get
        {
            if (m_instance == null)
            {
                var go = new GameObject(name: "[COROUTINE MANAGER]");
                m_instance = go.AddComponent<Coroutines>();
                DontDestroyOnLoad(go);
            }
            return m_instance;
        }
    }
    private static Coroutines m_instance;

    public static Coroutine StartRoutine (IEnumerator enumerator)
    {
        return instance.StartCoroutine(enumerator);
    }
    public static void DestroyRoutine(Coroutine routine)
    {
        instance.StopCoroutine(routine);
    }
}
