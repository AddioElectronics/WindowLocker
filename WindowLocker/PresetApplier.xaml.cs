using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowLocker
{
    /// <summary>
    /// Interaction logic for PresetApplier.xaml
    /// </summary>
    public partial class PresetApplier : Window
    {

        private LockedWindow? _selectedWindow;
        private int _presetIndex;


        public LockedWindowPreset[] Presets { get; }

        public ObservableCollection<LockedWindow> LockedWindows { get; }


        public LockedWindowPreset Preset { get; private set; }

        public LockedWindow? SelectedWindow
        {
            get => _selectedWindow;
            private set
            {
                if (_selectedWindow != null)
                {
                    _selectedWindow.IsSelected = false;
                }


                _selectedWindow = value;
                ApplyButton.IsEnabled = value != null;
            }
        }


        public PresetApplier(IEnumerable<LockedWindowPreset> presets, IEnumerable<LockedWindow> lockedWindows)
        {
            Debug.Assert(presets != null);
            Debug.Assert(presets.DefaultIfEmpty() != null);
            Debug.Assert(lockedWindows != null);
            Debug.Assert(lockedWindows.DefaultIfEmpty() != null);

            InitializeComponent();
            LockedWindows = new ObservableCollection<LockedWindow>(lockedWindows);
            SelectedWindow = LockedWindows.FirstOrDefault(x => x.IsSelected);
            Presets = presets as LockedWindowPreset[] ?? presets.ToArray();
            
            DataContext = this;
            ApplyPresetData(Presets[0]);
            Preset = Preset ?? throw new NullReferenceException(nameof(Preset));
        }

        private void ApplyPresetData(LockedWindowPreset preset)
        {
            Preset = preset;
            ProcessNameText.Text = preset.processName;
            //LockedText.Text = presetData.locked.ToString();
            AllowMinimizeText.Text= preset.allowMinimize.ToString();
            HideTitleText.Text= preset.hideTitleBar.ToString();

            LeftTextBlock.Text = XPosition.Text = preset.rect.Left.ToString();
            TopTextBlock.Text = YPosition.Text = preset.rect.Top.ToString();
            WindowWidth.Text = (preset.rect.Right - preset.rect.Left).ToString();
            WindowHeight.Text = (preset.rect.Bottom - preset.rect.Top).ToString();
            BottomTextBlock.Text = XPosition.Text = preset.rect.Right.ToString();
            RightTextBlock.Text = YPosition.Text = preset.rect.Bottom.ToString();
        }

        private void WindowTemplate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is LockedWindow window)
            {
                SelectedWindow = window;
            }
        }

        private void NextPreset()
        {
            if (++_presetIndex < Presets.Length)
            {
                ApplyPresetData(Presets[_presetIndex]);
            }
            else
            {
                Close();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(SelectedWindow != null);

            SelectedWindow.ApplyPreset(Preset);
            LockedWindows.Remove(SelectedWindow);
            SelectedWindow = null;

            NextPreset();
        }

        private void DontApplyButton_Click(object sender, RoutedEventArgs e)
        {
            NextPreset();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
