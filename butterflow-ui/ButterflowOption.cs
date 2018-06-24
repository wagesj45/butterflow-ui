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

        public string LabelValue { get; set; }

        #endregion

        #region Dependency Properties

        public static DependencyProperty LabelValueProperty = DependencyProperty.Register("LabelValue", typeof(string), typeof(ButterflowOption));

        #endregion

        public ButterflowOption()
        {
            //
        }
    }
}
