using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FinnZan.Utilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Mostly doing bindings and commands
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEventSource _source = null;
        private MainWindowViewModel _vm = null;

        public MainWindow()
        {
            //Debugger.Launch();
            InitializeComponent();                                  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxFilterTypes.ItemsSource = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
            cbxFilterTypes.SelectedIndex = 0;

            SetMode();
        }

        private void SetMode()
        {
            if (App.RunAsServer)
            {
                panelAppName.Visibility = Visibility.Visible;
                lbAppName.Content = App.AppName;
                this.Title = $"{App.AppName} Log View";

                //var ps = new PipeLogReceiver();
                //ps.StartListening(App.AppName);
                //_source = ps;
                WcfLogReceiver.Instance.StartListening(App.AppName);
                _source = WcfLogReceiver.Instance;

                _vm = new MainWindowViewModel(_source);
            }
            else
            {
                panelBrowse.Visibility = Visibility.Visible;
                _source = new LogFileReader();
                _vm = new MainWindowViewModel(_source);
            }

            DataContext = _vm;

            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Filters")
                {
                    lbxFilters.ItemsSource = null;
                    lbxFilters.ItemsSource = _vm.Filters;
                }

                if (e.PropertyName == "FiltersEx")
                {
                    lbxFiltersEx.ItemsSource = null;
                    lbxFiltersEx.ItemsSource = _vm.FiltersEx;
                }

                if (e.PropertyName == "AppDomains")
                {
                    BindAppDomains();
                }
            });

            try
            {
                var filtersEx = Properties.Settings.Default["FiltersEx"] as string;

                var toks = filtersEx.Split('$');
                foreach (var t in toks.Where(x => !string.IsNullOrEmpty(x)))
                {
                    _vm.AddFilter(FilterType.Event, t, false);
                }
            }
            catch (Exception ex) 
            {
                
            }
        }

        private void BindAppDomains()
        {
            if (_vm != null && ((FilterType)cbxFilterTypes.SelectedItem) == FilterType.AppDomain)
            {
                tbKey.ItemsSource = _vm.AppDomains;
            }
        }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void lvEvents_MouseDoubleClick(object sender, EventArgs e)
        {
#if !PRODUCTION_RELEASE
            panelStackTrace.Visibility = Visibility.Visible;
            try
            {
                if (lvEvents.SelectedItem != null)
                {
                    lbxStackFrames.ItemsSource = (lvEvents.SelectedItem as LogEvent).CallStack;
                }
            }
            catch (Exception ex)
            {

            }
#endif
        }

        private void btCloseStackTrace_Click(object sender, RoutedEventArgs e)
        {
            panelStackTrace.Visibility = Visibility.Collapsed;
        }

        private void cbxFilterTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindAppDomains();
        }

        private void btFilters_Click(object sender, RoutedEventArgs e)
        {
            _vm.FilterEnabled = !_vm.FilterEnabled;
        }

        private void btAddFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbKey.Text.Length > 0)
                {
                    var type = (FilterType)Enum.Parse(typeof(FilterType), cbxFilterTypes.SelectedItem.ToString(), false);                
                    _vm.AddFilter(type, tbKey.Text, !cbExclude.IsChecked.Value);

                    UpdateFilterSettings();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btDeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                var curItem = ((ListBoxItem)lbxFilters.ContainerFromElement((Button)sender)).Content as Filter;                
                _vm.DeleteFilter(curItem, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btDeleteFilterEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var curItem = ((ListBoxItem)lbxFiltersEx.ContainerFromElement((Button)sender)).Content as Filter;
                _vm.DeleteFilter(curItem, false);

                UpdateFilterSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateFilterSettings() 
        {
            var s = string.Empty;
            foreach (var filter in _vm.FiltersEx.Where(x => x.Type == FilterType.Event))
            {
                s += "$" + filter.Key;
            }
            Properties.Settings.Default["FiltersEx"] = s;
            Properties.Settings.Default.Save();
        }

        private void BtClear_Click(object sender, RoutedEventArgs e)
        {
            _vm.Clear();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvEvents.SelectedIndex != -1)
            {
                var evt = lvEvents.SelectedItem as LogEvent;

                var mi = sender as MenuItem;
                if (mi.Header.ToString().Contains("Include"))
                {
                    _vm.AddFilter(FilterType.Source, evt.Source, true);
                }

                if (mi.Header.ToString().Contains("Exclude"))
                {
                    _vm.AddFilter(FilterType.Source, evt.Source, false);
                }

                if (mi.Header.ToString().Contains("Copy"))
                {
                    Clipboard.SetText(evt.Event);
                }
            }
        }
                
        private void btBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            var result = dlg.ShowDialog();
            if (result.Value) // Test result.
            {
                lbFile.Content = dlg.FileName;
                (_source as LogFileReader).LoadFile(dlg.FileName);                                              
            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }


    [ValueConversion(typeof(int), typeof(Color))]
    public class ThreadIDColorConverter : IValueConverter
    {
        public static Color[] ThreadColors = { Color.FromRgb(200, 100, 200),
                                    Color.FromRgb(200, 200, 200),
                                    Color.FromRgb(200, 255, 255),
                                    Color.FromRgb(200, 100, 100),
                                    Color.FromRgb(255, 250, 200),
                                    Color.FromRgb(250, 100, 200),
                                    Color.FromRgb(100, 250, 200),
                                    Color.FromRgb(200, 100, 255),
                                    Color.FromRgb(255, 255, 200)};

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int v = (int)value;
                if (v == 1 || v == -1)
                {
                    return Colors.White;
                }
                else
                {
                    return ThreadColors[v % ThreadColors.Length];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c = (Color)value;

            return 0;
        }

    }
}
