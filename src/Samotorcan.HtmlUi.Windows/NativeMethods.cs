using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Native methods.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        public const int HT_CAPTION = 0x2;
        public const int WM_SYSCOMMAND = 0x112;
        public const int MF_STRING = 0x0;
        public const int MF_SEPARATOR = 0x800;

        public const int SYSMENU_DEVTOOLS_ID = 0x1;

        [DllImportAttribute("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImportAttribute("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, uint threadID);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)]bool bRevert);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "3")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "4")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum Messages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "P/Invoke class.")]
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public uint message;
            public IntPtr hwnd;
        }

        /// <summary>
        /// Set window position flags.
        /// </summary>
        [Flags]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.
            /// </summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsyncWindowPosition = 0x4000,

            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,

            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,

            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.
            /// </summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,

            /// <summary>
            /// Hides the window.
            /// </summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,

            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            NoActivate = 0x0010,

            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified, the valid contents 
            /// of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            NoCopyBits = 0x0100,

            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            /// <remarks>SWP_NOMOVE</remarks>
            NoMove = 0x0002,

            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            NoOwnerZOrder = 0x0200,

            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            NoRedraw = 0x0008,

            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            NoReposition = 0x0200,

            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            NoSendChanging = 0x0400,

            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            /// <remarks>SWP_NOSIZE</remarks>
            NoSize = 0x0001,

            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            /// <remarks>SWP_NOZORDER</remarks>
            NoZOrder = 0x0004,

            /// <summary>
            /// Displays the window.
            /// </summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040
        }
    }
}
