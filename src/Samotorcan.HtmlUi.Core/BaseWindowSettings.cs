using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base window settings.
    /// </summary>
    public class BaseWindowSettings
    {
        #region Properties
        #region Public

        #region View
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public string View { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindowSettings"/> class.
        /// </summary>
        public BaseWindowSettings()
        {
            View = "/Views/Index.html";
        }

        #endregion
    }
}
