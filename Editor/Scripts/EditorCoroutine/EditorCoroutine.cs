using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace ST.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public static class EditorCoroutine
    {
        /// <summary>
        /// 
        /// </summary>
        private class Coroutine : IEnumerator
        {
            /// <summary>
            /// 
            /// </summary>
            private Stack<IEnumerator> executionStack;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iterator"></param>
            public Coroutine(IEnumerator iterator)
            {
                executionStack = new Stack<IEnumerator>();
                executionStack.Push(iterator);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
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
            /// 
            /// </summary>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Reset()
            {
                throw new System.NotSupportedException("This Operation Is Not Supported.");
            }

            /// <summary>
            /// 
            /// </summary>
            public object Current
            {
                get { return executionStack.Peek().Current; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iterator"></param>
            /// <returns></returns>
            public bool Find(IEnumerator iterator)
            {
                return executionStack.Contains(iterator);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static List<Coroutine> editorCoroutineList;

        /// <summary>
        /// 
        /// </summary>
        private static List<IEnumerator> buffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
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
        /// 
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
