using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// SourceMap
    /// </summary>
    internal class SourceMap
    {
        #region Properties
        #region Public

        #region Version
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }
        #endregion
        #region File
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File { get; set; }
        #endregion
        #region SourceRoot
        /// <summary>
        /// Gets or sets the source root.
        /// </summary>
        /// <value>
        /// The source root.
        /// </value>
        public string SourceRoot { get; set; }
        #endregion
        #region sources
        /// <summary>
        /// Gets or sets the sources.
        /// </summary>
        /// <value>
        /// The sources.
        /// </value>
        public List<string> sources { get; set; }
        #endregion
        #region SourcesContent
        /// <summary>
        /// Gets or sets the content of the sources.
        /// </summary>
        /// <value>
        /// The content of the sources.
        /// </value>
        public List<string> SourcesContent { get; set; }
        #endregion
        #region Names
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<string> Names { get; set; }
        #endregion
        #region Mappings
        /// <summary>
        /// Gets or sets the mappings.
        /// </summary>
        /// <value>
        /// The mappings.
        /// </value>
        public string Mappings { get; set; }
        #endregion

        #endregion
        #endregion
    }
}
