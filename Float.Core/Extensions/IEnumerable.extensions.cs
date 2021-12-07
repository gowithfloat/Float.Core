using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Extensions on IEnumerable types.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns null if this sequence is empty, or this sequence if not.
        /// </summary>
        /// <returns>Null if this sequence is empty, or this sequence if not.</returns>
        /// <typeparam name="T1">The type of elements in the sequence.</typeparam>
        /// <param name="first">This sequence.</param>
        public static IEnumerable<T1> NullIfEmpty<T1>(this IEnumerable<T1> first)
        {
            return first?.Any() == true ? first : null;
        }

        /// <summary>
        /// Iterates this sequence, performing an action on each element.
        /// </summary>
        /// <returns>This enumerable object.</returns>
        /// <param name="first">This sequence.</param>
        /// <param name="action">An action to perform using elements from this sequence.</param>
        /// <typeparam name="T1">The type of elements in the sequence.</typeparam>
        public static IEnumerable<T1> ForEach<T1>(this IEnumerable<T1> first, Action<T1> action)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using (var enumerator1 = first.GetEnumerator())
            {
                while (enumerator1.MoveNext())
                {
                    action(enumerator1.Current);
                }
            }

            return first;
        }

        /// <summary>
        /// Applies a specified function to the corresponding elements of three sequences, producing a sequence of the results.
        /// </summary>
        /// <returns>An IEnumerable that contains merged elements of three input sequences.</returns>
        /// <param name="first">This sequence.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="third">The third sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the three sequences.</param>
        /// <typeparam name="T1">The type of the elements of this input sequence.</typeparam>
        /// <typeparam name="T2">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="T3">The type of the elements of the third input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(this IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, Func<T1, T2, T3, TResult> resultSelector)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (third == null)
            {
                throw new ArgumentNullException(nameof(third));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            using var enumerator1 = first.GetEnumerator();
            using var enumerator2 = second.GetEnumerator();
            using var enumerator3 = third.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext() && enumerator3.MoveNext())
            {
                yield return resultSelector(enumerator1.Current, enumerator2.Current, enumerator3.Current);
            }
        }

        /// <summary>
        /// Returns an enumerable for the keys of the pairs in the given enumerable.
        /// </summary>
        /// <returns>An enumerable over the keys in the given enumerable.</returns>
        /// <param name="enumerable">The enumerable object from which to retrieve keys.</param>
        /// <typeparam name="T1">The type of keys in the given enumerable.</typeparam>
        /// <typeparam name="T2">The type of values in the given enumerable.</typeparam>
        public static IEnumerable<T1> Keys<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            using var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Key;
            }
        }

        /// <summary>
        /// Returns an enumerable for the values of the pairs in the given enumerable.
        /// </summary>
        /// <returns>An enumerable over the values in the given enumerable.</returns>
        /// <param name="enumerable">The enumerable object from which to retrieve keys.</param>
        /// <typeparam name="T1">The type of keys in the given enumerable.</typeparam>
        /// <typeparam name="T2">The type of values in the given enumerable.</typeparam>
        public static IEnumerable<T2> Values<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            using var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Value;
            }
        }

        /// <summary>
        /// Creates a dictionary from an enumerable of key/value pairs.
        /// </summary>
        /// <returns>A new dictionary with keys and values from the pairs in the enumerable object.</returns>
        /// <param name="enumerable">The enumerable object from which to retrieve keys and values.</param>
        /// <typeparam name="T1">The type of keys in the enumerable and result dictionary.</typeparam>
        /// <typeparam name="T2">The type of values in the enumerable and result dictionary.</typeparam>
        public static IDictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            return enumerable.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// This is defined here as the Linq method is defined on <see cref="IEnumerable{T}"/>, not <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to check for emptiness.</param>
        /// <returns><c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.</returns>
        public static bool Any(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (enumerable is ICollection collection)
            {
                return collection.Count != 0;
            }

            // note that IEnumerator<T> is IDisposable, but IEnumerator is not
            // see https://github.com/dotnet/runtime/blob/master/src/libraries/System.Linq/src/System/Linq/AnyAll.cs#L12
            return enumerable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// This is defined here as the Linq method is defined on <see cref="IEnumerable{T}"/>, not <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="enumerable">An <see cref="IEnumerable"/> to whose elements to apply the predicate.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
        public static bool Any(this IEnumerable enumerable, Func<object, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var el in enumerable)
            {
                if (predicate(el))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if this collection includes any objects that are an instance of type T1.
        /// For less strict type checking, use <see cref="ContainsSubclassOf{T}"/> instead.
        /// </summary>
        /// <typeparam name="T1">The type for which to search.</typeparam>
        /// <param name="enumerable">A collection to search for inclusion of the given type.</param>
        /// <returns><c>true</c> if the exact type was found, <c>false</c> otherwise.</returns>
        public static bool ContainsInstanceOf<T1>(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            return enumerable.Any(el => typeof(T1) == el.GetType());
        }

        /// <summary>
        /// Returns true if this collection includes any objects that inherit from T1.
        /// For more strict type checking, use <see cref="ContainsInstanceOf{T}"/> instead.
        /// </summary>
        /// <typeparam name="T1">The type for which to search.</typeparam>
        /// <param name="enumerable">A collection to search for inclusion of the given type.</param>
        /// <returns><c>true</c> if the type was found, <c>false</c> otherwise.</returns>
        public static bool ContainsSubclassOf<T1>(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            return enumerable.Any(el => el.GetType().IsSubclassOf(typeof(T1)));
        }
    }
}
