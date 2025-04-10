
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public static class SerializedPropertyExtensions
{
    private delegate FieldInfo GetFieldInfoAndStaticTypeFromProperty(SerializedProperty aProperty, out Type aType);
    private static GetFieldInfoAndStaticTypeFromProperty m_GetFieldInfoAndStaticTypeFromProperty;

    public static FieldInfo GetFieldInfoAndStaticType(this SerializedProperty prop, out Type type)
    {
        if (m_GetFieldInfoAndStaticTypeFromProperty == null)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.Name == "ScriptAttributeUtility")
                    {
                        MethodInfo mi = t.GetMethod("GetFieldInfoAndStaticTypeFromProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        m_GetFieldInfoAndStaticTypeFromProperty = (GetFieldInfoAndStaticTypeFromProperty)Delegate.CreateDelegate(typeof(GetFieldInfoAndStaticTypeFromProperty), mi);
                        break;
                    }
                }
                if (m_GetFieldInfoAndStaticTypeFromProperty != null) break;
            }
            if (m_GetFieldInfoAndStaticTypeFromProperty == null)
            {
                UnityEngine.Debug.LogError("GetFieldInfoAndStaticType::Reflection failed!");
                type = null;
                return null;
            }
        }
        return m_GetFieldInfoAndStaticTypeFromProperty(prop, out type);
    }

    public static T GetCustomAttributeFromProperty<T>(this SerializedProperty prop) where T : System.Attribute
    {
        var info = prop.GetFieldInfoAndStaticType(out _);
        return info.GetCustomAttribute<T>();
    }
    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty serializedProperty)
    {
        SerializedProperty currentProperty = serializedProperty.Copy();
        SerializedProperty nextSiblingProperty = serializedProperty.Copy();
        {
            nextSiblingProperty.Next(false);
        }

        if (currentProperty.Next(true))
        {
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;

                yield return currentProperty;
            }
            while (currentProperty.Next(false));
        }
    }
}