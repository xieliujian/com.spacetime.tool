using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ST.Tool.Test
{
    /// <summary>
    /// <see cref="EditorCoroutine"/> 功能验证测试：通过菜单触发，在编辑器中依次输出 1~10 并等待子协程完成。
    /// </summary>
    public class EditorCoroutineTest
    {
        /// <summary>
        /// 启动编辑器协程测试，菜单位于 <c>SpaceTime/Test/Tool/EditorCoroutineTest</c>。
        /// </summary>
        [MenuItem("SpaceTime/Test/Tool/EditorCoroutineTest")]
        static void Test()
        {
            EditorCoroutine.StartEditorCoroutine(CountToTenCoroutine());
        }

        /// <summary>
        /// 从 1 数到 10，每次输出后等待下一帧及 1 秒，最后调用子协程打印完成信息。
        /// </summary>
        /// <returns>用于编辑器协程驱动的迭代器。</returns>
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
        /// 计数完成后的子协程：打印 DONE! 并结束。
        /// </summary>
        /// <returns>用于编辑器协程驱动的迭代器。</returns>
        static IEnumerator CountToTenCompletedSubroutine()
        {
            Debug.LogFormat("DONE!");

            yield break;
        }
    }
}
