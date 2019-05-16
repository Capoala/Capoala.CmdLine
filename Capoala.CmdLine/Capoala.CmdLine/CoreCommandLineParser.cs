using System;
using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// The core parsing mechanism for parsing the command line.
    /// </summary>
    public static class CoreCommandLineParser
    {
        /// <summary>
        /// Determines if the argument was found in the provided command line segment.
        /// </summary>
        /// <param name="delimiter">The delimiter for the argument.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="comparisonType">Specifies the culture, case, and sort rules for comparison and equality operations.</param>
        /// <param name="commandLineSegment">The command line segment to parse.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the delimiter and argument name pairing is
        /// found within <paramref name="commandLineSegment"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Nested argument searching is not supported by this method alone. If a sub-argument
        /// is the target, then  you will need to provide the sub-segment to this method.
        /// </remarks>
        public static bool Found(string delimiter, string argumentName, StringComparison comparisonType, string commandLineSegment)
            => commandLineSegment.Split().Any(entry => entry.Equals($"{delimiter}{argumentName}", comparisonType));

        /// <summary>
        /// Determines if the argument was found in the provided command line segment.
        /// </summary>
        /// <param name="delimiter">The delimiter for the argument.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="comparisonType">Specifies the culture, case, and sort rules for comparison and equality operations.</param>
        /// <param name="commandLineSegment">The command line segment to parse.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the delimiter and argument name pairing is
        /// found within <paramref name="commandLineSegment"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Nested argument searching is not supported by this method alone. If a sub-argument
        /// is the target, then  you will need to provide the sub-segment to this method.
        /// </remarks>
        public static bool Found(string delimiter, string argumentName, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => commandLineSegment.Any(entry => entry.Equals($"{delimiter}{argumentName}", comparisonType));

        /// <summary>
        /// Returns the segment, including the argument, if found within the provided command line segment.
        /// </summary>
        /// <param name="delimiter">The delimiter for the argument.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="comparisonType">Specifies the culture, case, and sort rules for comparison and equality operations.</param>
        /// <param name="commandLineSegment">The command line segment to parse.</param>
        /// <returns>
        /// Returns an enumerable of the segment for the provided delimiter and argument name pairing.
        /// </returns>
        public static IEnumerable<string> GetSegment(string delimiter, string argumentName, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => commandLineSegment.SkipWhile(entry => !entry.Equals($"{delimiter}{argumentName}", comparisonType))
                        .TakeWhile(entry => !entry.StartsWith(delimiter, comparisonType));

        /// <summary>
        /// Returns the parameters for the argument if found within the provided command line segment.
        /// </summary>
        /// <param name="delimiter">The delimiter for the argument.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="comparisonType">Specifies the culture, case, and sort rules for comparison and equality operations.</param>
        /// <param name="commandLineSegment">The command line segment to parse.</param>
        /// <returns>
        /// Returns an enumerable of the parameters for the provided delimiter and argument name pairing.
        /// </returns>
        public static IEnumerable<string> GetParams(string delimiter, string argumentName, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => GetSegment(delimiter, argumentName, comparisonType, commandLineSegment).Skip(1);





        public static ValueTuple<int, int, IEnumerable<string>> FindFirst(string delimiter, string argumentName, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
        {
            var argument = $"{delimiter}{argumentName}";
            var elementsSkipped = 0;
            var elementsTaken = 0;
            var didFindFirstMatch = false;

            foreach (var entry in commandLineSegment)
            {
                if (didFindFirstMatch)
                {
                    if (entry.StartsWith(delimiter, comparisonType))
                        break;
                    else
                        elementsTaken++;
                }
                else
                {
                    if (entry.Equals(argument, comparisonType))
                    {
                        didFindFirstMatch = true;
                        elementsTaken++;
                    }
                    else
                        elementsSkipped++;
                }
            }

            return (elementsSkipped, elementsTaken, commandLineSegment.Skip(elementsSkipped).Take(elementsTaken));
        }

        public static IEnumerable<ValueTuple<int, int, IEnumerable<string>>> FindAll(string delimiter, string argumentName, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
        {
            var argument = $"{delimiter}{argumentName}";
            var elementsSkipped = 0;
            var elementsTaken = 0;
            var didFindFirstMatch = false;

            foreach (var entry in commandLineSegment)
            {
                if (didFindFirstMatch)
                {
                    if (entry.StartsWith(delimiter, comparisonType))
                    {
                        yield return (elementsSkipped, elementsTaken, commandLineSegment.Skip(elementsSkipped).Take(elementsTaken));

                        elementsSkipped += elementsTaken;

                        elementsTaken = 0;

                        if (entry.Equals(argument))
                        {
                            elementsTaken++;
                        }
                        else
                        {
                            didFindFirstMatch = false;
                        }
                    }
                    else
                        elementsTaken++;
                }
                else
                {
                    if (entry.Equals(argument, comparisonType))
                    {
                        didFindFirstMatch = true;
                        elementsTaken++;
                    }
                    else
                        elementsSkipped++;
                }
            }

            if (elementsTaken > 0)
                yield return (elementsSkipped, elementsTaken, commandLineSegment.Skip(elementsSkipped).Take(elementsTaken));

            //return (elementsSkipped, elementsTaken, commandLineSegment.Skip(elementsSkipped).Take(elementsTaken));
        }
    }
}
