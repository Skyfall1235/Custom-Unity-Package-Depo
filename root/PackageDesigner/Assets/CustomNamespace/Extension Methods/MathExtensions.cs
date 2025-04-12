using UnityEditor;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides general mathematical extension methods.
    /// </summary>
    public static class MathExtensions
    {
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
    }
}