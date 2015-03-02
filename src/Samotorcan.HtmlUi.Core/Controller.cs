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
    /// Controller. TODO: create attributes like controller name, camel case ...
    /// </summary>
    public abstract class Controller : IDisposable
    {
        #region Properties
        #region Public

        #region Id
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; private set; }
        #endregion

        #endregion
        #region Private

        #region Properties
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        private List<ControllerPropertyInfo> Properties { get; set; }
        #endregion
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        private string Name { get; set; }
        #endregion
        #region Type
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        private Type Type { get; set; }
        #endregion
        #region IgnoreProperties
        /// <summary>
        /// Gets or sets the ignore properties.
        /// </summary>
        /// <value>
        /// The ignore properties.
        /// </value>
        private List<string> IgnoreProperties { get; set; }
        #endregion

        #endregion
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        protected Controller(int id)
        {
            Id = id;

            Properties = new List<ControllerPropertyInfo>();
            IgnoreProperties = new List<string>
            {
                "Id"
            };
            Type = GetType();
            Name = Type.Name;

            // find all properties
            foreach (var property in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !IgnoreProperties.Contains(p.Name)))
            {
                var readAccess = property.CanRead && property.GetGetMethod(false) != null;
                var writeAccess = property.CanWrite && property.GetSetMethod(false) != null;
                PropertyAccess? access = null;

                // access
                if (readAccess && writeAccess)
                    access = PropertyAccess.Read | PropertyAccess.Write;
                else if (readAccess)
                    access = PropertyAccess.Read;
                else if (writeAccess)
                    access = PropertyAccess.Write;

                // add
                if (access != null)
                    Properties.Add(new ControllerPropertyInfo { Name = property.Name, PropertyInfo = property, Access = access.Value });
            }
        }

        #endregion
        #region Methods
        #region Internal

        #region GetProperties
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        private List<ControllerProperty> GetProperties(PropertyAccess? access, PropertyNameType propertyNameType)
        {
            return Properties.Where(p => access == null || p.Access.HasFlag(access.Value))
                .Select(p => new ControllerProperty
                {
                    Name = propertyNameType == PropertyNameType.CamelCase ? StringUtility.CamelCase(p.Name) : p.Name,
                    Value = p.Access.HasFlag(PropertyAccess.Read) ? p.PropertyInfo.GetValue(this, null) : null,
                    Access = p.Access
                })
                .ToList();
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        internal List<ControllerProperty> GetProperties(PropertyAccess access, PropertyNameType propertyNameType)
        {
            return GetProperties((PropertyAccess?)access, propertyNameType);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        internal List<ControllerProperty> GetProperties(PropertyNameType propertyNameType)
        {
            return GetProperties(null, propertyNameType);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        internal List<ControllerProperty> GetProperties()
        {
            return GetProperties(null, PropertyNameType.Normal);
        }
        #endregion
        #region GetDescription
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        internal ControllerDescription GetDescription(PropertyNameType propertyNameType)
        {
            return new ControllerDescription
            {
                Name = Name,
                Properties = GetProperties(propertyNameType)
            };
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        internal ControllerDescription GetDescription()
        {
            return GetDescription(PropertyNameType.CamelCase);
        }
        #endregion
        #region TrySetProperty
        /// <summary>
        /// Tries to set property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyNameType">Type of the property name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">propertyName</exception>
        internal bool TrySetProperty(string propertyName, object value, PropertyNameType propertyNameType)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            var property = Properties.FirstOrDefault(p => p.Access.HasFlag(PropertyAccess.Write) &&
                (propertyNameType == PropertyNameType.CamelCase
                    ? StringUtility.CamelCase(p.Name) == propertyName
                    : p.Name == propertyName));

            if (property != null)
            {
                var propertyInfo = property.PropertyInfo;
                var propertyType = propertyInfo.PropertyType;

                if (value == null)
                {
                    if (!propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null)
                    {
                        propertyInfo.SetValue(this, value);
                        return true;
                    }
                }
                else
                {
                    var valueType = value.GetType();

                    if (valueType == propertyType)
                    {
                        propertyInfo.SetValue(this, value);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to set property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal bool TrySetProperty(string propertyName, object value)
        {
            return TrySetProperty(propertyName, value, PropertyNameType.CamelCase);
        }
        #endregion

        #endregion
        #endregion
        #region Data

        #region ControllerPropertyInfo
        /// <summary>
        /// Controller property info.
        /// </summary>
        private class ControllerPropertyInfo : ControllerProperty
        {
            /// <summary>
            /// Gets or sets the property info.
            /// </summary>
            /// <value>
            /// The property info.
            /// </value>
            public PropertyInfo PropertyInfo { get; set; }
        }
        #endregion

        #endregion

        #region IDisposable

        /// <summary>
        /// Was dispose already called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
