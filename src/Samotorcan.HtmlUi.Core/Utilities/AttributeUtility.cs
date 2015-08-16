using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    internal static class AttributeUtility
    {
        public static Dictionary<string, TDelegate> GetMethods<TType, TAttribute, TDelegate>(TType obj, BindingFlags flags)
            where TDelegate : class
            where TAttribute : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return typeof(TType).GetMethods(flags)
                .Where(m => m.GetCustomAttribute<TAttribute>() != null)
                .ToDictionary(m => m.Name, m => m.IsStatic
                    ? (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), m)
                    : (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), obj, m));
        }
    }
}
