using System;

namespace Samotorcan.HtmlUi.Core.Attributes
{
    /// <summary>
    /// Exclude property or method from binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeAttribute : Attribute
    {
    }
}
