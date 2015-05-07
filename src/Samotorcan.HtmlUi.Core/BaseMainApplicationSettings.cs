using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base main application settings.
    /// </summary>
    public class BaseMainApplicationSettings : BaseApplicationSettings
    {
        #region Properties
        #region Public

        #region D3D11Enabled
        /// <summary>
        /// Gets a value indicating whether D3D11 is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if D3D11 is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool D3D11Enabled { get; set; }
        #endregion
        #region CommandLineArgsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether command line arguments are enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if command line arguments are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CommandLineArgsEnabled { get; set; }
        #endregion
        #region RemoteDebuggingPort
        /// <summary>
        /// Gets or sets the remote debugging port. Set 0 to disable.
        /// </summary>
        /// <value>
        /// The remote debugging port.
        /// </value>
        public int RemoteDebuggingPort { get; set; }
        #endregion
        #region ChromeViewsEnabled
        /// <summary>
        /// Gets or sets a value indicating whether chrome views are enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if chrome views are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool ChromeViewsEnabled { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
