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

        /// <summary> Gets or sets the butyterflow options configuration. </summary>
        /// <value> The options configuration. </value>
        public OptionsConfiguration OptionsConfiguration { get; set; } = new OptionsConfiguration();

        #endregion

        /// <summary> Default constructor. </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary> Event handler. Called by btnFilePicker for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Supported Video Files|*.mp4;*.mkv";
            

            var result = ofd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                txtFileName.Text = ofd.FileName;
                mediaPreview.Source = new Uri(ofd.FileName);
                this.OptionsConfiguration.Width = mediaPreview.NaturalVideoWidth.ToString();
                this.OptionsConfiguration.Height = mediaPreview.NaturalVideoHeight.ToString();

            }
        }

        /// <summary> Event handler. Called by PlaybackRate radio buttons for checked events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void PlaybackRateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var typedSender = (RadioButton)sender;

            if(typedSender != null)
            {
                var tag = typedSender.Tag.ToString();
                this.OptionsConfiguration.PlaybackRate = tag;
            }
        }
    }
}
