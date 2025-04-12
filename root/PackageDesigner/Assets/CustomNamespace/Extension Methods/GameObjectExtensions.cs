using UnityEngine;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="GameObject"/> class.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Checks if a GameObject has a specific component attached.
        /// </summary>
        /// <typeparam name="T">The type of the Component to check for.</typeparam>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <returns>True if the GameObject has the specified component, otherwise false.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// Recursively searches through the children and grandchildren of a Transform to find a child with the specified name.
        /// </summary>
        /// <param name="aParent">The parent Transform to start the search from.</param>
        /// <param name="aName">The name of the child Transform to find.</param>
        /// <returns>The Transform of the child with the specified name, or null if no such child is found.</returns>
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            foreach (Transform child in aParent)
            {
                if (child.name == aName)
                    return child;
                var result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}