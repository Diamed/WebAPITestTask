using System;
using System.Collections.Generic;
using System.Linq;

namespace DerivcoTestTask.Model
{
    public static class Extensions
    {
        public static T[,] To2D<T>(this T[][] source)
        {
            try
            {
                var firstDimension = source.Length;
                var secondDimension = source.GroupBy(row => row.Length).Single().Key;

                var result = new T[firstDimension, secondDimension];
                for (int i = 0; i < firstDimension; ++i)
                {
                    for (int j = 0; j < secondDimension; ++j)
                    {
                        result[i, j] = source[i][j];
                    }
                }

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }

        public static IEnumerable<T> Order<T>(this IEnumerable<T> input) where T : Square
        {
            return input.OrderBy(s => s.X).ThenBy(s => s.Y);
        }
    }
}