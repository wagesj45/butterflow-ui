using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        /// <summary> List of types of videos supported by ffmpeg, and thus butterflow. </summary>
        private const string SUPPORTED_FILE_TYPES = "*.3dostr;*.3g2;*.3gp;*.4xm;*.a64;*.aa;*.aac;*.ac3;*.acm;*.act;*.adf;*.adp;*.ads;*.adts;*.adx;*.aea;*.afc;*.aiff;*.aix;*.alaw;*.alias_pix;*.alsa;*.amr;*.anm;*.apc;*.ape;*.apng;*.aqtitle;*.asf;*.asf_o;*.asf_stream;*.ass;*.ast;*.au;*.avi;*.avisynth;*.avm2;*.avr;*.avs;*.bethsoftvid;*.bfi;*.bfstm;*.bin;*.bink;*.bit;*.bmp_pipe;*.bmv;*.boa;*.brender_pix;*.brstm;*.c93;*.caca;*.caf;*.cavsvideo;*.cdg;*.cdxl;*.chromaprint;*.cine;*.concat;*.crc;*.dash;*.data;*.daud;*.dcstr;*.dds_pipe;*.dfa;*.dirac;*.dnxhd;*.dpx_pipe;*.dsf;*.dsicin;*.dss;*.dts;*.dtshd;*.dv;*.dv1394;*.dvbsub;*.dvbtxt;*.dvd;*.dxa;*.ea;*.ea_cdata;*.eac3;*.epaf;*.exr_pipe;*.f32be;*.f32le;*.f4v;*.f64be;*.f64le;*.fbdev;*.ffm;*.ffmetadata;*.fifo;*.film_cpk;*.filmstrip;*.flac;*.flic;*.flv;*.framecrc;*.framehash;*.framemd5;*.frm;*.fsb;*.g722;*.g723_1;*.g729;*.genh;*.gif;*.gsm;*.gxf;*.h261;*.h263;*.h264;*.hash;*.hds;*.hevc;*.hls;*.hls;*.applehttp;*.hnm;*.ico;*.idcin;*.idf;*.iec61883;*.iff;*.ilbc;*.image2;*.image2pipe;*.ingenient;*.ipmovie;*.ipod;*.ircam;*.ismv;*.iss;*.iv8;*.ivf;*.ivr;*.j2k_pipe;*.jack;*.jacosub;*.jpeg_pipe;*.jpegls_pipe;*.jv;*.latm;*.lavfi;*.libcdio;*.libdc1394;*.libgme;*.libopenmpt;*.live_flv;*.lmlm4;*.loas;*.lrc;*.lvf;*.lxf;*.m4v;*.matroska;*.matroska;*.webm;*.md5;*.mgsts;*.microdvd;*.mjpeg;*.mkvtimestamp_v2;*.mkv;*.mlp;*.mlv;*.mm;*.mmf;*.mov;*.mov;*.mp4;*.m4a;*.3gp;*.3g2;*.mj2;*.mp2;*.mp3;*.mp4;*.mpc;*.mpc8;*.mpeg;*.mpeg1video;*.mpeg2video;*.mpegts;*.mpegtsraw;*.mpegvideo;*.mpjpeg;*.mpl2;*.mpsub;*.msf;*.msnwctcp;*.mtaf;*.mtv;*.mulaw;*.musx;*.mv;*.mvi;*.mxf;*.mxf_d10;*.mxf_opatom;*.mxg;*.nc;*.nistsphere;*.nsv;*.null;*.nut;*.nuv;*.oga;*.ogg;*.ogv;*.oma;*.openal;*.opengl;*.opus;*.oss;*.paf;*.pam_pipe;*.pbm_pipe;*.pcx_pipe;*.pgm_pipe;*.pgmyuv_pipe;*.pictor_pipe;*.pjs;*.pmp;*.png_pipe;*.ppm_pipe;*.psp;*.psxstr;*.pulse;*.pva;*.pvf;*.qcp;*.qdraw_pipe;*.r3d;*.rawvideo;*.realtext;*.redspark;*.rl2;*.rm;*.roq;*.rpl;*.rsd;*.rso;*.rtp;*.rtp_mpegts;*.rtsp;*.s16be;*.s16le;*.s24be;*.s24le;*.s32be;*.s32le;*.s8;*.sami;*.sap;*.sbg;*.sdl;*.sdl2;*.sdp;*.sdr2;*.segment;*.sgi_pipe;*.shn;*.siff;*.singlejpeg;*.sln;*.smjpeg;*.smk;*.smoothstreaming;*.smush;*.sndio;*.sol;*.sox;*.spdif;*.spx;*.srt;*.stl;*.stream_segment;*.ssegment;*.subviewer;*.subviewer1;*.sunrast_pipe;*.sup;*.svag;*.svcd;*.swf;*.tak;*.tedcaptions;*.tee;*.thp;*.tiertexseq;*.tiff_pipe;*.tmv;*.truehd;*.tta;*.tty;*.txd;*.u16be;*.u16le;*.u24be;*.u24le;*.u32be;*.u32le;*.u8;*.uncodedframecrc;*.v210;*.v210x;*.v4l2;*.vag;*.vc1;*.vc1test;*.vcd;*.video4linux2;*.v4l2;*.vivo;*.vmd;*.vob;*.vobsub;*.voc;*.vpk;*.vplayer;*.vqf;*.w64;*.wav;*.wc3movie;*.webm;*.webm_chunk;*.webm_dash_manifest;*.webp;*.webp_pipe;*.webvtt;*.wsaud;*.wsd;*.wsvqa;*.wtv;*.wv;*.wve;*.x11grab;*.xa;*.xbin;*.xmv;*.xv;*.xvag;*.xwma;*.yop;*.yuv4mpegpipe";

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
            this.OptionsConfiguration.AddConstantCallProperty("CommandLineOutput");
            InitializeComponent();

            // Check for updates.
            if (OctokitWrapper.CurrentVersionStatus == OctokitWrapper.VersionStatus.behind)
            {
                var updateMessageBoxResult = MessageBox.Show(string.Format("{0} {1}", Localization.Localization.BehindVersionStatusDescription, Localization.Localization.BehindVersionQuestion), Localization.Localization.UpdateAvailableLabel, MessageBoxButton.YesNo, MessageBoxImage.Information);

                // If the user wants to update now, take them to the latest release on github and close this window.
                if(updateMessageBoxResult == MessageBoxResult.Yes)
                {
                    Process.Start("https://github.com/wagesj45/butterflow-ui/releases/latest");
                    this.Close();
                }
            }
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
            ofd.Multiselect = true;
            ofd.Filter = Localization.Localization.SupportedFileTypesLabel + "|" + SUPPORTED_FILE_TYPES;

            var result = ofd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                this.OptionsConfiguration.VideoInput = ofd.FileNames;
                this.ButterflowWrapper.Probe(ofd.FileName);

                if (ofd.FileNames.Count() == 1)
                {
                    this.txtFileName.Text = ofd.FileName;
                    this.mediaPreview.Source = new Uri(ofd.FileName);

                    //Hack to get the first frame to display in the media preview element.
                    this.mediaPreview.Play();
                    this.mediaPreview.PausePlayback();
                }
                else
                {
                    this.txtFileName.Text = Localization.Localization.MultipleFilesText;
                    this.mediaPreview.Source = default(Uri);
                }
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

        /// <summary> Event handler. Called by btnCancel for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ButterflowWrapper.Cancel();
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
            var ofd = new OpenFileDialog();
            ofd.Filter = Localization.Localization.ButterflowUIConfigurationLabel + "|*.bui";

            var result = ofd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                var binaryFormatter = new BinaryFormatter();
                var file = binaryFormatter.Deserialize(ofd.OpenFile());
                if(file is OptionsConfigurationFile)
                {
                    this.OptionsConfiguration.LoadFile((OptionsConfigurationFile)file);
                }
            }
        }

        /// <summary> Event handler. Called by menuSaveConfiguration for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Localization.Localization.ButterflowUIConfigurationLabel + "|*.bui";

            var result = sfd.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(sfd.OpenFile(), this.OptionsConfiguration.ToFile());
            }
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
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }

        /// <summary> Event handler. Called by menuOptions for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void menuOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();

            optionsWindow.Show();
        }

        #endregion
    }
}
