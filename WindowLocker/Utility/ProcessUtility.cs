using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace WindowLocker.Utility
{
    public static class ProcessUtility
    {

        public static Process? TryGetProcessFromHWND(IntPtr hwnd)
        {
            try
            {
                uint pid = Win32.GetProcessIdFromHwnd(hwnd);
                return Process.GetProcessById((int)pid);
            }
            catch { return null; }
        }

        public static string[]? GetProcessArgumentsArray(int pid)
        {
            string? arguments = GetProcessArguments(pid);

            if (arguments == null)
            {
                return null;
            }

            return Win32.SplitArgs(arguments);
        }

        public static string? GetProcessArguments(int pid)
        {
            string? arguments = null;
            string query = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {pid}";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string? arg = obj["CommandLine"].ToString();

                    if (arg != null)
                    {
                        arguments = arg;
                        break;
                    }
                }
            }

            return arguments;
        }

        /// <summary>
        /// Enumerates through all running processes and finds the parent to the process with <paramref name="pid"/>.
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns>ID of the process parent to <paramref name="pid"/>, or 0 if the process does not have a parent. Can also return -1 if enumeration has failed.</returns>
        public static int GetParentProcessID(int pid)
        {
            int parentPID = 0;

            nint snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (snapshot == nint.Zero)
                return -1;

            PROCESSENTRY32 procEntry = new PROCESSENTRY32()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
            };

            if (Process32First(snapshot, ref procEntry) == false)
                return -1;

            do
            {
                if (pid == procEntry.th32ProcessID)
                    parentPID = (int)procEntry.th32ParentProcessID;
            }
            while (parentPID == 0 && Process32Next(snapshot, ref procEntry));

            return parentPID;
        }

        /// <summary>
        /// Enumerates through all running processes and finds the parent processes for all <paramref name="pids"/>.
        /// </summary>
        /// <param name="pids">Array of process IDs</param>
        /// <param name="parents">Array of the parent IDs</param>
        /// <returns>True if enumeration was a success, false if enumeration failed.</returns>
        public static bool GetParentProcessIDs(int[] pids, out int[] parents)
        {
            int counter = 0;
            parents = new int[pids.Length];

            nint snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (snapshot == nint.Zero)
                return false;

            PROCESSENTRY32 procEntry = new PROCESSENTRY32()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
            };

            if (Process32First(snapshot, ref procEntry) == false)
                return false;

            do
            {
                for (int i = 0; i < pids.Length; i++)
                {
                    if (pids[i] == procEntry.th32ProcessID)
                    {
                        parents[i] = (int)procEntry.th32ParentProcessID;
                        counter++;
                    }
                }
            }
            while (counter < pids.Length && Process32Next(snapshot, ref procEntry));

            return true;
        }

        /// <summary>
        /// Enumerates through all running processes searching for the process with <paramref name="pid"/>,
        /// and returns the <see cref="PROCESSENTRY32"/> of the process.
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="processEntry">Information about the process</param>
        /// <returns>True if the process was found. False if it was not found or if enumeration failed.</returns>
        public static bool GetProcessEntry(int pid, out PROCESSENTRY32 processEntry)
        {
            int parentPID = 0;
            processEntry = default;

            nint snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (snapshot == nint.Zero)
                return false;

            PROCESSENTRY32 procEntry = new PROCESSENTRY32()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
            };

            if (Process32First(snapshot, ref procEntry) == false)
                return false;

            do
            {
                if (pid == procEntry.th32ProcessID)
                {
                    processEntry = procEntry;
                    return true;
                }
            }
            while (parentPID == 0 && Process32Next(snapshot, ref procEntry));

            return false;
        }

        static uint TH32CS_SNAPPROCESS = 2;

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public nint th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(nint hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(nint hSnapshot, ref PROCESSENTRY32 lppe);
    }
}
