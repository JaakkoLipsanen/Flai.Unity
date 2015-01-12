using System.Collections;
using UnityEngine;

namespace Flai
{
    public class CoroutineRunner : Singleton<CoroutineRunner>
    {
        public static void StartCoroutine(IEnumerator enumerator)
        {
            ((MonoBehaviour)CoroutineRunner.Instance).StartCoroutine(enumerator);
        }
    }
}
