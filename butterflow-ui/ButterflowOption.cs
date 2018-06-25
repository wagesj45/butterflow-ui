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

        public string DescriptionValue { get; set; }

        public Visibility ToolTipVisibility
        {
            get
            {
                //return string.IsNullOrWhiteSpace(this.DescriptionValue) ? Visibility.Hidden : Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty LabelValueProperty = DependencyProperty.Register("LabelValue", typeof(string), typeof(ButterflowOption));
        public static DependencyProperty DescriptionValueProperty = DependencyProperty.Register("DescriptionValue", typeof(string), typeof(ButterflowOption));
        public static DependencyProperty ToolTipVisibilityProperty = DependencyProperty.Register("ToolTipVisibility", typeof(Visibility), typeof(ButterflowOption));

        #endregion

        public ButterflowOption()
        {
            //
        }
    }
}
