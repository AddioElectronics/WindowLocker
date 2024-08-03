using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace WindowLocker
{
    public static class Win32
    {
        public const int GWL_STYLE = -16;
        public const long WS_CAPTION = 0x00C00000;
        public const long WS_SIZEBOX = 0x00040000;
        public const long WS_MAXIMIZEBOX = 0x00010000L;
        public const long WS_MINIMIZEBOX = 0x00020000L;
        public const long WS_THICKFRAME = 0x00040000;

        public enum GWL
        {
            EXSTYLE = -20,
            HINSTANCE = -6,
            ID = -12,
            STYLE = -16,
            USERDATA = -21,
            WNDPROC = -4
        }

        public const long DWLP_MSGRESULT = 0;
        public const long DWLP_DLGPROC = DWLP_MSGRESULT + sizeof(int); // assuming LRESULT is of size int
        public static readonly long DWLP_USER = DWLP_DLGPROC + IntPtr.Size; // assuming DLGPROC is a pointer

        public const int VK_LBUTTON = 0x01; // Left mouse button
        public const int VK_RBUTTON = 0x02; // Right mouse button

        // Constants for window events
        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        private const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        public enum EventType : uint
        {
            SystemForeground = 0x0003,
            MoveSizeStart = 0x000A,
            MoveSizeEnd = 0x000B,
            MinimizeStart = 0x0016,
            MinimizeEnd = 0x0017,
            ObjectDestroy = 0x8001,
            LocationChange = 0x800B
        }

        // Constants for ShowWindow function
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;

        public enum ShowWindowOptions
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            ShowNoActive = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        // Constants for SetWindowPos function
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public enum SWP : uint
        {
            NoSize = 0x0001,
            NoMove = 0x0002,
            NoZOrder = 0x0004,
            NoActivate = 0x0010,
            FrameChanged = 0x0020,
            ShowWindow = 0x0040,
        }

        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = IntPtr.Zero;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        /// <summary>
        /// Callback delegate for the <see cref="SetWinEventHook"/> function.
        /// </summary>
        public delegate void WinEventDelegate(IntPtr hWinEventHook, EventType eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(EventType eventMin, EventType eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);


        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, GWL nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowOptions nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            // Get the length of the window text
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
            {
                return string.Empty;
            }

            // Allocate a buffer to hold the window title
            StringBuilder sb = new StringBuilder(length + 1);

            // Get the window text
            if (GetWindowText(hWnd, sb, sb.Capacity) > 0)
            {
                return sb.ToString();
            }

            return string.Empty;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static uint GetProcessIdFromHwnd(IntPtr hWnd)
        {
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            return processId;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static bool IsConsoleWindow(IntPtr hWnd)
        {
            // Check if the window is a console window (e.g., by checking for specific styles or titles)
            // Implement your own logic here to distinguish console windows
            return GetParent(hWnd) == IntPtr.Zero; // Example: No parent could be a console window
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] SplitArgs(string commandLine)
        {
            IntPtr argv = CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            try
            {
                string[] args = new string[argc];
                for (int i = 0; i < args.Length; i++)
                {
                    IntPtr p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p)!;
                }
                return args;
            }
            finally
            {
                LocalFree(argv);
            }
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr hMem);

        public static string? TryGetProcessFileName(uint pid)
        {
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                // Open the process to query information
                hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);

                if (hProcess == IntPtr.Zero)
                {
                    return null;
                }

                // Query the full process image name
                StringBuilder exePath = new StringBuilder(1024);
                uint size = (uint)exePath.Capacity;
                if (QueryFullProcessImageName(hProcess, 0, exePath, ref size))
                {
                    return exePath.ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (hProcess != IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // Define the RECT structure
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT : IEquatable<RECT>   
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;


        public int Width => int.Abs(Right - Left);
        public int Height => int.Abs(Bottom - Top);

        public static readonly RECT Default = default;

        public bool Equals(RECT other)
        {
            return this.Left == other.Left
                && this.Top == other.Top
                && this.Right == other.Right
                && this.Bottom == other.Bottom;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj is RECT rect)
            {
                return Equals(rect);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Left.GetHashCode() ^ Top.GetHashCode())
                * (Right.GetHashCode() ^ Bottom.GetHashCode());
        }

        public static bool operator ==(RECT left, RECT right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RECT left, RECT right)
        {
            return !left.Equals(right);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }
}
