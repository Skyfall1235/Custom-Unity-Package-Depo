using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomNamespace.GenericDatatypes
{

    /// <summary>
    /// A serializable version of the <see cref="HashSet{T}"/> for use with Unity's serialization system.
    /// Implements <see cref="ISerializationCallbackReceiver"/> to handle serialization and deserialization.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    [Serializable]
    public class SerializableHashSet<T> : ISerializationCallbackReceiver
    {
        private readonly HashSet<T> m_hashSet = new HashSet<T>();

        [SerializeField]
        private List<T> m_serializableItems = new List<T>();

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_serializableItems.Clear();
            m_serializableItems = m_hashSet.ToList();
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_hashSet.Clear();
            foreach (T item in m_serializableItems)
            {
                m_hashSet.Add(item);
            }
        }

        /// <summary>
        /// Adds an element to the hash set.
        /// </summary>
        /// <param name="item">The element to add to the hash set.</param>
        /// <returns>True if the element is added to the hash set; false if the element is already present.</returns>
        public bool Add(T item) => m_hashSet.Add(item);

        /// <summary>
        /// Determines whether the hash set contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the hash set.</param>
        /// <returns>True if the hash set contains the specified element; otherwise, false.</returns>
        public bool Contains(T item) => m_hashSet.Contains(item);
    }
}