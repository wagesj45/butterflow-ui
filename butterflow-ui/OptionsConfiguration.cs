using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csmic;

namespace butterflow_ui
{

    /// <summary> (Serializable) the options configuration. </summary>
    [Serializable]
    public class OptionsConfiguration : INotifyPropertyChanged
    {
        #region Members

        /// <summary> Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private InputInterpreter interpreter = new InputInterpreter();

        private string playbackRate;
        private bool keepAudio;
        private int width;
        private int height;
        private bool losslessQuality;

        #endregion

        #region Properties

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
                OnPropertyChanged("PlaybackRate");
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
                OnPropertyChanged("KeepAudio");
            }
        }

        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
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
                OnPropertyChanged("Width");
            }
        }

        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
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
                OnPropertyChanged("Height");
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
                OnPropertyChanged("LosslessQuality");
            }
        }

        #endregion

        #region Methods

        /// <summary> Executes the property changed action. </summary>
        /// <param name="name"> The name. </param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
