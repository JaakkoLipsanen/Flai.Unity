using Flai.Diagnostics;
using UnityEngine;

namespace Flai
{
    public abstract class Singleton<T> : FlaiScript
        where T : FlaiScript
    {
        private static readonly object _lock = new object();
        private static T _instance;
        private static bool _applicationIsQuitting = false;

        protected virtual bool IsVisibleInInspector
        {
            get { return false; }
        }

        public static T Instance
        {
            get
            {
                /* TEST */
                /*  if (_applicationIsQuitting)
                  {
                      Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit." + " Won't create again - returning null.");
                      return null;
                  }  */

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Singleton<T>.FindObjectOfType(typeof(T));
                        if (Singleton<T>.FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            FlaiDebug.LogError("[Singleton] Something went really wrong " + " - there should never be more than 1 singleton!" + " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (typeof(T).IsGenericType)
                        {
                            FlaiDebug.LogWarning("[Singleton] Creating a Singleton<{0}>. Warning: type is generic!", typeof(T).Name);
                        }

                        if (_instance == null)
                        {
                            string singletonName = "~" + typeof(T).Name;
                            GameObject previous = Scene.Scene.Find(singletonName);
                            if (previous && previous.Has<T>())
                            {
                                _instance = previous.Get<T>();
                                FlaiDebug.LogWithTypeTag<T>("--WARNING/NOTIFICATION--: Check why this is happening");
                            }
                            else
                            {
                                GameObject singleton = new GameObject();
                                _instance = singleton.AddComponent<T>();
                                singleton.name = singletonName;
                                singleton.hideFlags = singleton.Get<Singleton<T>>().IsVisibleInInspector ? HideFlags.None : HideFlags.HideInHierarchy;

                                Singleton<T>.DontDestroyOnLoad(singleton);
                                FlaiDebug.Log("[Singleton] Singleton<{0}> created.", typeof(T).Name);
                            }
                        }
                        else
                        {
                            /* !!!!! */
                            // okay this might be useful, but meh. this happens everytime the scripts are recompiled and i dont really care
                            //   Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool Exists
        {
            get
            {
                if (_instance != null)
                {
                    return true;
                }

                if (Singleton<T>.FindObjectOfType(typeof(T)) != null)
                {
                    return true;
                }

                string singletonName = "~" + typeof(T).Name;
                GameObject previous = Scene.Scene.Find(singletonName);
                if (previous && previous.Has<T>())
                {
                    return true;
                }

                return false;
            }
        }

        public static T EnsureInstanceExists()
        {
            return Singleton<T>.Instance;
        }

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
