using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace butterflow_ui
{
    public class ButterflowWrapper
    {
        #region Members

        /// <summary> Full pathname of the butterflow executable file. </summary>
        private Lazy<string> executablePath = new Lazy<string>(() => Path.Combine(Assembly.GetExecutingAssembly().Location, "ThirdPartyCompiled", "butterflow.exe"));

        #endregion

        #region Methods

        /// <summary> Runs butterflow with the given <paramref name="optionsConfiguration"/>. </summary>
        /// <param name="optionsConfiguration"> The options configuration. </param>
        public void Run(OptionsConfiguration optionsConfiguration)
        {
            string arguments = optionsConfiguration.ToButterflowArguments();

            Run(arguments);
        }

        public void Probe(string videoFile)
        {
            string arguments = string.Format("-prb \"{0}\"", videoFile);
        }

        /// <summary> Runs butterflow with the given <paramref name="arguments"/>. </summary>
        /// <param name="arguments">     Options for controlling the operation. </param>
        private void Run(string arguments)
        {
            var processStartInfo = new ProcessStartInfo(executablePath.Value, arguments);

            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
        }

        #endregion
    }
}
