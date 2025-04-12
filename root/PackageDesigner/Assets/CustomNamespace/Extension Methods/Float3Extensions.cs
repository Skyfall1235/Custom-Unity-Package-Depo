using Unity.Mathematics;
using UnityEngine;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="float3"/> structure from Unity.Mathematics.
    /// </summary>
    public static class Float3Extensions
    {
        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="float3"/>.
        /// </summary>
        /// <param name="from">The <see cref="Vector3"/> to convert.</param>
        /// <returns>A new <see cref="float3"/> with the same component values.</returns>
        public static float3 Vector3ToFloat3(this Vector3 from)
        {
            return new float3(from.x, from.y, from.z);
        }

        /// <summary>
        /// Converts a <see cref="float3"/> to a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="from">The <see cref="float3"/> to convert.</param>
        /// <returns>A new <see cref="Vector3"/> with the same component values.</returns>
        public static Vector3 Float3ToVector3(this float3 from)
        {
            return new Vector3(from.x, from.y, from.z);
        }

        /// <summary>
        /// Calculates the distance between two <see cref="float3"/> points.
        /// </summary>
        /// <param name="a">The first <see cref="float3"/> point.</param>
        /// <param name="b">The second <see cref="float3"/> point.</param>
        /// <returns>The distance between the two points.</returns>
        public static float Distance(this float3 a, float3 b)
        {
            return math.sqrt(math.pow(a.x - b.x, 2) + math.pow(a.y - b.y, 2) + math.pow(a.z - b.z, 2));
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="float3"/> vectors.
        /// </summary>
        /// <param name="a">The first <see cref="float3"/> vector.</param>
        /// <param name="b">The second <see cref="float3"/> vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float DotProduct(float3 a, float3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        /// <summary>
        /// Calculates the magnitude (length) of a <see cref="float3"/> vector.
        /// </summary>
        /// <param name="floatVal">The <see cref="float3"/> vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public static float GetFloat3Magnitude(this float3 floatVal)
        {
            return math.sqrt(floatVal.x * floatVal.x + floatVal.y * floatVal.y + floatVal.z * floatVal.z);
        }

        /// <summary>
        /// Returns a normalized version of the <see cref="float3"/> vector (a vector with the same direction but a magnitude of 1).
        /// </summary>
        /// <param name="vector">The <see cref="float3"/> vector to normalize.</param>
        /// <returns>The normalized <see cref="float3"/> vector, or <see cref="float3.zero"/> if the input vector's magnitude is zero.</returns>
        public static float3 NormalizeVector(this float3 vector)
        {
            float magnitude = vector.GetFloat3Magnitude();
            if (magnitude > 0)
            {
                return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
            }
            else
            {
                return float3.zero;
            }
        }

        /// <summary>
        /// Calculates the cross product of two <see cref="float3"/> vectors.
        /// The cross product of two vectors results in a new vector that is perpendicular to both.
        /// </summary>
        /// <param name="a">The first <see cref="float3"/> vector.</param>
        /// <param name="b">The second <see cref="float3"/> vector.</param>
        /// <returns>The cross product of the two vectors.</returns>
        public static float3 Cross(this float3 a, float3 b)
        {
            return math.cross(a, b);
        }

        /// <summary>
        /// Returns a new <see cref="float3"/> vector with each component clamped to the specified minimum and maximum values.
        /// </summary>
        /// <param name="vector">The <see cref="float3"/> vector to clamp.</param>
        /// <param name="min">The minimum allowed value for each component.</param>
        /// <param name="max">The maximum allowed value for each component.</param>
        /// <returns>A new <see cref="float3"/> vector with clamped components.</returns>
        public static float3 Clamp(this float3 vector, float min, float max)
        {
            return math.clamp(vector, min, max);
        }

        /// <summary>
        /// Returns a new <see cref="float3"/> vector with the absolute value of each component.
        /// </summary>
        /// <param name="vector">The <see cref="float3"/> vector.</param>
        /// <returns>A new <see cref="float3"/> vector with absolute values.</returns>
        public static float3 Abs(this float3 vector)
        {
            return math.abs(vector);
        }
    }
}