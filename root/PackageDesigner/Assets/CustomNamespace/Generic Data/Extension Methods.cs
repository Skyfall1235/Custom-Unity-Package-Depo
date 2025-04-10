using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
public static class ExtensionMethods
{

    #region Data Container Manipulation

    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
    {
        return current.Next ?? current.List.First;
    }

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
    {
        return current.Previous ?? current.List.Last;
    }

    #endregion

    #region float3 methods for utilization in Unity Jobs

    public static float3 Vector3ToFloat3(this Vector3 from)
    {
        return new float3(from.x, from.y, from.z);
    }

    public static Vector3 Float3ToVector3(this float3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }

    public static float DotProduct(float3 a, float3 b)
    {
        float product = a.x * b.x + a.y * b.y + a.z * b.z;
        return product;
    }

    public static float GetFloat3Magnitude(this float3 floatVal)
    {
        return (float)Mathf.Sqrt(floatVal.x * floatVal.x + floatVal.y * floatVal.y + floatVal.z * floatVal.z);
    }

    public static float3 NormalizeVector(this float3 vector)
    {
        float magnitude = GetFloat3Magnitude(vector);
        if (magnitude > 0)
        {
            return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }
        else
        {
            return float3.zero;
        }
    }

    #endregion

    #region Vector Methods

    /// <summary>
    /// Creates a new Vector3 with the absolute values of each component of the input vector.
    /// </summary>
    /// <param name="inputVector">The Vector3 to get the absolute values of.</param>
    /// <returns>A new Vector3 with the absolute values of the input vector's components.</returns>
    static Vector3 Abs(this Vector3 inputVector)
    {
        return new Vector3(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.y), Mathf.Abs(inputVector.z));
    }

    /// <summary>
    /// Calculates the absolute value of a Vector2 by combining its x and y components into a single float and then taking the absolute value.
    /// </summary>
    /// <param name="vector">The Vector2 to calculate the absolute value of.</param>
    /// <returns>The absolute value of the combined x and y components of the vector.</returns>
    public static float AbsOfVector2AsFloat(this Vector2 vector)
    {
        float combinedVector = Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
        return combinedVector;
    }

    /// <summary>
    /// Calculates the absolute value of a Vector3 by combining its x, y, and z components into a single float and then taking the absolute value.
    /// </summary>
    /// <param name="vector">The Vector3 to calculate the absolute value of.</param>
    /// <returns>The absolute value of the combined x, y, and z components of the vector.</returns>
    public static float AbsOfVector3AsFloat(this Vector3 vector)
    {
        float combinedVector = Mathf.Abs(vector.x) + Mathf.Abs(vector.y) + Mathf.Abs(vector.z);
        return combinedVector;
    }

    #endregion

    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return gameObject.GetComponent<T>() != null;
    }

    /// <summary>
    /// Maps a value from one range to another
    /// </summary>
    /// <param name="value">The value you want to remap</param>
    /// <param name="from1">The lower bound of the starting range</param>
    /// <param name="to1">The upper bound of the starting range</param>
    /// <param name="from2">The lower bound of the ending range</param>
    /// <param name="to2">The upper bound of the ending range</param>
    /// <returns>The value remapped from one range to another as a float</returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void SwingTwist(this Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
    {
        Vector3 r = new Vector3(q.x, q.y, q.z);

        // singularity: rotation by 180 degree
        if (r.sqrMagnitude < float.Epsilon)
        {
            Vector3 rotatedTwistAxis = q * twistAxis;
            Vector3 swingAxis = Vector3.Cross(twistAxis, rotatedTwistAxis);

            if (swingAxis.sqrMagnitude > float.Epsilon)
            {
                float swingAngle =
                  Vector3.Angle(twistAxis, rotatedTwistAxis);
                swing = Quaternion.AngleAxis(swingAngle, swingAxis);
            }
            else
            {
                // more singularity: 
                // rotation axis parallel to twist axis
                swing = Quaternion.identity; // no swing
            }

            // always twist 180 degree on singularity
            twist = Quaternion.AngleAxis(180.0f, twistAxis);
            return;
        }

        // meat of swing-twist decomposition
        Vector3 p = Vector3.Project(r, twistAxis);
        twist = new Quaternion(p.x, p.y, p.z, q.w);
        twist = Quaternion.Normalize(twist);
        swing = q * Quaternion.Inverse(twist);
    }

    /// <summary>
    /// Gets a private value from an object using reflection
    /// This is usually bad but unity is forcing my hand
    /// </summary>
    /// <typeparam name="T">The type of variable that you are retrieving</typeparam>
    /// <param name="obj">object to get values from</param>
    /// <param name="name">The name of the variable you wish to retrieve</param>
    /// <returns>the value of the variable by the name passed into the method</returns>
    public static T GetFieldValue<T>(this object obj, string name)
    {
        // Set the flags so that private and public fields from instances will be found
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var field = obj.GetType().GetField(name, bindingFlags);
        return (T)field?.GetValue(obj);
    }

    public static void Resize<T>(this List<T> listToResize, int desiredSize, T element)
    {
        int current = listToResize.Count;
        if (desiredSize < current)
            listToResize.RemoveRange(desiredSize, current - desiredSize);
        else if (desiredSize > current)
        {
            if (desiredSize > listToResize.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                listToResize.Capacity = desiredSize;
            listToResize.AddRange(Enumerable.Repeat(element, desiredSize - current));
        }
    }
    public static void Resize<T>(this List<T> listToResize, int desiredSize) where T : new()
    {
        Resize(listToResize, desiredSize, new T());
    }
}
