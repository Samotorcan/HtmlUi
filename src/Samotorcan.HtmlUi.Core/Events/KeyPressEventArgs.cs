using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Events
{
    /// <summary>
    /// Key press event arguments.
    /// </summary>
    [CLSCompliant(false)]
    public class KeyPressEventArgs : EventArgs
    {
        #region Properties
        #region Public

        #region NativeKeyCode
        /// <summary>
        /// Gets the native key code.
        /// </summary>
        /// <value>
        /// The native key code.
        /// </value>
        public int NativeKeyCode { get; private set; }
        #endregion
        #region Modifiers
        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        public CefEventFlags Modifiers { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPressEventArgs"/> class.
        /// </summary>
        /// <param name="nativeKeyCode">The native key code.</param>
        /// <param name="modifiers">The modifiers.</param>
        public KeyPressEventArgs(int nativeKeyCode, CefEventFlags modifiers)
        {
            NativeKeyCode = nativeKeyCode;
            Modifiers = modifiers;
        }

        #endregion
    }
}
