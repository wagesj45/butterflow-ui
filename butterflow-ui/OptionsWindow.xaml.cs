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

        /// <summary> Gets or sets the supported languages. </summary>
        /// <value> The supported languages. </value>
        public List<CultureInfo> SupportedLanguages { get; set; } = new List<CultureInfo>(new[] { CultureInfo.CreateSpecificCulture("en-US"), CultureInfo.CreateSpecificCulture("es-ES") });

        #endregion

        #region Constructors

        /// <summary> Default constructor. </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary> Event handler. Called by btnSave for click events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            this.Close();
        }

        #endregion
    }
}
