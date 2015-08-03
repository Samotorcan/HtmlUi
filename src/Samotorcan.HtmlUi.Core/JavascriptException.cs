using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Javascript exception.
    /// </summary>
    internal class JavascriptException
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
        public string Type { get; set; }
        #endregion
        #region Message
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        #endregion
        #region AdditionalData
        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        /// <value>
        /// The additional data.
        /// </value>
        public Dictionary<string, object> AdditionalData { get; set; }
        #endregion
        #region InnerException
        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>
        /// The inner exception.
        /// </value>
        public JavascriptException InnerException { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
