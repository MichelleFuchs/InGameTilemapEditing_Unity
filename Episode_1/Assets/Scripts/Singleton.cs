using UnityEngine;

//  Code from:
// https://www.youtube.com/watch?v=ErJgQY5smnw&t=3s

public abstract class Singleton<T> : MonoBehaviour where T : Component {
    private static T instance;

    protected static bool DontDestroy = true;

    private static bool m_applicationIsQuitting = false;

    public static T GetInstance () {
        if (m_applicationIsQuitting) { return null; }

        if (instance == null) {
            instance = FindObjectOfType<T> ();
            if (instance == null) {
                GameObject obj = new GameObject ();
                obj.name = typeof (T).Name;
                instance = obj.AddComponent<T> ();
            }
        }
        return instance;
    }

    /* IMPORTANT!!! To use Awake in a derived class you need to do it this way
     * protected override void Awake()
     * {
     *     base.Awake();
     *     //Your code goes here
     * }
     * */

    protected virtual void Awake () {
        if (instance == null) {
            instance = this as T;
            if (DontDestroy) DontDestroyOnLoad (gameObject);
        } else if (instance != this as T) {
            Destroy (gameObject);
        } else if (DontDestroy) { DontDestroyOnLoad (gameObject); }
    }

    private void OnApplicationQuit () {
        m_applicationIsQuitting = true;
    }
}