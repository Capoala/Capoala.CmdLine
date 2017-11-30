using System;
using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Contains extensions methods that extend native LINQ functionality.
    /// </summary>
    internal static class CommandLineLinq
    {
        /// <summary>
        /// Returns the specified collection, minus the given subset. 
        /// </summary>
        /// <typeparam name="T">The type of object in the collection.</typeparam>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="subset">The subset to remove from the source collection.</param>
        /// <returns>
        /// Returns the source collection, minus the subset. 
        /// If the subset was not found, then the source collection is returned without modification.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="sourceCollection"/> is null.
        /// </exception>
        internal static IEnumerable<T> ExcludeSubset<T>(this IEnumerable<T> sourceCollection, IEnumerable<T> subset)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));

            if (subset?.Any() ?? false)
            {
                if (subset.Count() <= sourceCollection.Count())
                {
                    int indexOfFirstMatch = sourceCollection.ToList().IndexOf(subset.FirstOrDefault());

                    if (indexOfFirstMatch != -1)
                    {
                        IEnumerable<T> remaining = sourceCollection.Skip(indexOfFirstMatch);

                        if (remaining.Any())
                        {
                            bool mismatchFound = false;
                            for (int i = 0; i < subset.Count(); i++)
                            {
                                T sourceCurrentElement = sourceCollection.ElementAt(indexOfFirstMatch + i);
                                T subsetCurrentElement = subset.ElementAt(i);

                                if (!sourceCurrentElement.Equals(subsetCurrentElement))
                                {
                                    mismatchFound = true;
                                    break;
                                }
                            }

                            if (mismatchFound)
                                return sourceCollection;
                            else
                            {
                                if (indexOfFirstMatch > 0)
                                    return sourceCollection.Take(indexOfFirstMatch).Skip(subset.Count());
                                else
                                    return sourceCollection.Skip(subset.Count());
                            }
                        }
                    }
                }
            }
            return sourceCollection;
        }

        /// <summary>
        /// Returns the specified subset of arguments from the source collection.
        /// </summary>
        /// <typeparam name="T">The type4 of object in the collection.</typeparam>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="subset">The subset to remove from the source collection.</param>
        /// <returns>
        /// Returns the subet collection if found in the source collection. 
        /// If the subset was not found, then an empty collection is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="sourceCollection"/> is null.
        /// </exception>
        internal static IEnumerable<T> GetSubset<T>(this IEnumerable<T> sourceCollection, IEnumerable<T> subset)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));

            if (subset.Count() > sourceCollection.Count())
                return Enumerable.Empty<T>();

            if (subset?.Any() ?? false)
            {
                if (sourceCollection.ExcludeSubset(subset).Count() == sourceCollection.Count()) return Enumerable.Empty<T>();

                if (subset.Count() <= sourceCollection.Count())
                {
                    int indexOfFirstMatch = sourceCollection.ToList().IndexOf(subset.FirstOrDefault());

                    if (indexOfFirstMatch != -1)
                    {
                        IEnumerable<T> remaining = sourceCollection.Skip(indexOfFirstMatch);

                        if (remaining.Any())
                        {
                            bool mismatchFound = false;
                            for (int i = 0; i < subset.Count(); i++)
                            {
                                T sourceCurrentElement = sourceCollection.ElementAt(indexOfFirstMatch + i);
                                T subsetCurrentElement = subset.ElementAt(i);

                                if (!sourceCurrentElement.Equals(subsetCurrentElement))
                                {
                                    mismatchFound = true;
                                    break;
                                }
                            }

                            if (mismatchFound)
                                return sourceCollection;
                            else
                            {
                                if (indexOfFirstMatch > 0)
                                    return sourceCollection.Take(indexOfFirstMatch).Skip(subset.Count());
                                else
                                    return sourceCollection.Skip(subset.Count());
                            }
                        }
                    }
                }
            }
            return sourceCollection;
        }

        /// <summary>
        /// Determines whether two sequences contain the same values, regardless of order.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="firstSequence">The source collection.</param>
        /// <param name="secondSequence">The collection to compare to.</param>
        /// <returns>
        /// Returns true if both sequences contain the same values.
        /// Returns false if <paramref name="secondSequence"/> is null, or the sequences do not contain the same values.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="firstSequence"/> is null.
        /// </exception>
        internal static bool UnorderedSequenceEquals<T>(this IEnumerable<T> firstSequence, IEnumerable<T> secondSequence)
        {
            if (firstSequence == null)
                throw new ArgumentNullException(nameof(firstSequence));
            else if (secondSequence == null)
                return false;
            else if (firstSequence.Count() != secondSequence.Count())
                return false;
            else
                foreach (T t in firstSequence)
                    if (!secondSequence.Contains(t))
                        return false;
            return true;
        }
    }
}