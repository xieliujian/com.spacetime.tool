using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ST.Tool.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class EditorCoroutineTest
    {
        /// <summary>
        /// 
        /// </summary>
        [MenuItem("SpaceTime/Test/Tool/EditorCoroutineTest")]
        static void Test()
        {
            EditorCoroutine.StartEditorCoroutine(CountToTenCoroutine());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static IEnumerator CountToTenCoroutine()
        {
            for (var i = 1; i <= 10; ++i)
            {
                Debug.LogFormat("{0}", i);

                // yield until next EditorApplication.update
                yield return null;

                // yield until 1 second has passed
                yield return new WaitForSeconds(1);
            }

            // yield until a subroutine has completed
            yield return CountToTenCompletedSubroutine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static IEnumerator CountToTenCompletedSubroutine()
        {
            Debug.LogFormat("DONE!");

            yield break;
        }
    }
}

