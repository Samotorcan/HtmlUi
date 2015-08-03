using System;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Access.
    /// </summary>
    [Flags]
    internal enum Access
    {
        /// <summary>
        /// The read.
        /// </summary>
        Read = 1,

        /// <summary>
        /// The write.
        /// </summary>
        Write = 2
    }
}
