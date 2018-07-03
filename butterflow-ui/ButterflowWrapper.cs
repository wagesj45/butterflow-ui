using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary> A butterflow wrapper. Provides interaction with the butterflow executable. </summary>
    public class ButterflowWrapper : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The RegEx string for matching probed resolution. </summary>
        private const string REGEX_RESOLUTION = @"Resolution\s*:\s(?<Width>\d+)x(?<Height>\d+)";
        /// <summary> The RegEx string for matching the probed playback rate.. </summary>
        private const string REGEX_RATE = @"Rate\s*:\s(?<Rate>\d+\.\d+) fps";

        /// <summary> Full pathname of the butterflow executable file. </summary>
        private Lazy<string> executablePath = new Lazy<string>(() => Path.Combine(Directory.GetCurrentDirectory(), "ThirdPartyCompiled", "butterflow.exe"));
        /// <summary> The console output from butterflow. </summary>
        private string consoleOutput = string.Empty;
        /// <summary> Event queue for all listeners interested in ParsedConsoleOutputRecieved events. </summary>
        public event EventHandler<ButterflowOutputArgs> ParsedConsoleOutputRecieved;

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
            process.OutputDataReceived += Process_OutputDataReceived; ;

            process.Start();
            process.BeginOutputReadLine();
        }

        /// <summary>
        /// Parses console output and attempts to find known values. If a known value is found, the
        /// <seealso cref="ParsedConsoleOutputRecieved"/> event is triggered.
        /// </summary>
        /// <param name="consoleOutput"> The console output from butterflow. </param>
        private void ParseConsoleOutput(string consoleOutput)
        {
            if (string.IsNullOrWhiteSpace(consoleOutput))
            {
                //Ignore null content and just escape.
                return;
            }

            //Test for resolution
            var regex = new Regex(REGEX_RESOLUTION);
            foreach (Match match in regex.Matches(consoleOutput))
            {
                var width = match.Groups["Width"].Value;
                var height = match.Groups["Height"].Value;

                OnParsedConsoleOutputRecieved(ButterflowOutputType.Width, width, consoleOutput);
                OnParsedConsoleOutputRecieved(ButterflowOutputType.Height, height, consoleOutput);
            }

            //Test for playback rate
            regex = new Regex(REGEX_RATE);
            foreach(Match match in regex.Matches(consoleOutput))
            {
                var rate = match.Groups["Rate"].Value;

                OnParsedConsoleOutputRecieved(ButterflowOutputType.Rate, rate, consoleOutput);
            }
        }

        /// <summary> Executes the parsed console output recieved action. </summary>
        /// <param name="outputType">    Type of the output. </param>
        /// <param name="value">         The value. </param>
        /// <param name="consoleOutput"> The console output from butterflow. </param>
        private void OnParsedConsoleOutputRecieved(ButterflowOutputType outputType, string value, string consoleOutput)
        {
            if (this.ParsedConsoleOutputRecieved != null)
            {
                this.ParsedConsoleOutputRecieved(this, new ButterflowOutputArgs(outputType, value, consoleOutput));
            }
        }

        /// <summary> Event handler. Called by Process for output data received events. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Data received event information. </param>
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.ConsoleOutput += string.Format("{0}{1}", e.Data, Environment.NewLine);
            ParseConsoleOutput(e.Data);
        }

        #endregion

        #region Subclasses

        /// <summary> Arguments for butterflow output events where a known value of type <seealso cref="ButterflowOutputType"/> can be parsed. </summary>
        public class ButterflowOutputArgs : ButterflowConsoleOutputArgs
        {

            #region Properties

            /// <summary> Gets or sets the type of the output detected from butterflow. </summary>
            /// <value> The type of the output detected from butterflow. </value>
            public ButterflowOutputType OutputType { get; private set; }

            /// <summary> Gets or sets the value detected from butterflow. </summary>
            /// <value> The value detected from butterflow. </value>
            public string Value { get; private set; }

            #endregion

            /// <summary> Constructor. </summary>
            /// <param name="outputType">    The type of the output detected from butterflow. </param>
            /// <param name="value">         The value detected from butterflow. </param>
            /// <param name="consoleOutput"> The console output. </param>
            public ButterflowOutputArgs(ButterflowOutputType outputType, string value, string consoleOutput)
                : base(consoleOutput)
            {
                this.OutputType = outputType;
                this.Value = value;
            }
        }

        /// <summary> Arguments for butterflow console output events. </summary>
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

        /// <summary> Values that represent butterflow output types. </summary>
        public enum ButterflowOutputType
        {
            /// <summary> Video Width. </summary>
            Width,
            /// <summary> Video Height. </summary>
            Height,
            /// <summary> Video playback rate. </summary>
            Rate,
            /// <summary> Video processing progress. </summary>
            Progress
        }

        #endregion
    }
}
