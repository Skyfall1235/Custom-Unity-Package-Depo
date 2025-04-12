using System.Reflection;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods that utilize reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a private value from an object using reflection
        /// This is usually bad but unity is forcing my hand
        /// </summary>
        /// <remarks>
        /// Using reflection to access private members can have performance implications and may break if the internal structure of the target object changes.
        /// Consider alternative approaches if possible.
        /// </remarks>
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
    }
}