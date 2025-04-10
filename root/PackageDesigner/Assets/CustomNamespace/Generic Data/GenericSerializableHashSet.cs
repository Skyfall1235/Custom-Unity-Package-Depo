using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SerializableHashSet<T> : ISerializationCallbackReceiver
{
    private HashSet<T> hashSet = new HashSet<T>();

    [SerializeField]
    private List<T> serializableItems = new List<T>();

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        this.serializableItems.Clear();
        this.serializableItems = new List<T>(this.hashSet);
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        hashSet.Clear();
        foreach (T item in this.serializableItems)
        {
            this.hashSet.Add(item);
        }
    }

    public bool Add(T item) => this.hashSet.Add(item);

    public bool Contains(T item) => this.hashSet.Contains(item);
}
