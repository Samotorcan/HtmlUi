using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Main application settings.
    /// </summary>
    public class MainApplicationSettings : BaseMainApplicationSettings
    {
        #region Properties
        #region Public

        #region WindowSettings
        /// <summary>
        /// Gets the window settings.
        /// </summary>
        /// <value>
        /// The window settings.
        /// </value>
        public WindowSettings WindowSettings { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplicationSettings"/> class.
        /// </summary>
        /// <param name="windowSettings">The window settings.</param>
        public MainApplicationSettings(WindowSettings windowSettings)
        {
            WindowSettings = windowSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplicationSettings"/> class.
        /// </summary>
        public MainApplicationSettings()
            : this(new WindowSettings()) { }

        #endregion
    }
}
