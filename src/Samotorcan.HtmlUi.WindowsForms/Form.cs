using System.Windows.Forms;

namespace Samotorcan.HtmlUi.WindowsForms
{
    /// <summary>
    /// Form.
    /// </summary>
    internal class Form : System.Windows.Forms.Form
    {
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
    }
}
