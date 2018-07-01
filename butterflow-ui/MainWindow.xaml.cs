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
        #region Members

        //

        #endregion

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
                this.OptionsConfiguration.VideoInput = ofd.FileName;

                //Hack to get the first frame to display in the media preview element.
                //This also triggers the MediaOpened event so we can get the metadata from the element.
                mediaPreview.Play();
                mediaPreview.PausePlayback();
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

        /// <summary> Event handler. Called by bntVideoPlay for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void bntVideoPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this.mediaPreview.CanPlay() || this.mediaPreview.CanPause.GetValueOrDefault(false))
            {
                this.mediaPreview.TogglePlayPause();
            }

            if(this.mediaPreview.IsPlaying)
            {
                this.PlayPauseButtonIcon.Template = Application.Current.Resources["PauseIcon"] as ControlTemplate;
            }
            else
            {
                this.PlayPauseButtonIcon.Template = Application.Current.Resources["PlayIcon"] as ControlTemplate;
            }
        }

        /// <summary> Event handler. Called by bntVideoStop for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void bntVideoStop_Click(object sender, RoutedEventArgs e)
        {
            this.PlayPauseButtonIcon.Template = Application.Current.Resources["PlayIcon"] as ControlTemplate;
            this.mediaPreview.Stop();
        }

        /// <summary> Event handler. Called by bntVideoForward for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void bntVideoForward_Click(object sender, RoutedEventArgs e)
        {
            if (this.mediaPreview.CanSkipForward(null))
            {
                this.mediaPreview.SkipForward(null);
            }
        }

        /// <summary> Event handler. Called by bntVideoBackward for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void bntVideoBackward_Click(object sender, RoutedEventArgs e)
        {
            if (this.mediaPreview.CanSkipBack(null))
            {
                this.mediaPreview.SkipBack(null);
            }
        }

        /// <summary> Event handler. Called by mediaPreview for media opened events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void mediaPreview_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.OptionsConfiguration.Width = this.mediaPreview.NaturalVideoWidth.ToString();
            this.OptionsConfiguration.Height = this.mediaPreview.NaturalVideoHeight.ToString();
        }

        /// <summary> Event handler. Called by mediaPreview for media ended events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void mediaPreview_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.PlayPauseButtonIcon.Template = Application.Current.Resources["PlayIcon"] as ControlTemplate;
        }
    }
}
