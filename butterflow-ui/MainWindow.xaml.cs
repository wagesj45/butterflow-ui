using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Methods

        /// <summary> Gets the recursive children of a <paramref name="parent"/> element that are of type <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="parent"> The parent element. </param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the recursive childrens in this
        /// collection.
        /// </returns>
        private IEnumerable<T> GetRecursiveChildren<T>(object parent) where T : DependencyObject
        {
            if (parent is DependencyObject)
            {
                var list = new List<T>();

                foreach (var child in LogicalTreeHelper.GetChildren((DependencyObject)parent))
                {
                    if (child is DependencyObject)
                    {
                        if (child is T)
                        {
                            list.Add((T)child);
                        }
                        list.AddRange(GetRecursiveChildren<T>((DependencyObject)child));
                    }
                }

                return list;
            }

            return Enumerable.Empty<T>();
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
                    // This case doesn't need to be considered since we're binding the progress bar's value to a property on the butterflow wrapper.
                    // We may use this in the future, though.
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

                this.ButterflowWrapper.Probe(ofd.FileName);

                //Hack to get the first frame to display in the media preview element.
                mediaPreview.Play();
                mediaPreview.PausePlayback();
            }
        }

        /// <summary> Event handler. Called by btnFileOutputPicker for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnFileOutputPicker_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "MPEG 4|*.mp4";

            var result = sfd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                this.OptionsConfiguration.VideoOutput = sfd.FileName;
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

        /// <summary> Event handler. Called by txtPlaybackRate for got focus events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void txtPlaybackRate_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clear all the radio buttons because we got focus from the user in the playbackrate textbox.
            var playbackRateRadioButtons = GetRecursiveChildren<RadioButton>(this.butterflowUIWindow);

            foreach (var radioButton in playbackRateRadioButtons)
            {
                radioButton.IsChecked = false;
            }
        }

        /// <summary> Event handler. Called by btnCopyArguments for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnCopyArguments_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.OptionsConfiguration.CommandLineOutput);
        }

        /// <summary> Event handler. Called by btnProcess for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            this.ButterflowWrapper.Run(this.OptionsConfiguration);
        }

        /// <summary> Event handler. Called by btnRemoveSubregion for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnRemoveSubregion_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if(button != null)
            {
                if (button.Tag.GetType() == typeof(Guid))
                {
                    var identifier = (Guid)button.Tag;

                    if (this.OptionsConfiguration.Subregions.Any(sr => sr.Identifier == identifier))
                    {
                        var item = this.OptionsConfiguration.Subregions.Where(sr => sr.Identifier == identifier).First();
                        this.OptionsConfiguration.Subregions.Remove(item);
                    }
                }
            }
        }

        /// <summary> Event handler. Called by menuOpen for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary> Event handler. Called by menuSaveConfiguration for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary> Event handler. Called by menuSaveConfigurationAs for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuSaveConfigurationAs_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary> Event handler. Called by menuButterflowGithub for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuButterflowGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/dthpham/butterflow");
        }

        /// <summary> Event handler. Called by menuButterflowUIGithub for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuButterflowUIGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/wagesj45/butterflow-ui");
        }

        /// <summary> Event handler. Called by menuAboutButterflowUI for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuAboutButterflowUI_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
