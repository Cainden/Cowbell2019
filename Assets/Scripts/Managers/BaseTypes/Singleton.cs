using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    /// <summary>
    /// Get an instance of the singleton object.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                // Does one exist in the scene?
                T[] allComponentInstances = FindObjectsOfType<T>();
                if (allComponentInstances != null)
                {
                    if (allComponentInstances.Length > 0)
                    {
                        m_instance = allComponentInstances[0];

                        // If there are multiple instances in the scene it should be fixed
                        // so there is only one instance. Otherwise, some members that have
                        // been set in the editor window may be tossed out because the wrong
                        // instance has been set in this Property.
                        if (allComponentInstances.Length > 1)
                        {
                            Debug.LogWarning("More that one instance of " + typeof(T).Name + ". Please fix this!!");
                        }
                    }
                }

                // Otherwise, lets create one
                if (m_instance == null)
                {
                    GameObject newObject = new GameObject(typeof(T).Name);
                    m_instance = newObject.AddComponent<T>();
                }
            }

            return m_instance;
        }
    }

    /// <summary>
    /// This check is still defensive, in case multiple singleton
    /// objects are added to the scene, or across multiple scenes.
    /// </summary>
    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = gameObject.GetComponent<T>();
        }
        else if (m_instance != gameObject.GetComponent<T>())
        {
            GameObject.Destroy(gameObject);
        }
    }
}
