using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csmic;

namespace butterflow_ui
{

    /// <summary> (Serializable) the options configuration. </summary>
    [Serializable]
    public class OptionsConfiguration : PropertyChangedAlerter
    {
        #region Members

        /// <summary> An interpreter used to ensure numeric input is correctly calculated. </summary>
        private InputInterpreter interpreter = new InputInterpreter();

        private string playbackRate;
        private bool keepAudio;
        private int width;
        private int height;
        private bool keepAspectRatio;
        private bool losslessQuality;
        private string videoInput;
        private string videoOutput;
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
        /// <value> True if keep audio, false if not. </value>
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
                interpreter.Interpret(value);
                this.width = interpreter.Int;
                OnPropertyChanged();
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

        /// <summary> Gets or sets a value indicating whether the keep aspect ratio of the input video file for the output video file. </summary>
        /// <value> True if keep aspect ratio, false if not. </value>
        public bool KeepAspectRatio
        {
            get
            {
                return this.keepAspectRatio;
            }
            set
            {
                this.keepAspectRatio = value;
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
        public string VideoInput
        {
            get
            {
                return this.videoInput;
            }
            set
            {
                this.videoInput = value;
                OnPropertyChanged();
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
            AddConstantCallProperty("CommandLineOutput");

            this.subregions.CollectionChanged += SubregionsCollectionChanged;
        }

        private void SubregionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null)
            {
                foreach(ButterflowSubregion newItem in e.NewItems)
                {
                    newItem.PropertyChanged += SubregionPropertyChanged;
                }
            }
            if(e.OldItems != null)
            {
                foreach(ButterflowSubregion oldItem in e.OldItems)
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

        /// <summary> Converts this object to a butterflow options. </summary>
        /// <returns> This object as a string. </returns>
        public string ToButterflowArguments()
        {
            var stringBuilder = new StringBuilder("-v "); //Verbose

            if (this.KeepAspectRatio)
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

            stringBuilder.AppendFormat("\"{0}\"", this.VideoInput);

            if (this.Subregions.Any())
            {
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
