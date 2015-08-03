using Newtonsoft.Json.Linq;

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
