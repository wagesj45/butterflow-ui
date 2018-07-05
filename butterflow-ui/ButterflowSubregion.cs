using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary> A butterflow video subregion. Represents a snippet of a video on which butterflow will act. </summary>
    [Serializable]
    public class ButterflowSubregion : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The start of the subregion. </summary>
        private TimeSpan start = TimeSpan.Zero;
        /// <summary> The end of the subregion. </summary>
        private TimeSpan end = TimeSpan.Zero;
        /// <summary> Type of opersion to perform on the subregion. </summary>
        private RegionType subregionType;
        /// <summary> The value targeted for the subregion. </summary>
        private decimal value;
        /// <summary> True if the subregion runs to the end, false if not. </summary>
        private bool toEnd;
        /// <summary> A unique identifier used to locate this subregion withing a subregion collection. </summary>
        private Guid identifier = new Guid();

        #endregion

        #region Properties

        /// <summary> Gets or sets the start of the subregion. </summary>
        /// <value> The start of the subregion. </value>
        public TimeSpan Start
        {
            get
            {
                return this.start;
            }
            set
            {
                this.start = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the end of the subregion. </summary>
        /// <value> The end of the subregion. </value>
        public TimeSpan End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.end = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the operation to be performed on the subregion. </summary>
        /// <value> The operation to be performed on subregion. </value>
        public RegionType SubregionType
        {
            get
            {
                return this.subregionType;
            }
            set
            {
                this.subregionType = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the targeted value of the subregion. </summary>
        /// <value> The value targeted for the subregion. </value>
        public decimal Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether the subregion runs to the end of the video. </summary>
        /// <value> True if the subregion runs to the end, false if not. </value>
        public bool ToEnd
        {
            get
            {
                return this.toEnd;
            }
            set
            {
                this.toEnd = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets a unique identifier used to locate this subregion withing a subregion collection. </summary>
        /// <value> The unique identifier. </value>
        public Guid Identifier
        {
            get
            {
                return this.identifier;
            }
            private set
            {
                this.identifier = value;
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
