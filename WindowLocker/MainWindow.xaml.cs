using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using WindowLocker.Extensions;
using static WindowLocker.Win32;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using Timer = System.Timers.Timer;

/* TODO:
 * -Docking areas with outline
 * -Display bounds viewer
*/
namespace WindowLocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string NoWindowSelectedText = "<No Window Selected>";

        private readonly WindowManager _windowManager;
        private readonly FindWindowHelper _findWindowHelper;

        public ObservableCollection<LockedWindow> LockedWindows { get; }
        public LockedWindow? SelectedWindow { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            LockedWindows = new ObservableCollection<LockedWindow>();
            _windowManager = new WindowManager();
            _findWindowHelper = new FindWindowHelper();
            _findWindowHelper.HoverWindow += FindWindowHelper_HoverWindow;
            _findWindowHelper.SelectedWindow += FindWindowHelper_SelectedWindow;
            DataContext = this;
            WindowNameLabel.Content = NoWindowSelectedText;
            App.ApplicationExit += App_ApplicationExit;
        }

        private void App_ApplicationExit(object? sender, EventArgs e)
        {
            foreach (var window in LockedWindows)
            {
                window.Locked = false;
                window.HideTitleBar = false;
            }
        }

        private void SetWindowControlEvents(bool subscribe)
        {
            XPosition.TextChanged -= WindowTextBox_TextChanged;
            YPosition.TextChanged -= WindowTextBox_TextChanged;
            WindowWidth.TextChanged -= WindowTextBox_TextChanged;
            WindowHeight.TextChanged -= WindowTextBox_TextChanged;

            if (subscribe)
            {
                XPosition.TextChanged += WindowTextBox_TextChanged;
                YPosition.TextChanged += WindowTextBox_TextChanged;
                WindowWidth.TextChanged += WindowTextBox_TextChanged;
                WindowHeight.TextChanged += WindowTextBox_TextChanged;
            }
        }

        private void ClearWindow()
        {
            if (SelectedWindow != null)
            {
                SelectedWindow.IsSelected = false;
                SelectedWindow = null;
            }

            WindowNameLabel.Content = NoWindowSelectedText;
            SetWindowControlEvents(false);
            XPosition.Clear();
            YPosition.Clear();
            WindowWidth.Clear();
            WindowHeight.Clear();
            SetWindowControlEvents(true);
        }

        private void AddLockedWindow(LockedWindow lockedWindow)
        {
            _windowManager.Add(lockedWindow);
            LockedWindows.Add(lockedWindow);
            SelectWindow(lockedWindow);

            lockedWindow.WindowClosed += SelectedWindow_WindowClosed;
        }

        private void SelectWindow(LockedWindow? lockedWindow)
        {
            if (lockedWindow == null)
            {
                ClearWindow();
                return;
            }

            if (SelectedWindow != null)
            {
                SelectedWindow.IsSelected = false;
            }

            lockedWindow.IsSelected = true;
            WindowNameLabel.Content = $"{lockedWindow.Title} - {lockedWindow.Pid}";
            SelectedWindow = lockedWindow;

            WindowLockedCheckBox.IsChecked = lockedWindow.Locked;
            HideBarCheckBox.IsChecked = lockedWindow.HideTitleBar;
            AllowMinimizeCheckBox.IsChecked = lockedWindow.AllowMinimize;

            SetWindowControlEvents(false);
            SetWindowPropertiesEnabled(lockedWindow.Locked);
            XPosition.Text = lockedWindow.X.ToString();
            YPosition.Text = lockedWindow.Y.ToString();
            WindowWidth.Text = lockedWindow.Width.ToString();
            WindowHeight.Text = lockedWindow.Height.ToString();
            LeftTextBox.Text = lockedWindow.Left.ToString();
            TopTextBox.Text = lockedWindow.Top.ToString();
            RightTextBox.Text = lockedWindow.Right.ToString();
            BottomTextBox.Text = lockedWindow.Bottom.ToString();
            SetWindowControlEvents(true);
        }

        private void SetWindowPropertiesEnabled(bool enabled)
        {
            XPosition.IsEnabled = enabled;
            YPosition.IsEnabled = enabled;
            WindowWidth.IsEnabled = enabled;
            WindowHeight.IsEnabled = enabled;
            LeftTextBox.IsEnabled = enabled;
            TopTextBox.IsEnabled = enabled;
            RightTextBox.IsEnabled = enabled;
            BottomTextBox.IsEnabled = enabled;
        }

        private void UpdateWindowRect(TextBox textBox)
        {
            if (SelectedWindow != null)
            {
                int value = int.Parse(textBox.Text);

                if (textBox == XPosition)
                {
                    SelectedWindow.X = value;
                    XPosition.Text = SelectedWindow.X.ToString();
                }
                else if (textBox == YPosition)
                {
                    SelectedWindow.Y = value;
                    YPosition.Text = SelectedWindow.Y.ToString();
                }
                else if (textBox == WindowWidth)
                {
                    SelectedWindow.Width = value;
                    WindowWidth.Text = SelectedWindow.Width.ToString();
                }
                else if (textBox == WindowHeight)
                {
                    SelectedWindow.Height = value;
                    WindowHeight.Text = SelectedWindow.Height.ToString();
                }
                else if (textBox == LeftTextBox)
                {
                    SelectedWindow.Left = value;
                    LeftTextBox.Text = SelectedWindow.Left.ToString();
                }
                else if (textBox == TopTextBox)
                {
                    SelectedWindow.Top = value;
                    TopTextBox.Text = SelectedWindow.Top.ToString();
                }
                else if (textBox == RightTextBox)
                {
                    SelectedWindow.Right = value;
                    RightTextBox.Text = SelectedWindow.Right.ToString();
                }
                else if (textBox == BottomTextBox)
                {
                    SelectedWindow.Bottom = value;
                    BottomTextBox.Text = SelectedWindow.Bottom.ToString();
                }
            }
        }

        #region Event Handlers

        private void SelectedWindow_WindowClosed(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LockedWindow window = (LockedWindow)sender!;
                LockedWindows.Remove(window);

                if (SelectedWindow == sender)
                {
                    SelectedWindow!.WindowClosed -= SelectedWindow_WindowClosed;
                    ClearWindow();
                }
            });
        }

        private void FindWindowButton_Click(object sender, RoutedEventArgs e)
        {
            _findWindowHelper.StartSearching();
            FindWindowButton.Content = "Release to capture";
        }

        private void FindWindowHelper_SelectedWindow(object? sender, WindowInfo e)
        {
            Dispatcher.Invoke(() =>
            {
                FindWindowButton.Content = "Select Window";
                FindWindowButton.IsChecked = false;

                //FindWindowButton.IsChecked = false;
                IntPtr hWnd = e.hwnd;
                if (hWnd != IntPtr.Zero && !_windowManager.Contains(hWnd))
                {
                    LockedWindow lockedWindow = new LockedWindow(hWnd);
                    AddLockedWindow(lockedWindow);
                }
                else
                {
                    WindowNameLabel.Content = SelectedWindow?.Title ?? NoWindowSelectedText;
                }
            });
        }

        private void FindWindowHelper_HoverWindow(object? sender, WindowInfo e)
        {
            Dispatcher.Invoke(() =>
            {
                WindowNameLabel.Content = e.title != null ? $"{e.title} - {e.pid}" : NoWindowSelectedText;
            });
        }

        private void FindProcessButton_Click(object sender, RoutedEventArgs e)
        {
            Xorrupt.Windows.Forms.OpenProcessDialog openProcessDialog = new Xorrupt.Windows.Forms.OpenProcessDialog(Xorrupt.Windows.Forms.OpenProcessDialog.ListTypeFlag.Windows);
            if (openProcessDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LockedWindow lockedWindow = new LockedWindow(openProcessDialog.Process!);
                AddLockedWindow(lockedWindow);
            }
        }

        private void RemoveWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWindow == null)
            {
                return;
            }

            _windowManager.Remove(SelectedWindow);
            LockedWindows.Remove(SelectedWindow);
            ClearWindow();
        }

        private void FileMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LockedWindows.Count != 0)
            {
                OpenPresetButton.IsEnabled = true;
                //OpenPresetAutoButton.IsEnabled = true;
                SaveAsPresetButton.IsEnabled = true;
            }
            else
            {
                OpenPresetButton.IsEnabled = false;
                //OpenPresetAutoButton.IsEnabled = false;
                SaveAsPresetButton.IsEnabled = false;
            }
        }

        private void WindowTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                bool firstChar = true;
                textBox.Text = textBox.Text.RemoveInvalidCharacters((c) =>
                {
                    bool valid = char.IsNumber(c) || (firstChar && c == '-');
                    firstChar = false;
                    return valid;
                });

                if (textBox.Text.Length == 0)
                {
                    textBox.Text = "0";
                }
            }
        }

        private void WindowTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWindowRect((sender as TextBox)!);
        }

        private void WindowTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateWindowRect((sender as TextBox)!);
            }
        }

        private void WindowLockedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWindow == null)
            {
                return;
            }

            if (WindowLockedCheckBox.IsChecked!.Value)
            {
                SelectedWindow.Locked = true;
                WindowLockedCheckBox.IsChecked = SelectedWindow.Locked;
            }
            else
            {
                SelectedWindow.Locked = false;
            }

            SetWindowPropertiesEnabled(SelectedWindow.Locked);
        }

        private void AllowMinimizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWindow == null)
            {
                return;
            }

            SelectedWindow.AllowMinimize = AllowMinimizeCheckBox.IsChecked!.Value;
        }

        private void HideBarCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWindow == null)
            {
                return;
            }

            SelectedWindow.HideTitleBar = HideBarCheckBox.IsChecked!.Value;
        }

        private void WindowTemplate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is LockedWindow window)
            {
                SelectWindow(window);
            }
        }

        private void LockedListCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox && checkbox.DataContext is LockedWindow window)
            {
                if (SelectedWindow == window)
                {
                    if (checkbox.IsChecked!.Value)
                    {
                        WindowLockedCheckBox.IsChecked = SelectedWindow.Locked = true;
                    }
                    else
                    {
                        WindowLockedCheckBox.IsChecked = SelectedWindow.Locked = false;

                    }
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        #endregion Event Handlers


        #region Preset

        private void ApplyPresetsSelect(LockedWindowPreset[] presets)
        {
            PresetApplier applier = new PresetApplier(presets, LockedWindows);
            applier.ShowDialog();
        }

        private void ApplyPreset(LockedWindow wnd, LockedWindowPreset preset)
        {
            Debug.Assert(wnd != null);
            Debug.Assert(preset != null);
            wnd.ApplyPreset(preset);
        }

        private int ApplyPresetsAuto(LockedWindowPreset[] presets)
        {
            List<LockedWindow> applied = new List<LockedWindow>();

            foreach (var preset in presets)
            {
                try
                {
                    var lockedWindow = LockedWindows.FirstOrDefault(x => !applied.Contains(x) && x.Process.ProcessName == preset.processName && x.Title == preset.windowTitle);

                    if (lockedWindow == null)
                    {
                        if (preset.filename != null && File.Exists(preset.filename))
                        {
                            Process? proc;

                            if (preset.arguments.NullOrEmpty())
                            {
                                proc = Process.Start(preset.filename);
                            }
                            else
                            {
                                proc = new Process();
                                proc.StartInfo.FileName = preset.filename/*"cmd.exe"*/;
                                proc.StartInfo.Arguments = preset.arguments;
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.RedirectStandardError = true;
                                proc.Start();
                            }

                            if (proc != null)
                            {
                                if (proc.MainWindowHandle == IntPtr.Zero)
                                {
                                    throw new NullReferenceException($"Main window handle null for process '{proc.ProcessName}'");
                                }

                                LockedWindow newWindow = new LockedWindow(proc.MainWindowHandle);
                                AddLockedWindow(newWindow);
                                ApplyPreset(newWindow, preset);
                                applied.Add(newWindow);
                            }
                        }
                    }
                    else
                    {
                        ApplyPreset(lockedWindow, preset);
                        applied.Add(lockedWindow);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while applying a preset to process '{preset.processName}': '{ex.Message}'");
                }
            }
            return applied.Count;
        }

        private void OpenPreset(bool auto)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "JSON Preset (*.wndlock)|*.wndlock|All files (*.*)|*.*";

                var result = fileDialog.ShowDialog();

                if (result != null && result.Value)
                {
                    string filename = fileDialog.FileName;
                    string json = File.ReadAllText(filename);

                    LockedWindowPreset[]? presets = JsonSerializer.Deserialize<LockedWindowPreset[]>(json, new JsonSerializerOptions { IncludeFields = true });

                    if (presets == null)
                    {
                        throw new JsonException("Unknown error");
                    }

                    if (auto)
                    {
                        int applied = ApplyPresetsAuto(presets);
                        MessageBox.Show($"{applied}/{presets.Length} presets were successfully applied.");
                    }
                    else
                    {
                        ApplyPresetsSelect(presets);
                    }
                }

            }
            catch (JsonException ex)
            {
                throwJsonException(ex);
            }
            catch (NotSupportedException ex)
            {
                throwJsonException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Failed to Open");
            }

            void throwJsonException(Exception ex)
            {
                MessageBox.Show($"Failed to deserialize data: {(ex).Message}", "Deserialization Failed");
            }
        }

        private void OpenPresetAuto_Click(object sender, RoutedEventArgs e)
        {
            OpenPreset(auto: true);
        }

        private void OpenPreset_Click(object sender, RoutedEventArgs e)
        {
            OpenPreset(auto: false);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = LockedWindows.Select(x =>
                {
                    x.CapturePosition();
                    return new LockedWindowPreset(x);
                });

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "JSON Preset (*.wndlock)|*.wndlock|All files (*.*)|*.*";

                var result = fileDialog.ShowDialog();

                if (result != null && result.Value)
                {
                    string filename = fileDialog.FileName;
                    File.WriteAllText(filename, json);
                }
            }
            catch (JsonException ex)
            {
                throwJsonException(ex);
            }
            catch (NotSupportedException ex)
            {
                throwJsonException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Failed to Save");
            }

            void throwJsonException(Exception ex)
            {
                MessageBox.Show($"Failed to serialize data: {(ex).Message}", "Serialization Failed");
            }
        }

        #endregion Preset
    }
}