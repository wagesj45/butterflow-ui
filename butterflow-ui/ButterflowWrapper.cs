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
    public class ButterflowWrapper : PropertyChangedAlerter
    {
        #region Members

        /// <summary> Full pathname of the butterflow executable file. </summary>
        private Lazy<string> executablePath = new Lazy<string>(() => Path.Combine(Directory.GetCurrentDirectory(), "ThirdPartyCompiled", "butterflow.exe"));
        /// <summary> The console output from butterflow. </summary>
        private string consoleOutput = string.Empty;
        /// <summary> Event queue for all listeners interested in ConsoleOutputRecieved events. </summary>
        //public event EventHandler<ButterflowConsoleOutputArgs> ConsoleOutputRecieved;

        #endregion

        #region Properties

        /// <summary> Gets the console output from butterflow. </summary>
        /// <value> The console output from butterflow. </value>
        public string ConsoleOutput
        {
            get
            {
                return this.consoleOutput;
            }
            private set
            {
                this.consoleOutput = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary> Runs butterflow with the given <paramref name="optionsConfiguration"/>. </summary>
        /// <param name="optionsConfiguration"> The options configuration. </param>
        public void Run(OptionsConfiguration optionsConfiguration)
        {
            string arguments = optionsConfiguration.ToButterflowArguments();

            Run(arguments);
        }

        /// <summary> Probes a video file. </summary>
        /// <param name="videoFile"> The video file to be probed. </param>
        public void Probe(string videoFile)
        {
            string arguments = string.Format("-prb \"{0}\"", videoFile);
            Run(arguments);
        }

        /// <summary> Runs butterflow with the given <paramref name="arguments"/>. </summary>
        /// <param name="arguments">     Options for controlling the operation. </param>
        private void Run(string arguments)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(executablePath.Value, arguments);

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += ProcessOutputDataReceived;

            process.Start();
            process.BeginOutputReadLine();
        }

        /// <summary> Process the output data received from the butterflow executable. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Data received event information. </param>
        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.ConsoleOutput += string.Format("{0}{1}", e.Data, Environment.NewLine);
            //OnConsoleOutputRecieved(e.Data);
        }

        /// <summary> Executes the console output recieved event handler. </summary>
        /// <param name="output"> The output that has been recieved from butterflow. </param>
        //protected void OnConsoleOutputRecieved(string output)
        //{
        //    if (this.ConsoleOutputRecieved != null)
        //    {
        //        this.ConsoleOutputRecieved(this, new ButterflowConsoleOutputArgs(output));
        //    }
        //}

        #endregion

        #region Subclasses

        public class ButterflowConsoleOutputArgs : EventArgs
        {
            #region Properties

            /// <summary> Gets the console output. </summary>
            /// <value> The console output. </value>
            public string ConsoleOutput { get; private set; }

            #endregion

            #region Constructors

            /// <summary> Constructor. </summary>
            /// <param name="consoleOutput"> The console output. </param>
            public ButterflowConsoleOutputArgs(string consoleOutput)
            {
                this.ConsoleOutput = consoleOutput;
            }

            #endregion
        }

        #endregion
    }
}
