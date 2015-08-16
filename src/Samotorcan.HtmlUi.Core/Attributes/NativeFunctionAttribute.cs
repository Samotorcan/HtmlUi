using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Samotorcan.HtmlUi.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class NativeFunctionAttribute : Attribute
    {
        private string Handle { get; set; }

        public NativeFunctionAttribute(string handle)
        {
            Handle = handle;
        }

        public static Dictionary<string, TDelegate> GetHandlers<TType, TDelegate>(TType obj) where TDelegate : class
        {
            return typeof(TType).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(m => new
                {
                    Attribute = m.GetCustomAttribute<NativeFunctionAttribute>(),
                    Method = m
                })
                .Where(i => i.Attribute != null)
                .ToDictionary(i => i.Attribute.Handle, i => i.Method.IsStatic
                    ? (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), i.Method)
                    : (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), obj, i.Method));
        }
    }
}
