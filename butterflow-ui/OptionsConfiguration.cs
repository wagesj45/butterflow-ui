using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csmic;

namespace butterflow_ui
{

    /// <summary> The butterflow options configuration. Contians all the options necessary to run butterflow and process a video. </summary>
    public class OptionsConfiguration : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The default pyramid scale setting. </summary>
        private const decimal DEFAULT_PYRAMID_SCALE = 0.5m;
        /// <summary> The default levels setting. </summary>
        private const int DEFAULT_LEVELS = 3;
        /// <summary> The default window size setting. </summary>
        private const int DEFAULT_WINDOW_SIZE = 25;
        /// <summary> The default iterations setting. </summary>
        private const int DEFAULT_ITERATIONS = 3;
        /// <summary> The default pixel neighborhood setting. </summary>
        private const int DEFAULT_PIXEL_NEIGHBORHOOD = 5;
        /// <summary> The default smooth derivative standard deviation setting. </summary>
        private const decimal DEFAULT_SMOOTH_DERIVATIVE_STANDARD_DEVIATION = 1.1m;
        /// <summary> The default flow filter type setting. </summary>
        private const FlowFilterType DEFAULT_FLOW_FILTER_TYPE = FlowFilterType.box;
        /// <summary> The output file format when operating on more than one video. </summary>
        private const string OUTPUT_FILE_FORMAT = "{0}_{1}";

        /// <summary> An input interpreter used for converting string values to numeric values. </summary>
        [NonSerialized]
        private InputInterpreter interpreter = new InputInterpreter();
        /// <summary> The aspect ratio used for calculating heights when the aspect ratio is locked. </summary>
        private decimal aspectRatio = 0;

        /// <summary> The playback rate. </summary>
        private string playbackRate;
        /// <summary> A value indicating whether or not to keep the original audio in the final video. </summary>
        private bool keepAudio;
        /// <summary> The width of the output video. </summary>
        private int width;
        /// <summary> The height of the output video. </summary>
        private int height;
        /// <summary> A value indicating whether or not to render unspecified subregions. </summary>
        private bool keepSubregions;
        /// <summary> A value indicating whether or not to render the final video with lossless quality. </summary>
        private bool losslessQuality;
        /// <summary> A value indicating whether or not to tune processing for smooth motion. </summary>
        private bool smoothMotion;
        /// <summary> A value indicating whether or not to lock the aspect ratio to the <seealso cref="width"/> of the video. </summary>
        private bool lockAspectRatio;
        /// <summary> The video input files. </summary>
        private IEnumerable<string> videoInput;
        /// <summary> The video output file. </summary>
        private string videoOutput;
        /// <summary> A value indicating whether or not to use fast pyramids when processing a video. </summary>
        private bool fastPyramid;
        /// <summary> The pyramid scale setting. </summary>
        private decimal pyramidScale;
        /// <summary> The level size setting. </summary>
        private int levels;
        /// <summary> Size of the windowing average. </summary>
        private int windowSize;
        /// <summary> The number of iterations per pyramid level. </summary>
        private int iterations;
        /// <summary> The size of pixel neighborhood. </summary>
        private int pixelNeighborhood;
        /// <summary> The standard deviation of smooth derivatives </summary>
        private decimal smoothDerivativeStandardDeviation;
        /// <summary> Type of the flow filter to use for processing. </summary>
        private FlowFilterType flowFilterType = FlowFilterType.box;
        /// <summary> The subregions of the video on which to process. </summary>
        private ObservableCollection<ButterflowSubregion> subregions = new ObservableCollection<ButterflowSubregion>();

        #endregion

        #region Properties

        /// <summary> Gets the command line output given the current configuration. </summary>
        /// <value> The command line output. </value>
        public string CommandLineOutput
        {
            get
            {
                return ToButterflowArguments();
            }
        }

        /// <summary> Gets a value indicating whether butterflow will process multiple files. </summary>
        /// <value> True if butterflow will process multiple files, false if not. </value>
        public bool MultipleFiles
        {
            get
            {
                return this.VideoInput != null && this.VideoInput.Count() > 1;
            }
        }

        /// <summary> Gets or sets the playback rate. </summary>
        /// <value> The playback rate. </value>
        public string PlaybackRate
        {
            get
            {
                return this.playbackRate;
            }
            set
            {
                this.playbackRate = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether the keep audio. </summary>
        /// <value> True if keeping audio in the final video, false if not. </value>
        public bool KeepAudio
        {
            get
            {
                return this.keepAudio;
            }
            set
            {
                this.keepAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether the butterflow should be turned toward smooth motion. </summary>
        /// <value> True if tuned toward smooth motion, false if not. </value>
        public bool SmoothMotion
        {
            get
            {
                return this.smoothMotion;
            }
            set
            {
                this.smoothMotion = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether to lock aspect ratio of the video. </summary>
        /// <value> True if locking aspect ratio of the video, false if not. </value>
        public bool LockAspectRatio
        {
            get
            {
                return this.lockAspectRatio;
            }
            set
            {
                if (value && this.width != 0 && this.height != 0)
                {
                    this.aspectRatio = Convert.ToDecimal(this.height) / Convert.ToDecimal(this.width);
                }

                this.lockAspectRatio = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the width of the video output. </summary>
        /// <value> The width of the video output. </value>
        public string Width
        {
            get
            {
                return this.width.ToString();
            }
            set
            {
                var oldWidth = this.width;

                interpreter.Interpret(value);
                this.width = interpreter.Int;

                OnPropertyChanged();

                if (this.lockAspectRatio)
                {
                    interpreter.Interpret(string.Format("{0} * {1}", this.aspectRatio, this.width));
                    this.height = interpreter.Int;

                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary> Gets or sets the height of the video output. </summary>
        /// <value> The height of the video output. </value>
        public string Height
        {
            get
            {
                return this.height.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.height = interpreter.Int;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether the keep subregions that are not explicitly specified. </summary>
        /// <value> True if keeping subregions not explicitly specified, false if not. </value>
        public bool KeepSubregions
        {
            get
            {
                return this.keepSubregions;
            }
            set
            {
                this.keepSubregions = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether the result is rendered in lossless quality. </summary>
        /// <value> True if lossless quality is selected, false if not. </value>
        public bool LosslessQuality
        {
            get
            {
                return this.losslessQuality;
            }
            set
            {
                this.losslessQuality = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the video input file path. </summary>
        /// <value> The video input file path. </value>
        public IEnumerable<string> VideoInput
        {
            get
            {
                return this.videoInput;
            }
            set
            {
                this.videoInput = value;
                OnPropertyChanged();
                OnPropertyChanged("MultipleFiles");
            }
        }

        /// <summary> Gets or sets the video output file path. </summary>
        /// <value> The video output file path. </value>
        public string VideoOutput
        {
            get
            {
                return this.videoOutput;
            }
            set
            {
                this.videoOutput = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether to use fast pyramids. </summary>
        /// <value> True if using fast pyramids, false if not. </value>
        public bool FastPyramid
        {
            get
            {
                return this.fastPyramid;
            }
            set
            {
                this.fastPyramid = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the pyramid scale factor. </summary>
        /// <value> The pyramid scale factor. </value>
        public string PyramidScale
        {
            get
            {
                return this.pyramidScale.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.pyramidScale = interpreter.Decimal;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the number of pyramid layers. </summary>
        /// <value> The number of pyramid layers. </value>
        public string Levels
        {
            get
            {
                return this.levels.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.levels = interpreter.Int;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the size of the windowing average. </summary>
        /// <value> The size of the windowing average. </value>
        public string WindowSize
        {
            get
            {
                return this.windowSize.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.windowSize = interpreter.Int;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the number of iterations at each pyramid level. </summary>
        /// <value> The number of iterations at each pyramid level. </value>
        public string Iterations
        {
            get
            {
                return this.iterations.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.iterations = interpreter.Int;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the size of the pixel neighborhood. </summary>
        /// <value> The size of the pixel neighborhood. </value>
        /// <remarks> Per butterflow's documentation, the valid range for --poly-n is {5,7}. </remarks>
        public string PixelNeighborhood
        {
            get
            {
                return this.pixelNeighborhood.ToString();
            }
            set
            {
                interpreter.Interpret(value);

                if (interpreter.Int >= 5 && interpreter.Int <= 7)
                {
                    this.pixelNeighborhood = interpreter.Int;
                }

                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the standard deviation of smooth derivatives. </summary>
        /// <value> The standard deviation of smooth derivatives. </value>
        public string SmoothDerivativeStandardDeviation
        {
            get
            {
                return this.smoothDerivativeStandardDeviation.ToString();
            }
            set
            {
                interpreter.Interpret(value);
                this.smoothDerivativeStandardDeviation = interpreter.Decimal;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the type of the flow filter used for optical flow calculations. </summary>
        /// <value> The type of the flow filter used for optical flow calculations. </value>
        public FlowFilterType FlowFilterType
        {
            get
            {
                return this.flowFilterType;
            }
            set
            {
                this.flowFilterType = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the subregions of the video on which to work. </summary>
        /// <value> The subregions of the video. </value>
        public ObservableCollection<ButterflowSubregion> Subregions
        {
            get
            {
                return this.subregions;
            }
            set
            {
                this.subregions = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Contructors

        /// <summary> Default constructor. </summary>
        public OptionsConfiguration()
        {
            // Set default values
            this.pyramidScale = DEFAULT_PYRAMID_SCALE;
            this.levels = DEFAULT_LEVELS;
            this.windowSize = DEFAULT_WINDOW_SIZE;
            this.iterations = DEFAULT_ITERATIONS;
            this.pixelNeighborhood = DEFAULT_PIXEL_NEIGHBORHOOD;
            this.smoothDerivativeStandardDeviation = DEFAULT_SMOOTH_DERIVATIVE_STANDARD_DEVIATION;
            this.flowFilterType = DEFAULT_FLOW_FILTER_TYPE;

            this.videoInput = new string[0];

            this.subregions.CollectionChanged += Subregions_CollectionChanged; ;
        }

        /// <summary> Event handler. Called by Subregions for collection changed events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Notify collection changed event information. </param>
        private void Subregions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ButterflowSubregion newItem in e.NewItems)
                {
                    newItem.PropertyChanged += SubregionPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (ButterflowSubregion oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= SubregionPropertyChanged;
                }
            }

            OnPropertyChanged("CommandLineOutput");
        }

        #endregion

        #region Methods

        private void SubregionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CommandLineOutput");
        }

        /// <summary> Converts this object to a <seealso cref="OptionsConfigurationFile"/>. </summary>
        /// <returns> This object as an OptionsConfigurationFile. </returns>
        public OptionsConfigurationFile ToFile()
        {
            var file = new OptionsConfigurationFile()
            {
                FastPyramid = this.fastPyramid,
                FlowFilterType = this.flowFilterType,
                Iterations = this.iterations,
                KeepAudio = this.keepAudio,
                KeepSubregions = this.keepSubregions,
                Levels = this.levels,
                LockAspectRatio = this.lockAspectRatio,
                LosslessQuality = this.losslessQuality,
                PixelNeighborhood = this.pixelNeighborhood,
                PlaybackRate = this.playbackRate,
                PyramidScale = this.pyramidScale,
                SmoothDerivativeStandardDeviation = this.smoothDerivativeStandardDeviation,
                SmoothMotion = this.smoothMotion,
                WindowSize = this.windowSize
            };

            return file;
        }

        /// <summary> Loads an option configuration file's contents into the <seealso cref="OptionsConfiguration"/>. </summary>
        /// <param name="file"> The file to load. </param>
        public void LoadFile(OptionsConfigurationFile file)
        {
            this.FastPyramid = file.FastPyramid;
            this.FlowFilterType = file.FlowFilterType;
            this.Iterations = file.Iterations.ToString();
            this.KeepAudio = file.KeepAudio;
            this.KeepSubregions = file.KeepSubregions;
            this.Levels = file.Levels.ToString();
            this.LockAspectRatio = file.LockAspectRatio;
            this.LosslessQuality = file.LosslessQuality;
            this.PixelNeighborhood = file.PixelNeighborhood.ToString();
            this.PlaybackRate = file.PlaybackRate;
            this.PyramidScale = file.PyramidScale.ToString();
            this.SmoothDerivativeStandardDeviation = file.SmoothDerivativeStandardDeviation.ToString();
            this.SmoothMotion = file.SmoothMotion;
            this.WindowSize = file.WindowSize.ToString();
        }

        /// <summary> Converts this object to a butterflow options. </summary>
        /// <returns> This object as a string. </returns>
        public string ToButterflowArguments(int videoInputIndex = 0)
        {
            var stringBuilder = new StringBuilder("-v "); // Verbose

            if (this.LockAspectRatio)
            {
                stringBuilder.AppendFormat("-vs {0}:-1 ", this.Width);
            }
            else
            {
                stringBuilder.AppendFormat("-vs {0}:{1} ", this.Width, this.Height);
            }

            if (!string.IsNullOrWhiteSpace(this.PlaybackRate)) stringBuilder.AppendFormat("-r {0} ", this.PlaybackRate);
            if (this.KeepAudio) stringBuilder.Append("-audio ");
            if (this.LosslessQuality) stringBuilder.Append("-l ");

            if (this.Subregions.Any())
            {
                stringBuilder.Append("-s ");

                foreach (var anon in this.Subregions.Select((sr, index) => new { Index = index, Subregion = sr }))
                {
                    string format = "ss\\.fff";

                    if (anon.Index > 0)
                    {
                        stringBuilder.Append(":");
                    }

                    if (anon.Subregion.Start.TotalHours > 1)
                    {
                        format = "h\\:m\\:s\\.fff";
                    }
                    else if (anon.Subregion.Start.TotalMinutes > 1)
                    {
                        format = "m\\:s\\.fff";
                    }

                    stringBuilder.AppendFormat("a={0},b={1},{2}={3}", anon.Subregion.Start.ToString(format), anon.Subregion.ToEnd ? "end" : anon.Subregion.End.ToString(format), anon.Subregion.SubregionType, anon.Subregion.Value);
                }
                stringBuilder.Append(" ");
            }

            if (this.KeepSubregions) stringBuilder.Append("-k ");
            if (this.SmoothMotion) stringBuilder.Append("-sm ");
            if (this.FastPyramid) stringBuilder.Append("--fast-pyr ");
            if (this.pyramidScale != DEFAULT_PYRAMID_SCALE) stringBuilder.AppendFormat("--pyr-scale {0} ", this.PyramidScale);
            if (this.levels != DEFAULT_LEVELS) stringBuilder.AppendFormat("--levels {0} ", this.Levels);
            if (this.windowSize != DEFAULT_WINDOW_SIZE) stringBuilder.AppendFormat("--winsize {0} ", this.WindowSize);
            if (this.iterations != DEFAULT_ITERATIONS) stringBuilder.AppendFormat("--iters {0} ", this.Iterations);
            if (this.pixelNeighborhood != DEFAULT_PIXEL_NEIGHBORHOOD) stringBuilder.AppendFormat("--poly-n {0} ", this.PixelNeighborhood);
            if (this.smoothDerivativeStandardDeviation != DEFAULT_SMOOTH_DERIVATIVE_STANDARD_DEVIATION) stringBuilder.AppendFormat("--poly-s {0} ", this.SmoothDerivativeStandardDeviation);
            if (this.FlowFilterType != DEFAULT_FLOW_FILTER_TYPE) stringBuilder.AppendFormat("-ff {0} ", this.FlowFilterType);

            if (!string.IsNullOrWhiteSpace(this.VideoOutput))
            {
                string videoOutputFile = string.Empty;

                if (this.MultipleFiles)
                {
                    var format = new StringBuilder(Path.GetFileNameWithoutExtension(this.VideoOutput));
                    format.Append("_{0:");
                    for (int i = 0; i < this.videoInput.Count().ToString().Length; i++)
                    {
                        format.Append("0");
                    }
                    format.Append("}");
                    format.Append(Path.GetExtension(this.VideoOutput));

                    var newName = string.Format(format.ToString(), videoInputIndex + 1); // Since the index is zero based, we will add one to make the output more human readable.

                    videoOutputFile = Path.Combine(Path.GetDirectoryName(this.VideoOutput), newName);
                }
                else
                {
                    videoOutputFile = this.VideoOutput;
                }

                stringBuilder.AppendFormat("-o \"{0}\" ", videoOutputFile);
            }

            if (this.VideoInput.Any())
            {
                stringBuilder.AppendFormat("\"{0}\"", this.VideoInput.ElementAt(videoInputIndex));
            }

            return stringBuilder.ToString();
        }

        /// <summary> Returns a string that represents the current object. </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return ToButterflowArguments();
        }

        #endregion
    }
}
