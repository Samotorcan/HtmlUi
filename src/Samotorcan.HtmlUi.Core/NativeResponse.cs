namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Native response.
    /// </summary>
    internal class NativeResponse
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
        public NativeResponseType Type { get; set; }
        #endregion
        #region Exception
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public JavascriptException Exception { get; set; }
        #endregion
        #region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
