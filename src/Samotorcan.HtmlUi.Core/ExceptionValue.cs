namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Exception value.
    /// </summary>
    internal sealed class ExceptionValue
    {
        #region Properties
        #region Public

        #region Value
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public static ExceptionValue Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ExceptionValue"/> class.
        /// </summary>
        static ExceptionValue()
        {
            Value = new ExceptionValue();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ExceptionValue"/> class from being created.
        /// </summary>
        private ExceptionValue() { }

        #endregion
    }
}
