using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WindowLocker.Utility;

namespace Xorrupt.Windows.Forms
{
    // TODO: Clean up. Made some quick changes and left it in a mes.
    public partial class OpenProcessDialog : Form
    {
        #region Fields / Properties

        private ListTypeFlag _activeLists;

        public Process? Process { get; private set; }

        private Process[]? _processes;
        private Process[]? _processProcesses;
        private Process[]? _applicationProcesses;
        private Process[]? _windowProcesses;

        //private Dictionary<Process, ListViewItemProcess> _listViewItems;

        private ListViewItemProcess[]? _processItems;
        private ListViewItemProcess[]? _applicationItems;
        private ListViewItemProcess[]? _windowItems;

        private ListViewItemProcess[]? _filteredApplicationItems;
        private ListViewItemProcess[]? _filteredProcessItems;
        private ListViewItemProcess[]? _filteredWindowItems;

        private Ordering _processOrdering;
        public Ordering ProcessOrdering
        {
            get => _processOrdering;
            set
            {
                _processOrdering = value;
                OrderLists();
                FilterLists();
            }
        }

        private bool ListingProcesses => _activeLists.HasFlag(ListTypeFlag.Processes);
        private bool ListingApplications => _activeLists.HasFlag(ListTypeFlag.Applications);
        private bool ListingWindows => _activeLists.HasFlag(ListTypeFlag.Windows);

        #endregion

        #region Constructors

        public OpenProcessDialog(ListTypeFlag activeLists, bool canCreateProcess = true)
        {
            _activeLists = activeLists;
            InitializeComponent();

            ListViewItemProcess.PidHex = pIDHEXToolStripMenuItem.Checked;

            if (!ListingProcesses)
            {
                tabControl_Processes.TabPages.RemoveAt(2);
            }

            if (!ListingWindows)
            {
                tabControl_Processes.TabPages.RemoveAt(1);
            }

            if (!ListingApplications)
            {
                tabControl_Processes.TabPages.RemoveAt(0);
            }

            if (!canCreateProcess)
            {
                createProcessToolStripMenuItem.Visible = false;
            }

            Task task = new Task(() => GetProcessList());
            task.Start();
        }

        public OpenProcessDialog()
            : this(ListTypeFlag.Processes | ListTypeFlag.Applications | ListTypeFlag.Windows)
        {
        }

        ~OpenProcessDialog()
        {
            openFileDialog_Exe.Dispose();
            imageList_Icons.Dispose();

            DisposeProcesses();
        }

        #endregion

        #region Functions

        private void UpdateActiveLists()
        {
            listView_Windows.Enabled = _activeLists.HasFlag(ListTypeFlag.Windows);
            listView_Processes.Enabled = _activeLists.HasFlag(ListTypeFlag.Processes);
            listView_Applications.Enabled = _activeLists.HasFlag(ListTypeFlag.Applications);
        }

        private void GetProcessList()
        {
            _processes = Process.GetProcesses();


            int[] pids = _processes.Select(x => x.Id).ToArray();
            int[] parentIds = new int[_processes.Length];
            ProcessUtility.GetParentProcessIDs(pids, out parentIds);

            List<Process>? processProcesses = ListingProcesses ? new List<Process>() : null;
            List<Process>? applicationProcesses = ListingApplications ? new List<Process>() : null;
            List<Process>? windowProcesses = ListingWindows ? new List<Process>() : null;

            for (int i = 0; i < _processes.Length; i++)
            {
                string? path;
                string title;

                try
                {
                    path = _processes[i].MainModule?.FileName;
                    title = _processes[i].MainWindowTitle;
                }
                catch { }


                if (ListingProcesses)
                    if (parentIds[i] > 0 && _processes[i].MainWindowHandle == IntPtr.Zero)
                    {
                        processProcesses!.Add(_processes[i]);
                    }

                if (ListingApplications)
                    if (parentIds[i] == 0 || _processes[i].MainWindowHandle != IntPtr.Zero)
                    {
                        applicationProcesses!.Add(_processes[i]);
                    }

                if (ListingWindows)
                    if (_processes[i].MainWindowHandle != IntPtr.Zero)
                    {
                        windowProcesses!.Add(_processes[i]);
                    }
            }

            if (ListingProcesses)
            {
                _processProcesses = processProcesses!.ToArray();
                _processItems = _processProcesses.Select(x => new ListViewItemProcess(x, imageList_Icons)).ToArray();
            }

            if (ListingApplications)
            {
                _applicationProcesses = applicationProcesses!.ToArray();
                _applicationItems = _applicationProcesses.Select(x => new ListViewItemProcess(x, imageList_Icons)).ToArray();
            }

            if (ListingWindows)
            {
                _windowProcesses = windowProcesses!.ToArray();
                _windowItems = _windowProcesses.Select(x => new ListViewItemProcess(x, imageList_Icons)).ToArray();
            }

            //bool allUsed = _processes.All(x => _processProcesses.Contains(x) || _applicationProcesses.Contains(x) || _windowProcesses.Contains(x));

            //if (!allUsed)
            //{
            //    throw new Exception("Not all processes are accessible.");
            //}

            OrderLists();
            SetLists();
        }

        private void SetProcessList(ListView listView, ListViewItemProcess[] items)
        {
            listView.SuspendLayout();
            listView.Items.Clear();
            listView.Items.AddRange(items);
            listView.ResumeLayout();
        }

        private void DisposeProcesses()
        {
            if (_processes != null)
            {
                foreach (Process process in _processes)
                {
                    if (process != Process)
                        process.Dispose();
                }
                _processes = null;
            }
        }

        private void SetLists()
        {
            this.Invoke(new Action(() =>
            {
                if (ListingApplications)
                {
                    Debug.Assert(_processItems != null);
                    SetProcessList(listView_Processes, _processItems);
                }
                if (ListingProcesses)
                {
                    Debug.Assert(_applicationItems != null);
                    SetProcessList(listView_Applications, _applicationItems);
                }
                if (ListingWindows)
                {
                    Debug.Assert(_windowItems != null);
                    SetProcessList(listView_Windows, _windowItems);
                }
            }));
        }

        private void SetFilteredLists()
        {
            if (ListingApplications)
            {
                Debug.Assert(_filteredProcessItems != null);
                SetProcessList(listView_Processes, _filteredProcessItems);
            }
            if (ListingProcesses)
            {
                Debug.Assert(_filteredApplicationItems != null);
                SetProcessList(listView_Applications, _filteredApplicationItems);
            }
            if (ListingWindows)
            {
                Debug.Assert(_filteredWindowItems != null);
                SetProcessList(listView_Windows, _filteredWindowItems);
            }
        }

        private void FilterList(ListView listView, Regex filter, ListViewItemProcess[] items, ref ListViewItemProcess[]? filteredItems)
        {
            listView.SuspendLayout();
            filteredItems = items.AsParallel().Where(x => filter.IsMatch(x.ProcessName) || filter.IsMatch(x.Text)).ToArray();
            SetProcessList(listView, filteredItems);
            listView.ResumeLayout();
        }

        private void FilterLists()
        {
            tabControl_Processes.SuspendLayout();

            string filter = textBox_Filter.Text;

            if (filter == String.Empty)
            {
                SetLists();
                tabControl_Processes.ResumeLayout();
                return;
            }

            RegexOptions options =
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.Compiled;

            Regex regex = new Regex(filter, options);

            if (ListingApplications)
            {
                Debug.Assert(_applicationItems != null);
                FilterList(listView_Applications, regex, _applicationItems, ref _filteredApplicationItems);
            }

            if (ListingProcesses)
            {
                Debug.Assert(_processItems != null);
                FilterList(listView_Processes, regex, _processItems, ref _filteredProcessItems);
            }

            if (ListingWindows)
            {
                Debug.Assert(_windowItems != null);
                FilterList(listView_Windows, regex, _windowItems, ref _filteredWindowItems);
            }

            tabControl_Processes.ResumeLayout();
        }

        private ListViewItemProcess[] OrderByName(ListViewItemProcess[] items)
        {
            return items.OrderBy(x => x.ProcessName).ToArray();
        }

        private ListViewItemProcess[] OrderByAddress(ListViewItemProcess[] items)
        {
            return items.OrderBy(x => x.ProcessID).ToArray();
        }

        private ListViewItemProcess[] OrderProcesses(ListViewItemProcess[] items, Ordering order)
        {
            return order switch
            {
                Ordering.Name => OrderByName(items),
                Ordering.Address => OrderByAddress(items),
                _ => items
            };
        }

        private void OrderLists()
        {
            this.Invoke(new Action(() =>
            {
                if (ListingProcesses)
                {
                    Debug.Assert(_processItems != null);
                    _processItems = OrderProcesses(_processItems, ProcessOrdering);
                }
                if (ListingWindows)
                {
                    Debug.Assert(_windowItems != null);
                    _windowItems = OrderProcesses(_windowItems, ProcessOrdering);
                }
                if (ListingApplications)
                {
                    Debug.Assert(_applicationItems != null);
                    _applicationItems = OrderProcesses(_applicationItems, ProcessOrdering);
                }
            }));
        }

        #endregion

        #region Event Functions

        private void pIDHEXToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ListViewItemProcess.PidHex = pIDHEXToolStripMenuItem.Checked;
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            GetProcessList();
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            int tabIndex = tabControl_Processes.SelectedIndex;

            ListView activeList = (tabControl_Processes.TabPages[tabIndex].Controls[0] as ListView)!;

            if (activeList.SelectedItems.Count == 0)
            {
                System.Media.SystemSounds.Exclamation.Play();
                activeList.Focus();
                return;
            }

            ListViewItemProcess item = (activeList.SelectedItems[0] as ListViewItemProcess)!;
            Process = item.Process;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void createProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog_Exe.ShowDialog();

            if (result != DialogResult.OK)
                return;

            string path = openFileDialog_Exe.FileName;

            Process = new Process();
            Process.Start(path);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBox_Filter_TextChanged(object sender, EventArgs e)
        {
            FilterLists();
        }

        private void listView_Processes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _processOrdering = (Ordering)e.Column;
            OrderLists();
            FilterLists();
        }

        #endregion

        #region Definitions

        [Flags]
        public enum ListTypeFlag
        {
            Applications = (1 << 0),
            Windows = (1 << 1),
            Processes = (1 << 2)
        }

        public enum Ordering
        {
            Name = 0,
            Address = 1
        }

        internal class ListViewItemProcess : ListViewItem
        {
            internal static event EventHandler? PidHexChanged;

            private static bool s_pidHex;
            public static bool PidHex
            {
                get => s_pidHex;
                set
                {
                    if (s_pidHex != value)
                    {
                        s_pidHex = value;
                        PidHexChanged?.Invoke(null, EventArgs.Empty);
                    }

                }
            }

            private ListViewSubItem _nameItem;

            public Process Process { get; set; }

            public string ProcessName => Process.ProcessName;
            public int ProcessID => Process.Id;
            public Icon? Icon { get; protected init; }

            public void SetImage(ImageList imageList)
            {
                if (Icon == null)
                    return;

                ImageIndex = imageList.Images.Count;
                imageList.Images.Add(Icon);
            }

            public ListViewItemProcess(Process process, ImageList imageList)
            {
                Process = process;

                try
                {
                    if (process.MainModule?.FileName != null)
                    {
                        Icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                    }
                }
                catch { }

                SetImage(imageList);
                _nameItem = new ListViewSubItem();
                _nameItem.Text = ProcessName;

                PidHexChanged += ListViewItemProcess_PidHexChanged;
                UpdateText();

                SubItems.Add(_nameItem);
            }

            ~ListViewItemProcess()
            {
                PidHexChanged -= ListViewItemProcess_PidHexChanged;
            }

            private void ListViewItemProcess_PidHexChanged(object? sender, EventArgs e)
            {
                UpdateText();
            }

            private void UpdateText()
            {
                if (PidHex)
                {
                    Text = "0x" + ProcessID.ToString("X8");
                }
                else
                {
                    Text = ProcessID.ToString();
                }
            }

        }

#endregion
    }
}

