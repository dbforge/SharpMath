// Extensions.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SharpMath
{
    internal static class Extensions
    {
        public static IEnumerable<int> AllIndicesOf(this string str, string searchString)
        {
            var minIndex = str.IndexOf(searchString, StringComparison.Ordinal);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchString, minIndex + searchString.Length, StringComparison.Ordinal);
            }
        }

        public static string GetEnumDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        public static bool IsBracket(this string s)
        {
            return s == "(" || s == ")";
        }
    }
}