using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Internal method attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class InternalMethodAttribute : Attribute
    {
        #region Methods
        #region Public

        #region GetMethods
        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static Dictionary<string, TDelegate> GetMethods<TType, TDelegate>(TType obj) where TDelegate : class
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return typeof(TType).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.GetCustomAttribute<InternalMethodAttribute>() != null)
                .ToDictionary(m => StringUtility.Normalize(m.Name, NormalizeType.CamelCase), m => m.IsStatic
                    ? (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), m)
                    : (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), obj, m));
        }
        #endregion

        #endregion
        #endregion
    }
}
