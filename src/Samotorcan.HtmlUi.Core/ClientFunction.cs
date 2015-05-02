using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Client function.
    /// </summary>
    internal class ClientFunction
    {
        #region Properties
        #region Public

        #region ControllerId
        /// <summary>
        /// Gets or sets the controller identifier.
        /// </summary>
        /// <value>
        /// The controller identifier.
        /// </value>
        public int ControllerId { get; set; }
        #endregion
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        #endregion
        #region Arguments
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        [JsonProperty(PropertyName = "args")]
        public object[] Arguments { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
