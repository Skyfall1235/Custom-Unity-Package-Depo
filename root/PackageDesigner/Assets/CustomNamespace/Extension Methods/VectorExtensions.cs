using UnityEngine;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Vector2"/> and <see cref="Vector3"/> structures.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Creates a new Vector3 with the absolute values of each component of the input vector.
        /// </summary>
        /// <param name="inputVector">The Vector3 to get the absolute values of.</param>
        /// <returns>A new Vector3 with the absolute values of the input vector's components.</returns>
        public static Vector3 Abs(this Vector3 inputVector)
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
    }
}