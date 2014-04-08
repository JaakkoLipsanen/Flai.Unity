using UnityEngine;

namespace Flai
{
    public abstract class Singleton<T> : FlaiScript
        where T : FlaiScript
    {
        private static readonly object _lock = new object();
        private static T _instance;
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                /* TEST */
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit." + " Won't create again - returning null.");
                    return null;
                } 

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Singleton<T>.FindObjectOfType(typeof(T));
                        if (Singleton<T>.FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " + " - there should never be more than 1 singleton!" + " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (typeof (T).IsGenericType)
                        {
                            Debug.LogWarning("[Singleton] Creating a singleton of type " + typeof(T) + ". Warning: Type is generic!");    
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "[Singleton] " + typeof(T).Name;

                            Singleton<T>.DontDestroyOnLoad(singleton);
                            Debug.Log("[Singleton] An instance of " + typeof(T) + " is needed in the scene, so '" + singleton + "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        public bool IsPreservingInstance = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected override void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
    }
}
