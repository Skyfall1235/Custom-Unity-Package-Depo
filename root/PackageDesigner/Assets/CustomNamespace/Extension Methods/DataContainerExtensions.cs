using System.Collections.Generic;
using System.Linq;

namespace CustomNamespace.Extensions
{
    /// <summary>
    /// Provides extension methods for various data container types.
    /// </summary>
    public static class DataContainerExtensions
    {
        /// <summary>
        /// Returns the next node in the LinkedList, or the first node if the current node is the last.
        /// </summary>
        /// <typeparam name="T">The type of elements in the LinkedList.</typeparam>
        /// <param name="current">The current LinkedListNode.</param>
        /// <returns>The next LinkedListNode, or the first node if there is no next node.</returns>
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        /// <summary>
        /// Returns the previous node in the LinkedList, or the last node if the current node is the first.
        /// </summary>
        /// <typeparam name="T">The type of elements in the LinkedList.</typeparam>
        /// <param name="current">The current LinkedListNode.</param>
        /// <returns>The previous LinkedListNode, or the last node if there is no previous node.</returns>
        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }

        /// <summary>
        /// Resizes a List to the specified desired size. If the desired size is smaller than the current size, elements are removed from the end.
        /// If the desired size is larger, the list is padded with the provided element.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="listToResize">The List to resize.</param>
        /// <param name="desiredSize">The new desired size of the List.</param>
        /// <param name="element">The element to add when increasing the size of the List.</param>
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

        /// <summary>
        /// Resizes a List to the specified desired size. If the desired size is smaller than the current size, elements are removed from the end.
        /// If the desired size is larger, the list is padded with a new instance of the default value for type T.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List. This type must have a default constructor.</typeparam>
        /// <param name="listToResize">The List to resize.</param>
        /// <param name="desiredSize">The new desired size of the List.</param>
        public static void Resize<T>(this List<T> listToResize, int desiredSize) where T : new()
        {
            listToResize.Resize(desiredSize, new T());
        }
    }
}