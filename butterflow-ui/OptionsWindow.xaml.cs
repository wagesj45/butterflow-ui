using butterflow_ui.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace butterflow_ui
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        #region Properties

        /// <summary> The butterflow wrapper used to call butterflow. </summary>
        public ButterflowWrapper ButterflowWrapper { get; set; } = new ButterflowWrapper();

        /// <summary> Gets or sets the supported languages. </summary>
        /// <value> The supported languages. </value>
        public List<CultureInfo> SupportedLanguages { get; set; } = new List<CultureInfo>(new[]
        {
            CultureInfo.CreateSpecificCulture("en-US"),
            CultureInfo.CreateSpecificCulture("es"),
            CultureInfo.CreateSpecificCulture("ar"),
            CultureInfo.CreateSpecificCulture("ja"),
            CultureInfo.CreateSpecificCulture("ru"),
            CultureInfo.CreateSpecificCulture("zh-CN"),
        });

        #endregion Properties

        #region Constructors

        /// <summary> Default constructor. </summary>
        public OptionsWindow()
        {
            this.ButterflowWrapper.GetDevices();
            this.ButterflowWrapper.ButterflowExited += ButterflowWrapper_ButterflowExited;
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary> Event handler. Called by btnSave for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            this.Close();
        }

        /// <summary> Butterflow wrapper butterflow exited. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      The ButterflowExitArgs to process. </param>
        private void ButterflowWrapper_ButterflowExited(object sender, ButterflowWrapper.ButterflowExitArgs e)
        {
            if(Settings.Default.Device >= 0)
            {
                this.comboDeviceList.Dispatcher.Invoke(() => this.comboDeviceList.SelectedIndex = Settings.Default.Device);
            }
            else
            {
                this.comboDeviceList.Dispatcher.Invoke(() => this.comboDeviceList.SelectedIndex = 0);
            }
        }

        #endregion Methods
    }
}