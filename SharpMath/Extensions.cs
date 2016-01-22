using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SharpMath
{
    internal static class Extensions
    {
        public static bool IsBracket(this string s)
        {
            return s == "(" || s == ")";
        }

        public static IEnumerable<int> AllIndicesOf(this string str, string searchString)
        {
            int minIndex = str.IndexOf(searchString, StringComparison.Ordinal);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchString, minIndex + searchString.Length, StringComparison.Ordinal);
            }
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}