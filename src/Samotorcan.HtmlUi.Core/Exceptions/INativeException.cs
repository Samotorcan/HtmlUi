namespace Samotorcan.HtmlUi.Core.Exceptions
{
    /// <summary>
    /// Native exception interface
    /// </summary>
    internal interface INativeException
    {
        /// <summary>
        /// To the javascript exception.
        /// </summary>
        /// <returns></returns>
        JavascriptException ToJavascriptException();
    }
}
