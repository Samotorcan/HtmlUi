using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Cef argument.
    /// </summary>
    public sealed class CefArgument
    {
        #region Arguments

        #region DisableD3D11
        /// <summary>
        /// The disable d3d11 argument.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly CefArgument DisableD3D11 = new CefArgument("--disable-d3d11");
        #endregion

        #endregion
        #region Properties
        #region Public

        #region Value
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CefArgument"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private CefArgument(string value)
        {
            Value = value;
        }

        #endregion
    }
}
