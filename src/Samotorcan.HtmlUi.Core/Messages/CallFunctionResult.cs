using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Messages
{
    /// <summary>
    /// Call function result.
    /// </summary>
    internal class CallFunctionResult
    {
        #region Properties
        #region Public

        #region Result
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public JToken Result { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
