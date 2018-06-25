using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace butterflow_ui
{
    public class ButterflowOption : ContentControl
    {
        #region Properties

        /// <summary> Gets or sets the label value. </summary>
        /// <value> The label value. </value>
        public string LabelValue { get; set; }

        /// <summary> Gets or sets the description value. </summary>
        /// <value> The description value. </value>
        public string DescriptionValue { get; set; }

        #endregion

        #region Dependency Properties

        /// <summary> The label value property. </summary>
        public static DependencyProperty LabelValueProperty = DependencyProperty.Register("LabelValue", typeof(string), typeof(ButterflowOption));
        /// <summary> The description value property. </summary>
        public static DependencyProperty DescriptionValueProperty = DependencyProperty.Register("DescriptionValue", typeof(string), typeof(ButterflowOption));

        #endregion

        /// <summary> Default constructor. </summary>
        public ButterflowOption()
        {
            //
        }
    }
}
