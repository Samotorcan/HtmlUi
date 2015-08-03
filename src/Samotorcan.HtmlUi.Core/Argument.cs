namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Argument.
    /// </summary>
    public sealed class Argument
    {
        #region Arguments

        #region DisableD3D11
        /// <summary>
        /// The disable d3d11 argument.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It's immutable.")]
        public static readonly Argument DisableD3D11 = new Argument("--disable-d3d11");
        #endregion

        #endregion
        #region Properties
        #region Public

        #region Value
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private Argument(string value)
        {
            Value = value;
        }

        #endregion
    }
}
