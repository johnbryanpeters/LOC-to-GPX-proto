using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    // From http://www.dotnetperls.com/array-slice
    public static class Extensions
    {
        /// Get the array slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        // jbp add, 2016-02-13.

        /// Get left segment of array.
        public static T[] Left<T>(this T[] source, int length)
        {
            T[] result = new T[length];
            if (length <= source.Length)
            {
                result = Slice(source, 0, length);
            }
            return result;
        }

        /// Get interior segment of array.
        public static T[] Mid<T>(this T[] source, int start, int length)
        {
            T[] result = new T[length];
            if (          length <= source.Length &&
                           start >= 0             &&
                (start + length) <= (source.Length))
            {
                result = Slice(source, start, length - 1);
            }
            return result;
        }

        /// Get right segment of array.
        public static T[] Right<T>(this T[] source, int start, int length)
        {
            T[] result = new T[length];
            if (source.Length - start >= length)
            {
                result = Slice(source, start, length);
            }
            return result;
        }

        // Keep? Don't keep?
        static int IndexOf(byte[] ByteArrayToSearch, byte[] ByteArrayToFind)
        {
            // Any encoding will do, as long as all bytes represent a unique character.
            Encoding encoding = Encoding.GetEncoding("utf-8"); // 1252

            string toSearch = encoding.GetString(ByteArrayToSearch, 0, ByteArrayToSearch.Length);
            string toFind = encoding.GetString(ByteArrayToFind, 0, ByteArrayToFind.Length);
            int result = toSearch.IndexOf(toFind, StringComparison.Ordinal);
            return result;
        } // End IndexOf()
    }
}