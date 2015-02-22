using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Validation
{
    /// <summary>
    /// Argument validation. TODO: remove me
    /// </summary>
    public static class Argument
    {
        #region Methods
        #region Public

        #region Null
        /// <summary>
        /// Null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        public static void Null(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }
        #endregion
        #region NullOrEmpty
        /// <summary>
        /// Null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        public static void NullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name);
        }
        #endregion
        #region NullOrWhiteSpace
        /// <summary>
        /// Null or white space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        public static void NullOrWhiteSpace(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(name);
        }
        #endregion
        #region InvalidArgument
        /// <summary>
        /// Invalid argument.
        /// </summary>
        /// <param name="invalid">if set to <c>true</c> invalid.</param>
        /// <param name="message">The message.</param>
        /// <param name="name">The name.</param>
        public static void InvalidArgument(bool invalid, string message, string name)
        {
            if (invalid)
                throw new ArgumentException(message, name);
        }
        #endregion
        #region InvalidOperation
        /// <summary>
        /// Invalid operation.
        /// </summary>
        /// <param name="invalid">if set to <c>true</c> invalid.</param>
        /// <param name="message">The message.</param>
        public static void InvalidOperation(bool invalid, string message)
        {
            if (invalid)
                throw new InvalidOperationException(message);
        }
        #endregion

        #endregion
        #endregion
    }
}
