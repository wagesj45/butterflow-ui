using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace butterflow_ui
{
    /// <summary>
    /// Interaction logic for ButterflowOption.xaml
    /// </summary>
    public partial class ButterflowOption : UserControl
    {
        #region Properties

        public string LabelValue
        {
            get
            {
                return (string)GetValue(LabelValueProperty);
            }
            set
            {
                SetValue(LabelValueProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty LabelValueProperty = DependencyProperty.Register("LabelProperty", typeof(string), typeof(ButterflowOption));

        #endregion

        public ButterflowOption()
        {
            InitializeComponent();
        }
    }
}
