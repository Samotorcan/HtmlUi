using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Form.
    /// </summary>
    internal class Form : System.Windows.Forms.Form
    {
        #region Properties
        #region Public

        #region ProcessWndProc
        /// <summary>
        /// Process WND proc delegate,
        /// </summary>
        /// <param name="m">The m.</param>
        public delegate void ProcessWndProcAction(ref Message m);
        /// <summary>
        /// Gets or sets the process WND proc.
        /// </summary>
        /// <value>
        /// The process WND proc.
        /// </value>
        public ProcessWndProcAction ProcessWndProc { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        public Form()
            : base()
        {
            SetStyle(
                ControlStyles.ContainerControl
                | ControlStyles.ResizeRedraw
                | ControlStyles.FixedWidth
                | ControlStyles.FixedHeight
                | ControlStyles.StandardClick
                | ControlStyles.UserMouse
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.StandardDoubleClick
                | ControlStyles.CacheText
                | ControlStyles.EnableNotifyMessage
                | ControlStyles.UseTextForAccessibility
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.DoubleBuffer
                | ControlStyles.Opaque,
                false);

            SetStyle(
                ControlStyles.Selectable
                | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint,
                true);
        }

        #endregion
        #region Methods
        #region Internal

        #region WndProc
        /// <summary>
        /// WndProc.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (ProcessWndProc != null)
                ProcessWndProc(ref m);
        }
        #endregion

        #endregion
        #endregion
    }
}
