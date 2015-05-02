using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Client function result type.
    /// </summary>
    internal enum ClientFunctionResultType : int
    {
        /// <summary>
        /// The value.
        /// </summary>
        Value = 1,

        /// <summary>
        /// The undefined.
        /// </summary>
        Undefined = 2,

        /// <summary>
        /// The exception.
        /// </summary>
        Exception = 3,

        /// <summary>
        /// The function not found.
        /// </summary>
        FunctionNotFound = 4
    }
}
