using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace App.Core.Helper
{
    public static class StringHelper
    {

        public static bool IsNullOrEmpty(this string src)
        {
            return src == null || src.Trim() == ""; 
        }

        public static IEnumerable<T> AddIfNotExists<T>(this IEnumerable<T> list, T value)
        {
            if (!list.Contains(value))
            {
                return list.Append(value);
            }
            return list;
        }
    }
}
