using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Samotorcan.HtmlUi.Core.Metadata;
using Samotorcan.HtmlUi.Core.Utilities;

namespace Samotorcan.HtmlUi.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class SyncPropertyAttribute : Attribute
    {
        public static Dictionary<string, SyncProperty> GetProperties<TType>()
        {
            return typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<SyncPropertyAttribute>() != null)
                .ToDictionary(p => p.Name, p => new SyncProperty
                {
                    Name = p.Name,
                    SetDelegate = ExpressionUtility.CreateMethodDelegate(p.GetSetMethod(true)),
                    GetDelegate = ExpressionUtility.CreateMethodDelegate(p.GetGetMethod(true)),
                    SyncPropertyType = p.PropertyType
                });
        }
    }
}
