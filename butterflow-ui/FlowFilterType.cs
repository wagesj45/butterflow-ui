using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary> Values that represent a flow filter type used for optical flow calculations. </summary>
    [Serializable]
    public enum FlowFilterType
    {
        /// <summary> Box. </summary>
        box,
        /// <summary> Gaussian. </summary>
        gaussian
    }
}
