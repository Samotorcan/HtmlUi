using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Events
{
    /// <summary>
    /// Key press event arguments.
    /// </summary>
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

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPressEventArgs"/> class.
        /// </summary>
        /// <param name="nativeKeyCode">The native key code.</param>
        public KeyPressEventArgs(int nativeKeyCode)
        {
            Argument.Null(nativeKeyCode, "nativeKeyCode");

            NativeKeyCode = nativeKeyCode;
        }

        #endregion
    }
}
