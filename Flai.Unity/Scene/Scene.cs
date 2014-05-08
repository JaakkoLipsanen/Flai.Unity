
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Flai.Scene
{
    public static class Scene
    {
        #region Properties

        public static int CurrentSceneIndex
        {
            get { return Application.loadedLevel; }
        }

        public static string CurrentSceneName
        {
            get { return Application.loadedLevelName; }
        }

        public static SceneDescription CurrentSceneDescription
        {
            get { return SceneDescription.FromIndex(Scene.CurrentSceneIndex); }
        }

        #endregion

        #region Find

        public static T FindOfType<T>()
           where T : UnityObject
        {
            return UnityObject.FindObjectOfType<T>();
        }

        public static T[] FindAllOfType<T>()
           where T : UnityObject
        {
            return UnityObject.FindObjectsOfType<T>();
        }

        public static IEnumerable<T> FindAllOfType<T>(Predicate<T> predicate)
            where T : UnityObject
        {
            return UnityObject.FindObjectsOfType<T>().Where(obj => predicate(obj));
        }

        public static GameObject Find(string name)
        {
            return GameObject.Find(name);
        }

        public static GameObject FindWithTag(string tag)
        {
            return GameObject.FindWithTag(tag);
        }

        public static GameObject[] FindAllWithTag(string tag)
        {
            return GameObject.FindGameObjectsWithTag(tag);
        }

        #endregion

        #region Destroy

        public static void Destroy(ref GameObject gameObject)
        {
            gameObject.DestroyIfNotNull();
            gameObject = null;
        }

        public static void Destroy(GameObject gameObject)
        {
            gameObject.DestroyIfNotNull();
        }

        public static void DestroyGameObject<T>(ref T component)
            where T : Component
        {
            if (component == null)
            {
                component = null;
                return;
            }

            component.gameObject.DestroyIfNotNull();
            component = null;
        }

        public static void DestroyGameObject<T>(T component)
            where T : Component
        {
            if (component != null)
            {
                component.gameObject.DestroyIfNotNull();
            }
        }

        public static void DestroyImmediate(ref GameObject gameObject)
        {
            gameObject.DestroyImmediateIfNotNull();
            gameObject = null;
        }

        public static void DestroyImmediate(GameObject gameObject)
        {
            gameObject.DestroyImmediateIfNotNull();
        }

        #endregion

        #region Get Components

        public static T GetComponentInChildren<T>(Transform transform, bool searchRecursively)
           where T : Component
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var component = child.TryGet<T>();
                if (component != null)
                {
                    return component;
                }

                if (searchRecursively)
                {
                    var result = GetComponentInChildren<T>(child, true);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static List<T> GetComponentsInChildren<T>(Transform transform, bool searchRecursively = true)
            where T : Component
        {
            List<T> list = new List<T>();
            Scene.GetComponentsInChildren(transform, list, searchRecursively);
            return list;
        }

        public static ICollection<T> GetComponentsInChildren<T>(Transform transform, ICollection<T> collection, bool searchRecursively = true)
            where T : Component
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var component = child.TryGet<T>();
                if (component != null)
                {
                    collection.Add(component);
                }

                if (searchRecursively)
                {
                    GetComponentsInChildren(child, collection, true);
                }
            }

            return collection;
        }

        #endregion
    }
}
