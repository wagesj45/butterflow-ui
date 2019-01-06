using csmic;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace butterflow_ui
{
    /// <summary>
    /// An wrapper for the Octokit.net class. This class is used to extract functionality for
    /// checking github for updates to butterflow-ui installation.
    /// </summary>
    public static class OctokitWrapper
    {
        #region Members

        /// <summary> The RegEx string for matching . </summary>
        private const string REGEX_VERSION = @"(?<Major>\d+ ?)\.(?<Minor>\d+ ?)\.(?<Patch>\d+ ?)";

        /// <summary> The version status of the current installation. </summary>
        private static VersionStatus versionStatus = VersionStatus.Unknown;

        #endregion

        #region Properties

        /// <summary> Gets the current version status of this installation. </summary>
        /// <value> The current version status of this installation. </value>
        public static VersionStatus CurrentVersionStatus
        {
            get
            {
                return versionStatus;
            }
        }

        /// <summary> Gets information describing the current version status. </summary>
        /// <value> Information describing the current version status. </value>
        public static string CurrentVersionStatusDescription
        {
            get
            {
                switch (CurrentVersionStatus)
                {
                    case VersionStatus.Current:
                        return Localization.Localization.CurrentVersionStatusDescription;
                    case VersionStatus.Behind:
                        return Localization.Localization.BehindVersionStatusDescription;
                    case VersionStatus.Custom:
                        return Localization.Localization.CustomVersionStatusDescription;
                    case VersionStatus.Unknown:
                    default:
                        return Localization.Localization.UnknownVersionStatusDescription;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary> Static constructor. </summary>
        static OctokitWrapper()
        {
            versionStatus = GetVersionStatus();
        }

        #endregion

        #region Methods

        /// <summary> Gets version status from github. </summary>
        /// <returns> The current version status. </returns>
        private static VersionStatus GetVersionStatus()
        {
            try
            {
                var interpreter = new InputInterpreter();
                var client = new GitHubClient(new ProductHeaderValue("butterflow-ui"));
                var releases = client.Repository.Release.GetAll("wagesj45", "butterflow-ui").Result;

                if (releases.Any())
                {
                    var latest = releases.First();
                    decimal latestMajor = 0, latestMinor = 0, latestPatch = 0, currentMajor = 0, currentMinor = 0, currentPatch = 0;

                    var regex = new Regex(REGEX_VERSION);
                    foreach (Match match in regex.Matches(latest.TagName))
                    {
                        latestMajor = interpreter.ComputeExpression(match.Groups["Major"].Value);
                        latestMinor = interpreter.ComputeExpression(match.Groups["Minor"].Value);
                        latestPatch = interpreter.ComputeExpression(match.Groups["Patch"].Value);
                    }

                    foreach (Match match in regex.Matches(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                    {
                        currentMajor = interpreter.ComputeExpression(match.Groups["Major"].Value);
                        currentMinor = interpreter.ComputeExpression(match.Groups["Minor"].Value);
                        currentPatch = interpreter.ComputeExpression(match.Groups["Patch"].Value);
                    }

                    if (latestMajor == currentMajor && latestMinor == currentMinor && latestPatch == currentPatch)
                    {
                        return VersionStatus.Current;
                    }

                    if (latestMajor >= currentMajor && latestMinor >= currentMinor && latestPatch >= currentPatch)
                    {
                        return VersionStatus.Behind;
                    }

                    return VersionStatus.Custom;
                }
            }
            catch(Exception e)
            {
                //There was an issue connecting to Github. This could be caused by a missing network connection.
                //We can safely ignore an error in this process and proceed, falling through to the default connection
                //value of Unknown.
            }

            return VersionStatus.Unknown;
        }

        #endregion

        #region Subclasses

        /// <summary> Values that represent version status of the current installation of butterflow-ui. </summary>
        public enum VersionStatus
        {
            /// <summary> The current version is up to date with the github repository. </summary>
            Current,
            /// <summary> The current version is behind the github repository and should be updated. </summary>
            Behind,
            /// <summary> The current version is ahead of the github repository, or is a custom version of butterflow-ui that cannot be compared to the github repository. </summary>
            Custom,
            /// <summary> Github failed to respond with the current version. This could be because of rate limits or a network failure. </summary>
            Unknown
        }

        #endregion
    }
}
