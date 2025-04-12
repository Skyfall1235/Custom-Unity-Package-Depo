using System;
using System.Collections;
using UnityEngine;

namespace CustomNamespace.GenericDatatypes
{
    /// <summary>
    /// Wraps a coroutine and allows retrieval of the result yielded by the coroutine.
    /// Requires the owner MonoBehaviour to start the coroutine.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour that will start the coroutine.</typeparam>
    [Serializable]
    public class CoroutineWithData<T> where T : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Coroutine"/> instance that is running.
        /// </summary>
        public Coroutine coroutine { get; private set; }

        /// <summary>
        /// The current result yielded by the coroutine. This value is updated with each <c>yield return</c> statement.
        /// </summary>
        public object Result { get => m_result; }

        private object m_result;
        private readonly IEnumerator m_target;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineWithData{T}"/> class and starts the coroutine.
        /// </summary>
        /// <param name="owner">The <see cref="MonoBehaviour"/> that will start the coroutine.</param>
        /// <param name="target">The <see cref="IEnumerator"/> representing the coroutine.</param>
        public CoroutineWithData(T owner, IEnumerator target)
        {
            m_target = target;
            coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (m_target.MoveNext())
            {
                m_result = m_target.Current;
                yield return Result;
            }
        }
    }
}