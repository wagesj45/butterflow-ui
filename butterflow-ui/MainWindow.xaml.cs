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

        /// <summary> True if is the user has started clipping, false if not. </summary>
        private bool isClipping;
        /// <summary> The temporary storage for the clip start time. </summary>
        private TimeSpan clipStart;

        #endregion

        #region Properties

        /// <summary> Gets or sets the butterflow options configuration. </summary>
        /// <value> The options configuration. </value>
        public OptionsConfiguration OptionsConfiguration { get; set; } = new OptionsConfiguration();
        /// <summary> The butterflow wrapper used to call butterflow. </summary>
        public ButterflowWrapper ButterflowWrapper { get; set; } = new ButterflowWrapper();

        #endregion

        /// <summary> Default constructor. </summary>
        public MainWindow()
        {
            this.ButterflowWrapper.ParsedConsoleOutputRecieved += ButterflowWrapper_ParsedConsoleOutputRecieved;
            InitializeComponent();
        }

        /// <summary> Butterflow wrapper parsed console output recieved. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      The ButterflowOutputArgs to process. </param>
        private void ButterflowWrapper_ParsedConsoleOutputRecieved(object sender, ButterflowWrapper.ButterflowOutputArgs e)
        {
            switch (e.OutputType)
            {
                case ButterflowWrapper.ButterflowOutputType.Width:
                    this.OptionsConfiguration.Width = e.Value;
                    break;
                case ButterflowWrapper.ButterflowOutputType.Height:
                    this.OptionsConfiguration.Height = e.Value;
                    break;
                case ButterflowWrapper.ButterflowOutputType.Progress:
                    break;
                default:
                    break;
            }
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

                //this.ButterflowWrapper.ConsoleOutputRecieved += (o, ce) => this.txtConsoleOutput.Text = ce.ConsoleOutput;
                this.ButterflowWrapper.Probe(ofd.FileName);

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

            if (typedSender != null)
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

            if (this.mediaPreview.IsPlaying)
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

        /// <summary> Event handler. Called by bntClip for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void bntClip_Click(object sender, RoutedEventArgs e)
        {
            if (this.mediaPreview.Position.HasValue)
            {
                if (!this.isClipping)
                {
                    //start clipping
                    this.isClipping = true;
                    this.ClippingButtonIcon.Template = Application.Current.Resources["SnipCloseIcon"] as ControlTemplate;
                    this.clipStart = this.mediaPreview.Position.Value;
                }
                else
                {
                    //end clipping
                    this.isClipping = false;
                    this.ClippingButtonIcon.Template = Application.Current.Resources["SnipOpenIcon"] as ControlTemplate;
                    this.OptionsConfiguration.Subregions.Add(new ButterflowSubregion() { Start = this.clipStart, End = this.mediaPreview.Position.Value, ToEnd = false, Value = 1, SubregionType = RegionType.spd });
                    this.clipStart = TimeSpan.Zero;
                }
            }
        }

        /// <summary> Event handler. Called by mediaPreview for media opened events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void mediaPreview_MediaOpened(object sender, RoutedEventArgs e)
        {
            //this.OptionsConfiguration.Width = this.mediaPreview.NaturalVideoWidth.ToString();
            //this.OptionsConfiguration.Height = this.mediaPreview.NaturalVideoHeight.ToString();
        }

        /// <summary> Event handler. Called by mediaPreview for media ended events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void mediaPreview_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.PlayPauseButtonIcon.Template = Application.Current.Resources["PlayIcon"] as ControlTemplate;
        }

        /// <summary> Event handler. Called by ScrollViewer for scroll changed events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Scroll changed event information. </param>
        /// <remarks>
        /// This code autoscrolls the scroll viewer as more text is added. It is based on this example
        /// from Stack Overflow:
        /// https://stackoverflow.com/questions/2984803/how-to-automatically-scroll-scrollviewer-only-if-the-user-did-not-change-scrol.
        /// </remarks>
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            bool autoScroll = true;

            if (scrollViewer != null)
            {
                if (e.ExtentHeightChange == 0)
                {   // Content unchanged : user scroll event
                    if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                    {   // Scroll bar is in bottom
                        // Set auto-scroll mode
                        autoScroll = true;
                    }
                    else
                    {   // Scroll bar isn't in bottom
                        // Unset auto-scroll mode
                        autoScroll = false;
                    }
                }

                // Content scroll event : auto-scroll eventually
                if (autoScroll && e.ExtentHeightChange != 0)
                {   // Content changed and auto-scroll mode set
                    // Autoscroll
                    scrollViewer.ScrollToEnd();
                }
            }
        }
    }
}
