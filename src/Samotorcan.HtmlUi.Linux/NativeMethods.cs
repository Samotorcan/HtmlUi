using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Native methods
    /// </summary>
    internal static class NativeMethods
    {
        private const string CefDllName = "libcef";
        private const string XLibDllName = "libX11";

        private const CallingConvention CefCall = CallingConvention.Cdecl;

        /// <summary>
        /// Return the singleton X11 display shared with Chromium. The display is not
        /// thread-safe and must only be accessed on the browser process UI thread.
        /// </summary>
        /// <returns>XDisplay</returns>
        [DllImport(CefDllName, EntryPoint = "cef_get_xdisplay", CallingConvention = CefCall)]
        public static extern IntPtr get_xdisplay();

        /// <summary>
        /// Resize the window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [DllImport(XLibDllName)]
        extern public static void XResizeWindow(IntPtr display, IntPtr window, uint width, uint height);
    }
}
