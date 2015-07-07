using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Child application.
    /// </summary>
    public class ChildApplication : BaseChildApplication
    {
        #region Properties
        #region Public

        #region Current
        /// <summary>
        /// Gets the current application.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static new ChildApplication Current
        {
            get
            {
                return (ChildApplication)BaseChildApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class.
        /// </summary>
        public ChildApplication(ChildApplicationSettings settings)
            : base(settings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildApplication"/> class with default settings.
        /// </summary>
        public ChildApplication()
            : this(new ChildApplicationSettings()) { }

        #endregion
    }
}
