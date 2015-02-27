using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base child application.
    /// </summary>
    public abstract class BaseChildApplication : BaseApplication
    {
        #region Properties
        #region Public

        #region Current
        /// <summary>
        /// Gets the current application.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static new BaseChildApplication Current
        {
            get
            {
                return (BaseChildApplication)BaseApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseChildApplication"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Application must be run on child application process.</exception>
        public BaseChildApplication()
            : base()
        {
            if (HtmlUiRuntime.ApplicationType != ApplicationType.ChildApplication)
                throw new InvalidOperationException("Application must be run on child application process.");
        }

        #endregion
        #region Methods
        #region Protected

        #region RunInternal
        /// <summary>
        /// Run internal.
        /// </summary>
        protected override void RunInternal()
        {
            CefRuntime.Load();

            var arguments = Environment.GetCommandLineArgs();
            var mainArgs = new CefMainArgs(arguments.ToArray());
            var app = new App();

            CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
        }
        #endregion

        #endregion
        #endregion
    }
}
