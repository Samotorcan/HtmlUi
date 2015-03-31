using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// ObservableCollectionChangeAction
    /// </summary>
    internal enum ObservableCollectionChangeAction
    {
        /// <summary>
        /// The add.
        /// </summary>
        Add = 1,

        /// <summary>
        /// The remove.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// The replace.
        /// </summary>
        Replace = 3,

        /// <summary>
        /// The move.
        /// </summary>
        Move = 4
    }
}
