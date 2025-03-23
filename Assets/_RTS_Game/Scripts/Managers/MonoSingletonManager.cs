using UnityEngine;

public abstract class MonoSingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance != null) return m_instance;

            m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this.GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}