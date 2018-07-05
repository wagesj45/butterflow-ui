using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace butterflow_ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary> Raises the <see cref="E:System.Windows.Application.Startup" /> event. </summary>
        /// <param name="e"> A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event
        ///                  data. </param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Set our localization to the users perfered language
            Thread.CurrentThread.CurrentCulture = butterflow_ui.Properties.Settings.Default.Language;
            Thread.CurrentThread.CurrentUICulture = butterflow_ui.Properties.Settings.Default.Language;

            base.OnStartup(e);
        }
    }
}
