using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using WindowLocker.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowLocker.Win32;

namespace WindowLocker
{
    // TODO: Disable ability to move, minimize, or maximize
    // TODO: When settings rect, need to check if its in window bounds instead of min max.
    public class LockedWindow : IDisposable, INotifyPropertyChanged
    {
        private static readonly int MaxX;
        private static readonly int MaxY;
        private static readonly int MaxWidth;
        private static readonly int MaxHeight;

        private static readonly int MinX;
        private static readonly int MinY;
        private const int MinWidth = 3;
        private const int MinHeight = 5;

        static LockedWindow()
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            MaxWidth = screens.Max(x => x.Bounds.Width);
            MaxHeight = screens.Max(x => x.Bounds.Height);

            MinX = screens.Min(x => x.Bounds.Left);
            MinY = screens.Min(x => x.Bounds.Top);

            MaxX = screens.Max(x => x.Bounds.Right);
            MaxY = screens.Max(x => x.Bounds.Bottom);
        }

        public event EventHandler? WindowClosed;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IntPtr _winRectEventHandle;
        private readonly IntPtr _winDestroyEventHandle;

        private RECT _position;
        private bool _locked;
        private bool _allowMinimize;
        private bool _hideTitleBar;
        private bool _isSelected;
        private bool _isConsole;
        private bool disposedValue;
        private IntPtr _windowStyle;

        public LockedWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(hWnd), "Window handle null.");
            }

            this.HWND = hWnd;
            this.Title = Win32.GetWindowTitle(hWnd);
            this.Pid = Win32.GetProcessIdFromHwnd(HWND);

            _isConsole = Win32.IsConsoleWindow(hWnd);

            if (_isConsole)
            {
                //// Get conhost.exe pid for win event hooks.
                //this.Pid = (uint)ProcessUtility.GetParentProcessID((int)this.Pid);
            }

            // TODO: Does not work for all windows when Pid is passed, 0 crashes. Using timer for now.
            //RegisterWinEventHooks(out _winRectEventHandle, out _winDestroyEventHandle);

            CapturePosition();
            _windowStyle = Win32.GetWindowLongPtr(hWnd, GWL.STYLE);
            PropertyChanged += LockedWindow_PropertyChanged;
        }

        public LockedWindow(Process process)
            : this(process.MainWindowHandle)
        {

        }

        ~LockedWindow()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        #region Properties

        public IntPtr HWND { get; }
        public uint Pid { get; }
        public string Title { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {                    
                    if (value)
                    {
                        _locked = CapturePosition();
                    }
                    else
                    {
                        _locked = value;
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locked)));
                }
            }
        }

        public bool HideTitleBar
        {
            get => _hideTitleBar;
            set
            {
                if (_hideTitleBar != value)
                {
                    _hideTitleBar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HideTitleBar)));
                }
            }
        }

        public bool AllowMinimize
        {
            get => _allowMinimize;
            set
            {
                if (_allowMinimize != value)
                {
                    _allowMinimize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowMinimize)));
                }
            }
        }

        public RECT Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    ResetWindowRect();
                }
            }
        }

        public int X
        {
            get => _position.Left;
            set
            {
                if (!IsInsideScreenBounds(value, Y))
                {
                    SystemSounds.Beep.Play();
                    return;
                }    

                if (_position.Left != value)
                {
                    int difference = value - _position.Left;
                    _position.Left = value;
                    _position.Right += difference;
                    ResetWindowRect();
                }
            }
        }
        public int Y
        {
            get => _position.Top;
            set
            {
                if (!IsInsideScreenBounds(X, value))
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                if (_position.Top != value)
                {
                    int difference = value - _position.Top;
                    _position.Top = value;
                    _position.Bottom += difference;
                    ResetWindowRect();
                }
            }
        }
        public int Left
        {
            get => _position.Left;
            set
            {
                if (!IsInsideScreenBounds(value, Y))
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                if (_position.Left != value)
                {
                    _position.Left = value;
                    ResetWindowRect();
                }
            }
        }
        public int Top
        {
            get => _position.Top;
            set
            {
                if (!IsInsideScreenBounds(X, value))
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                if (_position.Top != value)
                {
                    _position.Top = value;
                    ResetWindowRect();
                }
            }
        }
        public int Right
        {
            get => _position.Right;
            set
            {
                if (!IsInsideScreenBounds(value, Y))
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                if (_position.Right != value)
                {
                    _position.Right = value;
                    ResetWindowRect();
                }
            }
        }
        public int Bottom
        {
            get => _position.Bottom;
            set
            {
                if (!IsInsideScreenBounds(X, value))
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                if (_position.Bottom != value)
                {
                    _position.Bottom = value;
                    ResetWindowRect();
                }
            }
        }
        public int Width
        {
            get => _position.Width;
            set
            {
                if (value < MinWidth || value > MaxWidth)
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                int right = _position.Left + value;

                if (_position.Right != right)
                {
                    _position.Right = right;
                    ResetWindowRect();
                }
            }
        }
        public int Height
        {
            get => _position.Height;
            set
            {
                if (value < MinHeight || value > MaxHeight)
                {
                    SystemSounds.Beep.Play();
                    return;
                }

                int bottom = _position.Top + value;
                if (_position.Bottom != bottom)
                {
                    _position.Bottom = bottom;
                    ResetWindowRect();
                }
            }
        }

        #endregion

        #region Functions

        private void LockedWindow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Locked)
            {
                if (HideTitleBar)
                {
                    // Hide title bar
                    SetWindowLongPtr(HWND, GWL_STYLE, new IntPtr(_windowStyle.ToInt64() & ~WS_CAPTION & ~WS_SIZEBOX));
                }
                else
                {
                    // Disable maximize and minimize buttons
                    SetWindowLongPtr(HWND, GWL_STYLE, new IntPtr(_windowStyle.ToInt64() & ~WS_THICKFRAME & ~WS_MAXIMIZEBOX & (AllowMinimize ? ~WS_MINIMIZEBOX : ~0L)));
                }
            }
            else
            {
                // Restore original window style
                SetWindowLongPtr(HWND, GWL_STYLE, new IntPtr(_windowStyle.ToInt64()));
            }


            SetWindowPos(HWND, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP.NoMove | SWP.NoSize | SWP.FrameChanged);
        }

        private void RegisterWinEventHooks(out IntPtr moveEventHandle, out IntPtr destroyEventHandle)
        {
            throw new NotSupportedException("SetWinEventHook causes crashes in WPF applications.");
            // TODO: Does not work for all windows when Pid is passed, 0 crashes. Using timer for now.
            //       For console applications you must get Pid of conhost.exe, but there may be more than 1 conhost running, making it tricky.

            moveEventHandle = SetWinEventHook(
               EventType.LocationChange,
               EventType.LocationChange,
               IntPtr.Zero,
               HandleMoveWinEvent,
               Pid,
               0,
               Win32.WINEVENT_OUTOFCONTEXT);

            destroyEventHandle = SetWinEventHook(
               EventType.ObjectDestroy,
               EventType.ObjectDestroy,
               IntPtr.Zero,
               HandleDestroyWinEvent,
               Pid,
               0,
               Win32.WINEVENT_OUTOFCONTEXT);
        }

        private bool IsInsideScreenBounds(int x, int y)
        {
            return Screen.AllScreens.Any(screen => screen.Bounds.Contains(x, y));
        }

        private bool CapturePosition()
        {
            if (Win32.IsIconic(this.HWND))
            {
                return false;
            }

            if (Win32.GetWindowRect(this.HWND, out var rect))
            {
                Position = rect;
                return true;
            }
            else
            {
                Position = RECT.Default;
                return false;
            }
        }

        public void ResetWindowRect()
        {
            if (!Win32.IsWindow(HWND))
            {
                Dispose();
                WindowClosed?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (!Locked || Position.Equals(default))
            {
                return;
            }

            if (Win32.GetWindowRect(this.HWND, out RECT rect) && rect != Position)
            {
                Win32.SetWindowPos(HWND, IntPtr.Zero, Position.Left, Position.Top, Position.Width, Position.Height, SWP.ShowWindow);
            }

            if (!AllowMinimize)
            {
                if (IsIconic(HWND))
                {
                    Win32.ShowWindow(HWND, ShowWindowOptions.Restore);
                }
            }
        }

        private void HandleDestroyWinEvent(IntPtr hWinEventHook, EventType eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != HWND)
            {
                return;
            }

            Dispose();
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }


        private void HandleMoveWinEvent(IntPtr hWinEventHook, EventType eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (!Locked || Position.Equals(default) || hwnd != HWND)
            {
                return;
            }

            switch (eventType)
            {
                case EventType.LocationChange:
                    Win32.ShowWindow(HWND, ShowWindowOptions.Restore);
                    ResetWindowRect();
                    break;

                case EventType.MoveSizeStart:
                case EventType.MoveSizeEnd:
                    ResetWindowRect();
                    break;
                case EventType.MinimizeStart:
                case EventType.MinimizeEnd:
                    if (AllowMinimize)
                    {
                        return;
                    }

                    Win32.ShowWindow(HWND, ShowWindowOptions.Restore);
                    goto case EventType.MoveSizeStart;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                if (_winRectEventHandle != IntPtr.Zero)
                    Win32.UnhookWinEvent(_winRectEventHandle);

                if (_winDestroyEventHandle != IntPtr.Zero)
                    Win32.UnhookWinEvent(_winDestroyEventHandle);
                
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
