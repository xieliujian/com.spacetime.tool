using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace ST.Tool
{
    /// <summary>
    /// 编辑器协程系统：在 <see cref="EditorApplication.update"/> 驱动下，
    /// 模拟 Unity 协程的嵌套执行与生命周期管理。
    /// </summary>
    public static class EditorCoroutine
    {
        /// <summary>
        /// 单个编辑器协程包装器，使用栈结构支持嵌套 <see cref="IEnumerator"/> 执行。
        /// </summary>
        private class Coroutine : IEnumerator
        {
            /// <summary>
            /// 协程执行栈，支持嵌套子协程的压入与弹出。
            /// </summary>
            private Stack<IEnumerator> executionStack;

            /// <summary>
            /// 初始化协程并将根迭代器入栈。
            /// </summary>
            /// <param name="iterator">要驱动的根迭代器。</param>
            public Coroutine(IEnumerator iterator)
            {
                executionStack = new Stack<IEnumerator>();
                executionStack.Push(iterator);
            }

            /// <summary>
            /// 推进协程执行；若当前迭代器产出子迭代器则压栈，子协程完成后自动出栈。
            /// </summary>
            /// <returns>若协程仍在运行则返回 <c>true</c>，全部完成返回 <c>false</c>。</returns>
            public bool MoveNext()
            {
                IEnumerator i = this.executionStack.Peek();

                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        executionStack.Push((IEnumerator)result);
                    }

                    return true;
                }
                else
                {
                    if (executionStack.Count > 1)
                    {
                        executionStack.Pop();
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 不支持重置操作，始终抛出异常。
            /// </summary>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Reset()
            {
                throw new System.NotSupportedException("This Operation Is Not Supported.");
            }

            /// <summary>
            /// 返回栈顶迭代器的当前值。
            /// </summary>
            public object Current
            {
                get { return executionStack.Peek().Current; }
            }

            /// <summary>
            /// 判断指定迭代器是否在执行栈中。
            /// </summary>
            /// <param name="iterator">要查找的迭代器。</param>
            /// <returns>找到返回 <c>true</c>，否则返回 <c>false</c>。</returns>
            public bool Find(IEnumerator iterator)
            {
                return executionStack.Contains(iterator);
            }
        }

        /// <summary>
        /// 当前运行中的编辑器协程列表。
        /// </summary>
        private static List<Coroutine> editorCoroutineList;

        /// <summary>
        /// 本帧待加入的迭代器缓冲区，避免在 Update 遍历时直接修改列表。
        /// </summary>
        private static List<IEnumerator> buffer;

        /// <summary>
        /// 启动一个编辑器协程；首次启动时注册 <see cref="EditorApplication.update"/> 驱动。
        /// </summary>
        /// <param name="iterator">要驱动的迭代器。</param>
        /// <returns>传入的迭代器本身，可用于后续查找。</returns>
        public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
        {
            if (editorCoroutineList == null)
            {
                // test
                editorCoroutineList = new List<Coroutine>();
            }

            if (buffer == null)
            {
                buffer = new List<IEnumerator>();
            }

            if (editorCoroutineList.Count == 0)
            {
                EditorApplication.update += Update;
            }

            // add iterator to buffer first
            buffer.Add(iterator);

            return iterator;
        }

        /// <summary>
        /// 检查指定迭代器是否已在运行列表中，防止重复添加。
        /// </summary>
        /// <param name="iterator">要查找的迭代器。</param>
        /// <returns>已存在返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        private static bool Find(IEnumerator iterator)
        {
            // If this iterator is already added
            // Then ignore it this time
            foreach (Coroutine editorCoroutine in editorCoroutineList)
            {
                if (editorCoroutine.Find(iterator))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 每帧驱动所有协程推进，处理缓冲区中待启动的协程，
        /// 并在全部完成后取消 <see cref="EditorApplication.update"/> 注册。
        /// </summary>
        private static void Update()
        {
            // EditorCoroutine execution may append new iterators to buffer
            // Therefore we should run EditorCoroutine first
            editorCoroutineList.RemoveAll
            (
                coroutine => { return coroutine.MoveNext() == false; }
            );

            // If we have iterators in buffer
            if (buffer.Count > 0)
            {
                foreach (IEnumerator iterator in buffer)
                {
                    // If this iterators not exists
                    if (!Find(iterator))
                    {
                        // Added this as new EditorCoroutine
                        editorCoroutineList.Add(new Coroutine(iterator));
                    }
                }

                // Clear buffer
                buffer.Clear();
            }

            // If we have no running EditorCoroutine
            // Stop calling update anymore
            if (editorCoroutineList.Count == 0)
            {
                EditorApplication.update -= Update;
            }
        }
    }
}
