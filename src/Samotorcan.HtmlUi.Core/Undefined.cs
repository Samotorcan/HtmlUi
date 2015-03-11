using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Undefined value.
    /// </summary>
    internal sealed class Undefined
    {
        #region Properties
        #region Public

        #region Value
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public static Undefined Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="Undefined"/> class.
        /// </summary>
        static Undefined()
        {
            Value = new Undefined();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Undefined"/> class from being created.
        /// </summary>
        private Undefined() { }

        #endregion
    }
}
