using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Property access.
    /// </summary>
    [Flags]
    internal enum PropertyAccess
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
