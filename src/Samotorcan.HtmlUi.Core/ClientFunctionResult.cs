using Newtonsoft.Json.Linq;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Client function result.
    /// </summary>
    internal class ClientFunctionResult
    {
        #region Properties
        #region Public

        #region Type
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ClientFunctionResultType Type { get; set; }
        #endregion
        #region Exception
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public JToken Exception { get; set; }
        #endregion
        #region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public JToken Value { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
