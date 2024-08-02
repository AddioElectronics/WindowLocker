using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static WindowLocker.Win32;
using Timer = System.Timers.Timer;

namespace WindowLocker
{
    public class WindowManager : ICollection<LockedWindow>, ICollection<IntPtr>
    {
        private readonly Dictionary<IntPtr, LockedWindow> _windows = new Dictionary<nint, LockedWindow>();

        object _lock = new object();
        Timer _timer;

        public WindowManager()
        {
            _timer = new Timer(1);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public int Count => _windows.Count;

        public bool IsReadOnly => false;

        //private void RegisterWinEventHook()
        //{
        //    IntPtr hWinEventHook = SetWinEventHook(
        //        EventType.MoveSizeStart,    // EventMin
        //        EventType.MinimizeEnd,      // EventMax
        //        IntPtr.Zero,                // hModWinEventProc
        //        HandleWinEvent,             // pfnWinEventProc
        //        0,                          // idProcess (0 means all processes)
        //        0,                          // idThread (0 means all threads)
        //        Win32.WINEVENT_OUTOFCONTEXT       // dwFlags
        //);
        //}

        //private void HandleWinEvent(IntPtr hWinEventHook, EventType eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        //{
        //    if (_windows.TryGetValue(hwnd, out var windowHandle))
        //    {
        //        windowHandle.HandleWinEvent(hWinEventHook, eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
        //    }
        //}

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
#warning SetWinEventHook crashing, using Timer for now.
            lock (_lock)
            {
                foreach (var window in _windows.Values)
                {
                    window.ResetWindowRect();
                }
            }
        }

        #region ICollection<LockedWindow>

        public void Add(LockedWindow item)
        {
            if (!_windows.ContainsKey(item.HWND))
            {
                lock (_lock)
                {
                    _windows.Add(item.HWND, item);
                }

                item.WindowClosed += WindowClosed;
            }
        }

        private void WindowClosed(object? sender, EventArgs e)
        {
            LockedWindow item = (LockedWindow)sender!;
            Remove(item);
        }

        public void Clear()
        {
            lock (_lock)
            {
                _windows.Clear();
            }
        }

        public bool Contains(LockedWindow item)
        {
            return _windows.ContainsKey(item.HWND);
        }

        public void CopyTo(LockedWindow[] array, int arrayIndex)
        {
            _windows.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<LockedWindow> GetEnumerator()
        {
            return _windows.Values.GetEnumerator();
        }

        public bool Remove(LockedWindow item)
        {
            lock (_lock)
            {
                return _windows.Remove(item.HWND);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _windows.Values.GetEnumerator();
        }

        #endregion ICollection<LockedWindow>

        #region ICollection<IntPtr>

        void ICollection<nint>.Add(nint hWnd)
        {
            var window = new LockedWindow(hWnd);
            Add(window);
        }

        bool ICollection<nint>.Contains(nint hWnd)
        {
            return _windows.ContainsKey(hWnd);
        }

        void ICollection<nint>.CopyTo(nint[] array, int arrayIndex)
        {
            _windows.Keys.CopyTo(array, arrayIndex);
        }

        IEnumerator<nint> IEnumerable<nint>.GetEnumerator()
        {
            return _windows.Keys.GetEnumerator();
        }

        bool ICollection<nint>.Remove(nint hWnd)
        {
            lock (_lock)
            {
                return _windows.Remove(hWnd);
            }
        }
        #endregion ICollection<IntPtr>
    }
}
