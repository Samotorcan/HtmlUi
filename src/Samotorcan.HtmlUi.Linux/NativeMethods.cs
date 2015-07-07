using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Native methods.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("libX11")]
        extern public static IntPtr XOpenDisplay(string displayName);

        [DllImport("libX11")]
        extern public static int XDefaultScreen(IntPtr display);

        [DllImport("libX11")]
        extern public static IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, int x, int y, uint width, uint height, uint borderWidth, ulong border, ulong background);

        [DllImport("libX11")]
        extern public static IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11")]
        extern public static ulong XBlackPixel(IntPtr display, int screenNumber);

        [DllImport("libX11")]
        extern public static ulong XWhitePixel(IntPtr display, int screenNumber);

        [DllImport("libX11")]
        extern public static void XSelectInput(IntPtr display, IntPtr window, EventMask eventMask);

        [DllImport("libX11")]
        public extern static void XNextEvent(IntPtr display, ref XEvent nextEvent);

        [DllImport("libX11")]
        extern public static void XMapWindow(IntPtr display, IntPtr window);

        [DllImport("libX11")]
        extern public static void XCloseDisplay(IntPtr display);

        [Flags]
        public enum EventMask
        {
            NoEventMask = 0,
            KeyPressMask = (1 << 0),
            KeyReleaseMask = (1 << 1),
            ButtonPressMask = (1 << 2),
            ButtonReleaseMask = (1 << 3),
            EnterWindowMask = (1 << 4),
            LeaveWindowMask = (1 << 5),
            PointerMotionMask = (1 << 6),
            PointerMotionHintMask = (1 << 7),
            Button1MotionMask = (1 << 8),
            Button2MotionMask = (1 << 9),
            Button3MotionMask = (1 << 10),
            Button4MotionMask = (1 << 11),
            Button5MotionMask = (1 << 12),
            ButtonMotionMask = (1 << 13),
            KeymapStateMask = (1 << 14),
            ExposureMask = (1 << 15),
            VisibilityChangeMask = (1 << 16),
            StructureNotifyMask = (1 << 17),
            ResizeRedirectMask = (1 << 18),
            SubstructureNotifyMask = (1 << 19),
            SubstructureRedirectMask = (1 << 20),
            FocusChangeMask = (1 << 21),
            PropertyChangeMask = (1 << 22),
            ColormapChangeMask = (1 << 23),
            OwnerGrabButtonMask = (1 << 24),
            All = KeyPressMask | KeyReleaseMask | ButtonPressMask | ButtonReleaseMask |
                EnterWindowMask | LeaveWindowMask | PointerMotionMask | PointerMotionHintMask |
                Button1MotionMask | Button2MotionMask | Button3MotionMask | Button4MotionMask | Button5MotionMask |
                ButtonMotionMask | KeymapStateMask | ExposureMask | VisibilityChangeMask |
                StructureNotifyMask | ResizeRedirectMask | ResizeRedirectMask | SubstructureNotifyMask |
                SubstructureRedirectMask | FocusChangeMask | PropertyChangeMask | ColormapChangeMask |
                OwnerGrabButtonMask
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct XEvent
        {
            [FieldOffset(0)]
            public XEventName type;
            [FieldOffset(0)]
            public XAnyEvent AnyEvent;
            [FieldOffset(0)]
            public XKeyEvent KeyEvent;
            [FieldOffset(0)]
            public XButtonEvent ButtonEvent;
            [FieldOffset(0)]
            public XMotionEvent MotionEvent;
            [FieldOffset(0)]
            public XCrossingEvent CrossingEvent;
            [FieldOffset(0)]
            public XFocusChangeEvent FocusChangeEvent;
            [FieldOffset(0)]
            public XExposeEvent ExposeEvent;
            [FieldOffset(0)]
            public XGraphicsExposeEvent GraphicsExposeEvent;
            [FieldOffset(0)]
            public XNoExposeEvent NoExposeEvent;
            [FieldOffset(0)]
            public XVisibilityEvent VisibilityEvent;
            [FieldOffset(0)]
            public XCreateWindowEvent CreateWindowEvent;
            [FieldOffset(0)]
            public XDestroyWindowEvent DestroyWindowEvent;
            [FieldOffset(0)]
            public XUnmapEvent UnmapEvent;
            [FieldOffset(0)]
            public XMapEvent MapEvent;
            [FieldOffset(0)]
            public XMapRequestEvent MapRequestEvent;
            [FieldOffset(0)]
            public XReparentEvent ReparentEvent;
            [FieldOffset(0)]
            public XConfigureEvent ConfigureEvent;
            [FieldOffset(0)]
            public XGravityEvent GravityEvent;
            [FieldOffset(0)]
            public XResizeRequestEvent ResizeRequestEvent;
            [FieldOffset(0)]
            public XConfigureRequestEvent ConfigureRequestEvent;
            [FieldOffset(0)]
            public XCirculateEvent CirculateEvent;
            [FieldOffset(0)]
            public XCirculateRequestEvent CirculateRequestEvent;
            [FieldOffset(0)]
            public XPropertyEvent PropertyEvent;
            [FieldOffset(0)]
            public XSelectionClearEvent SelectionClearEvent;
            [FieldOffset(0)]
            public XSelectionRequestEvent SelectionRequestEvent;
            [FieldOffset(0)]
            public XSelectionEvent SelectionEvent;
            [FieldOffset(0)]
            public XColormapEvent ColormapEvent;
            [FieldOffset(0)]
            public XClientMessageEvent ClientMessageEvent;
            [FieldOffset(0)]
            public XMappingEvent MappingEvent;
            [FieldOffset(0)]
            public XErrorEvent ErrorEvent;
            [FieldOffset(0)]
            public XKeymapEvent KeymapEvent;
            [FieldOffset(0)]
            public XEventPad Pad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XAnyEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XKeyEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr root;
            public IntPtr subwindow;
            public ulong time;
            public int x;
            public int y;
            public int x_root;
            public int y_root;
            public StateMask state;
            public uint keycode;
            public bool same_screen;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XButtonEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr root;
            public IntPtr subwindow;
            public ulong time;
            public int x;
            public int y;
            public int x_root;
            public int y_root;
            public uint state;
            public uint button;
            public sbyte same_screen;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XMotionEvent
        {
            public XEventName type;	
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr root;
            public IntPtr subwindow;
            public ulong time;
            public int x;
            public int y;
            public int x_root;
            public int y_root;
            public uint state;
            public byte is_hint;
            public bool same_screen;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XCrossingEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr root;
            public IntPtr subwindow;
            public ulong time;
            public int x;
            public int y;
            public int x_root;
            public int y_root;
            public CrossingMode mode;
            public CrossingDetail detail;
            public bool same_screen;
            public bool focus;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XFocusChangeEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public int mode;
            public CrossingDetail detail;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XKeymapEvent
        {
            public XEventName type;
            public IntPtr serial;
            public bool send_event;
            public IntPtr display;
            public IntPtr window;
            public byte key_vector0;
            public byte key_vector1;
            public byte key_vector2;
            public byte key_vector3;
            public byte key_vector4;
            public byte key_vector5;
            public byte key_vector6;
            public byte key_vector7;
            public byte key_vector8;
            public byte key_vector9;
            public byte key_vector10;
            public byte key_vector11;
            public byte key_vector12;
            public byte key_vector13;
            public byte key_vector14;
            public byte key_vector15;
            public byte key_vector16;
            public byte key_vector17;
            public byte key_vector18;
            public byte key_vector19;
            public byte key_vector20;
            public byte key_vector21;
            public byte key_vector22;
            public byte key_vector23;
            public byte key_vector24;
            public byte key_vector25;
            public byte key_vector26;
            public byte key_vector27;
            public byte key_vector28;
            public byte key_vector29;
            public byte key_vector30;
            public byte key_vector31;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XExposeEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public int x;
            public int y;
            public int width;
            public int height;
            public int count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XGraphicsExposeEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr drawable;
            public int x;
            public int y;
            public int width;
            public int height;
            public int count;
            public int major_code;
            public int minor_code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XNoExposeEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr drawable;
            public int major_code;
            public int minor_code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XVisibilityEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XCreateWindowEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr parent;
            public IntPtr window;
            public int x;
            public int y;
            public int width;
            public int height;
            public int border_width;
            public bool override_redirect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XDestroyWindowEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XUnmapEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public bool from_configure;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XMapEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public bool override_redirect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XMapRequestEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr parent;
            public IntPtr window;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XReparentEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public IntPtr parent;
            public int x;
            public int y;
            public bool override_redirect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XConfigureEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public int x;
            public int y;
            public int width;
            public int height;
            public int border_width;
            public IntPtr above;
            public bool override_redirect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XGravityEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XResizeRequestEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public int width;
            public int height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XConfigureRequestEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr parent;
            public IntPtr window;
            public int x;
            public int y;
            public int width;
            public int height;
            public int border_width;
            public IntPtr above;
            public int detail;
            public IntPtr value_mask;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XCirculateEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr xevent;
            public IntPtr window;
            public int place;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XCirculateRequestEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr parent;
            public IntPtr window;
            public int place;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XPropertyEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr atom;
            public ulong time;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XSelectionClearEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr selection;
            public ulong time;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XSelectionRequestEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr owner;
            public IntPtr requestor;
            public IntPtr selection;
            public IntPtr target;
            public IntPtr property;
            public ulong time;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XSelectionEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr requestor;
            public IntPtr selection;
            public IntPtr target;
            public IntPtr property;
            public ulong time;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XColormapEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr colormap;
            public bool c_new;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XClientMessageEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public IntPtr message_type;
            public int format;
            public IntPtr ptr1;
            public IntPtr ptr2;
            public IntPtr ptr3;
            public IntPtr ptr4;
            public IntPtr ptr5;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XMappingEvent
        {
            public XEventName type;
            public IntPtr serial;
            public sbyte send_event;
            public IntPtr display;
            public IntPtr window;
            public int request;
            public int first_keycode;
            public int count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XErrorEvent
        {
            public XEventName type;
            public IntPtr display;
            public IntPtr resourceid;
            public IntPtr serial;
            public byte error_code;
            public XRequest request_code;
            public byte minor_code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XEventPad
        {
            public IntPtr pad0;
            public IntPtr pad1;
            public IntPtr pad2;
            public IntPtr pad3;
            public IntPtr pad4;
            public IntPtr pad5;
            public IntPtr pad6;
            public IntPtr pad7;
            public IntPtr pad8;
            public IntPtr pad9;
            public IntPtr pad10;
            public IntPtr pad11;
            public IntPtr pad12;
            public IntPtr pad13;
            public IntPtr pad14;
            public IntPtr pad15;
            public IntPtr pad16;
            public IntPtr pad17;
            public IntPtr pad18;
            public IntPtr pad19;
            public IntPtr pad20;
            public IntPtr pad21;
            public IntPtr pad22;
            public IntPtr pad23;
        }

        public enum XEventName
        {
            KeyPress = 2,
            KeyRelease = 3,
            ButtonPress = 4,
            ButtonRelease = 5,
            MotionNotify = 6,
            EnterNotify = 7,
            LeaveNotify = 8,
            FocusIn = 9,
            FocusOut = 10,
            KeymapNotify = 11,
            Expose = 12,
            GraphicsExpose = 13,
            NoExpose = 14,
            VisibilityNotify = 15,
            CreateNotify = 16,
            DestroyNotify = 17,
            UnmapNotify = 18,
            MapNotify = 19,
            MapRequest = 20,
            ReparentNotify = 21,
            ConfigureNotify = 22,
            ConfigureRequest = 23,
            GravityNotify = 24,
            ResizeRequest = 25,
            CirculateNotify = 26,
            CirculateRequest = 27,
            PropertyNotify = 28,
            SelectionClear = 29,
            SelectionRequest = 30,
            SelectionNotify = 31,
            ColormapNotify = 32,
            ClientMessage = 33,
            MappingNotify = 34
        }

        public enum XRequest : byte
        {
            X_CreateWindow = 1,
            X_ChangeWindowAttributes = 2,
            X_GetWindowAttributes = 3,
            X_DestroyWindow = 4,
            X_DestroySubwindows = 5,
            X_ChangeSaveSet = 6,
            X_ReparentWindow = 7,
            X_MapWindow = 8,
            X_MapSubwindows = 9,
            X_UnmapWindow = 10,
            X_UnmapSubwindows = 11,
            X_ConfigureWindow = 12,
            X_CirculateWindow = 13,
            X_GetGeometry = 14,
            X_QueryTree = 15,
            X_InternAtom = 16,
            X_GetAtomName = 17,
            X_ChangeProperty = 18,
            X_DeleteProperty = 19,
            X_GetProperty = 20,
            X_ListProperties = 21,
            X_SetSelectionOwner = 22,
            X_GetSelectionOwner = 23,
            X_ConvertSelection = 24,
            X_SendEvent = 25,
            X_GrabPointer = 26,
            X_UngrabPointer = 27,
            X_GrabButton = 28,
            X_UngrabButton = 29,
            X_ChangeActivePointerGrab = 30,
            X_GrabKeyboard = 31,
            X_UngrabKeyboard = 32,
            X_GrabKey = 33,
            X_UngrabKey = 34,
            X_AllowEvents = 35,
            X_GrabServer = 36,
            X_UngrabServer = 37,
            X_QueryPointer = 38,
            X_GetMotionEvents = 39,
            X_TranslateCoords = 40,
            X_WarpPointer = 41,
            X_SetInputFocus = 42,
            X_GetInputFocus = 43,
            X_QueryKeymap = 44,
            X_OpenFont = 45,
            X_CloseFont = 46,
            X_QueryFont = 47,
            X_QueryTextExtents = 48,
            X_ListFonts = 49,
            X_ListFontsWithInfo = 50,
            X_SetFontPath = 51,
            X_GetFontPath = 52,
            X_CreatePixmap = 53,
            X_FreePixmap = 54,
            X_CreateGC = 55,
            X_ChangeGC = 56,
            X_CopyGC = 57,
            X_SetDashes = 58,
            X_SetClipRectangles = 59,
            X_FreeGC = 60,
            X_ClearArea = 61,
            X_CopyArea = 62,
            X_CopyPlane = 63,
            X_PolyPoint = 64,
            X_PolyLine = 65,
            X_PolySegment = 66,
            X_PolyRectangle = 67,
            X_PolyArc = 68,
            X_FillPoly = 69,
            X_PolyFillRectangle = 70,
            X_PolyFillArc = 71,
            X_PutImage = 72,
            X_GetImage = 73,
            X_PolyText8 = 74,
            X_PolyText16 = 75,
            X_ImageText8 = 76,
            X_ImageText16 = 77,
            X_CreateColormap = 78,
            X_FreeColormap = 79,
            X_CopyColormapAndFree = 80,
            X_InstallColormap = 81,
            X_UninstallColormap = 82,
            X_ListInstalledColormaps = 83,
            X_AllocColor = 84,
            X_AllocNamedColor = 85,
            X_AllocColorCells = 86,
            X_AllocColorPlanes = 87,
            X_FreeColors = 88,
            X_StoreColors = 89,
            X_StoreNamedColor = 90,
            X_QueryColors = 91,
            X_LookupColor = 92,
            X_CreateCursor = 93,
            X_CreateGlyphCursor = 94,
            X_FreeCursor = 95,
            X_RecolorCursor = 96,
            X_QueryBestSize = 97,
            X_QueryExtension = 98,
            X_ListExtensions = 99,
            X_ChangeKeyboardMapping = 100,
            X_GetKeyboardMapping = 101,
            X_ChangeKeyboardControl = 102,
            X_GetKeyboardControl = 103,
            X_Bell = 104,
            X_ChangePointerControl = 105,
            X_GetPointerControl = 106,
            X_SetScreenSaver = 107,
            X_GetScreenSaver = 108,
            X_ChangeHosts = 109,
            X_ListHosts = 110,
            X_SetAccessControl = 111,
            X_SetCloseDownMode = 112,
            X_KillClient = 113,
            X_RotateProperties = 114,
            X_ForceScreenSaver = 115,
            X_SetPointerMapping = 116,
            X_GetPointerMapping = 117,
            X_SetModifierMapping = 118,
            X_GetModifierMapping = 119,
            X_NoOperation = 127
        }

        public enum CrossingDetail : int
        {
            NotifyAncestor = 0,
            NotifyVirtual = 1,
            NotifyInferior = 2,
            NotifyNonlinear = 3,
            NotifyNonlinearVirtual = 4,
            NotifyPointer = 5,
            NotifyPointerRoot = 6,
            NotifyDetailNone = 7

        }

        public enum CrossingMode : int
        {
            NotifyNormal = 0,
            NotifyGrab = 1,
            NotifyUngrab = 2,
            NotifyWhileGrabbed = 3

        }

        public enum StateMask : uint
        {
            ShiftMask = (1 << 0),
            LockMask = (1 << 1),
            ControlMask = (1 << 2),
            Mod1Mask = (1 << 3),
            Mod2Mask = (1 << 4),
            Mod3Mask = (1 << 5),
            Mod4Mask = (1 << 6),
            Mod5Mask = (1 << 7),
            Button1Mask = (1 << 8),
            Button2Mask = (1 << 9),
            Button3Mask = (1 << 10),
            Button4Mask = (1 << 11),
            Button5Mask = (1 << 12),
            AnyModifier = (1 << 15)
        }
    }
}
