using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Messages
{
    /// <summary>
    /// CallMethod
    /// </summary>
    internal class CallMethod
    {
        #region Properties
        #region Public

        #region Id
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        #endregion
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        #endregion
        #region Arguments
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public JArray Arguments { get; set; }
        #endregion
        #region InternalMethod
        /// <summary>
        /// Gets or sets a value indicating whether [internal method].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [internal method]; otherwise, <c>false</c>.
        /// </value>
        public bool InternalMethod { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
