using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary> An options configuration file. This class is used to serialize out the configuration to a file. </summary>
    [Serializable]
    public class OptionsConfigurationFile
    {
        #region Properties

        /// <summary> Gets or sets the playback rate. </summary>
        /// <value> The playback rate. </value>
        public string PlaybackRate { get; set; }

        /// <summary> Gets or sets a value indicating whether the keep audio. </summary>
        /// <value> True if keep audio, false if not. </value>
        public bool KeepAudio { get; set; }

        /// <summary> Gets or sets a value indicating whether the keep subregions that are not explicitly specified. </summary>
        /// <value> True if keeping subregions not explicitly specified, false if not. </value>
        public bool KeepSubregions { get; set; }

        /// <summary> Gets or sets a value indicating whether the result is rendered in lossless quality. </summary>
        /// <value> True if lossless quality is selected, false if not. </value>
        public bool LosslessQuality { get; set; }

        /// <summary> Gets or sets a value indicating whether the butterflow should be turned toward smooth motion. </summary>
        /// <value> True if tuned toward smooth motion, false if not. </value>
        public bool SmoothMotion { get; set; }

        /// <summary> Gets or sets a value indicating whether to lock aspect ratio of the video. </summary>
        /// <value> True if locking aspect ratio of the video, false if not. </value>
        public bool LockAspectRatio { get; set; }

        /// <summary> Gets or sets a value indicating whether to use fast pyramids. </summary>
        /// <value> True if using fast pyramids, false if not. </value>
        public bool FastPyramid { get; set; }

        /// <summary> Gets or sets the pyramid scale factor. </summary>
        /// <value> The pyramid scale factor. </value>
        public decimal PyramidScale { get; set; }

        /// <summary> Gets or sets the number of pyramid layers. </summary>
        /// <value> The number of pyramid layers. </value>
        public int Levels { get; set; }

        /// <summary> Gets or sets the size of the windowing average. </summary>
        /// <value> The size of the windowing average. </value>
        public int WindowSize { get; set; }

        /// <summary> Gets or sets the number of iterations at each pyramid level. </summary>
        /// <value> The number of iterations at each pyramid level. </value>
        public int Iterations { get; set; }

        /// <summary> Gets or sets the size of the pixel neighborhood. </summary>
        /// <value> The size of the pixel neighborhood. </value>
        /// <remarks> Per butterflow's documentation, the valid range for --poly-n is {5,7}. </remarks>
        public int PixelNeighborhood { get; set; }

        /// <summary> Gets or sets the standard deviation of smooth derivatives. </summary>
        /// <value> The standard deviation of smooth derivatives. </value>
        public decimal SmoothDerivativeStandardDeviation { get; set; }

        /// <summary> Gets or sets the type of the flow filter used for optical flow calculations. </summary>
        /// <value> The type of the flow filter used for optical flow calculations. </value>
        public FlowFilterType FlowFilterType { get; set; }

        #endregion
    }
}
