namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Undefined value.
    /// </summary>
    internal sealed class UndefinedValue
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
        public static UndefinedValue Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="UndefinedValue"/> class.
        /// </summary>
        static UndefinedValue()
        {
            Value = new UndefinedValue();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="UndefinedValue"/> class from being created.
        /// </summary>
        private UndefinedValue() { }

        #endregion
    }
}
