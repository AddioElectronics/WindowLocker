using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using static WindowLocker.Win32;
using System.Diagnostics;
using WindowLocker.Utility;

namespace WindowLocker
{
    public struct WindowInfo
    {
        public string title;
        public IntPtr hwnd;
        public uint pid;
    }

    public class FindWindowHelper
    {
        private static readonly Process Explorer = Process.GetProcessesByName("explorer").First();


        public event EventHandler<WindowInfo>? HoverWindow;
        public event EventHandler<WindowInfo>? SelectedWindow;


        private Timer? _timer;
        private WindowInfo _lastWindowInfo;
        private IntPtr _lastHWnd;

        public FindWindowHelper() { }

        public void StartSearching()
        {
            if (_timer != null)
            {
                return;
            }

            _timer = new Timer(10);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public void StopSearching()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private static bool IsExplorer(IntPtr hWnd)
        {
            int pid = (int)Win32.GetProcessIdFromHwnd(hWnd);

            using (Process process = Process.GetProcessById(pid))
            {
                string title = Win32.GetWindowTitle(hWnd);
                return title == string.Empty && process.Id == Explorer.Id && process.ProcessName == Explorer.ProcessName && process.MainWindowHandle == Explorer.MainWindowHandle && process.MainWindowTitle == Explorer.MainWindowTitle;
            }
        }

        private IntPtr GetRootWindow(IntPtr hWnd)
        {
            IntPtr hWndParent = hWnd;
            IntPtr hwndRoot = hWnd;

            while (hWndParent != IntPtr.Zero)
            {
                hwndRoot = hWndParent;
                hWndParent = Win32.GetParent(hWnd);

                if (hWndParent == hwndRoot)
                    break;
            }
            return hwndRoot;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // TODO: Use LL Keyboard hook?

            _timer!.Stop();
            IntPtr hWnd = GetWindowAtMousePosition();

            if (IsExplorer(hWnd))
            {
                hWnd = IntPtr.Zero;
            }

            WindowInfo windowInfo = default;

            if (hWnd != IntPtr.Zero)
            {
                windowInfo = hWnd != _lastHWnd
                    ? new WindowInfo()
                    {
                        hwnd = hWnd,
                        title = Win32.GetWindowTitle(hWnd),
                        pid = Win32.GetProcessIdFromHwnd(hWnd),
                    }
                    : _lastWindowInfo;

                HoverWindow?.Invoke(this, windowInfo);
                _lastHWnd = hWnd;
                _lastWindowInfo = windowInfo;
            }
            else
            {
                if (_lastHWnd != IntPtr.Zero)
                {
                    _lastHWnd = IntPtr.Zero;
                    _lastWindowInfo = default;
                    HoverWindow?.Invoke(this, default);
                }
            }

            if (!IsMouseButtonDown(VK_LBUTTON))
            {
                _timer!.Stop();
                _timer.Dispose();
                _timer = null;

                SelectedWindow?.Invoke(this, _lastWindowInfo);
            }
            else
            {
                _timer.Start();
            }
        }

        private bool IsMouseButtonDown(int vKey)
        {
            return (GetAsyncKeyState(vKey) & 0x8000) != 0;
        }

        private IntPtr GetWindowAtMousePosition()
        {
            if (Win32.GetCursorPos(out POINT pt))
            {
                return GetRootWindow(Win32.WindowFromPoint(pt));
            }
            return IntPtr.Zero;
        }
    }
}
