using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace butterflow_ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        public string PlaybackRate
        {
            get
            {
                return (string)GetValue(PlaybackRateProperty);
            }
            set
            {
                SetValue(PlaybackRateProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty PlaybackRateProperty = DependencyProperty.Register("PlaybackRateProperty", typeof(string), typeof(MainWindow));

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Supported Video Files|*.mp4;*.mkv";

            var result = ofd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                txtFileName.Text = ofd.FileName;
                mediaPreview.Source = new Uri(ofd.FileName);
            }
        }
    }
}
