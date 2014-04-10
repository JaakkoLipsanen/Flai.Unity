
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Flai.Scene
{
    public static class Scene
    {
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
    }
}
