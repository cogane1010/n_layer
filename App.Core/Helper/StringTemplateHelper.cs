using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace App.Core.Helper
{
    public static class StringTemplateHelper
    {

        public static string ReplaceMacro(string value, string root, object @object)
        {

            return Regex.Replace(value, @"\${(.+?)}",
            match => {
                var p = Expression.Parameter(@object.GetType(), root);
                try
                {
                    var e = DynamicExpressionParser.ParseLambda(new[] { p }, null, match.Groups[1].Value);
                    return (e.Compile().DynamicInvoke(@object) ?? "").ToString();
                }
                catch (Exception e)
                {
                    return "";
                }


            });
        }
    }
}
