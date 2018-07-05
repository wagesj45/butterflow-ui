using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary> An options configuration file. </summary>
    [Serializable]
    public class OptionsConfigurationFile
    {
        #region Properties

        public string PlaybackRate { get; set; }
        public bool KeepAudio { get; set; }
        public bool KeepSubregions { get; set; }
        public bool LosslessQuality { get; set; }
        public bool SmoothMotion { get; set; }
        public bool LockAspectRatio { get; set; }
        public bool FastPyramid { get; set; }
        public decimal PyramidScale { get; set; }
        public int Levels { get; set; }
        public int WindowSize { get; set; }
        public int Iterations { get; set; }
        public int PixelNeighborhood { get; set; }
        public decimal SmoothDerivativeStandardDeviation { get; set; }
        public FlowFilterType FlowFilterType { get; set; }

        #endregion
    }
}
