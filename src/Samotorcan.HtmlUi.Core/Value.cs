namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Value.
    /// </summary>
    internal static class Value
    {
        #region Properties
        #region Public

        #region Undefined
        /// <summary>
        /// Gets the undefined.
        /// </summary>
        /// <value>
        /// The undefined.
        /// </value>
        public static UndefinedValue Undefined
        {
            get
            {
                return UndefinedValue.Value;
            }
        }
        #endregion
        #region Exception
        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public static ExceptionValue Exception
        {
            get
            {
                return ExceptionValue.Value;
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
