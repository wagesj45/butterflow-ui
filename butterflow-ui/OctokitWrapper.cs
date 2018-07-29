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
        private static VersionStatus versionStatus = VersionStatus.unknown;

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
                    case VersionStatus.current:
                        return Localization.Localization.CurrentVersionStatusDescription;
                    case VersionStatus.behind:
                        return Localization.Localization.BehindVersionStatusDescription;
                    case VersionStatus.custom:
                        return Localization.Localization.CustomVersionStatusDescription;
                    case VersionStatus.unknown:
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
                    return VersionStatus.current;
                }

                if (latestMajor >= currentMajor && latestMinor >= currentMinor && latestPatch >= currentPatch)
                {
                    return VersionStatus.behind;
                }

                return VersionStatus.custom;
            }

            return VersionStatus.unknown;
        }

        #endregion

        #region Subclasses

        /// <summary> Values that represent version status of the current installation of butterflow-ui. </summary>
        public enum VersionStatus
        {
            /// <summary> The current version is up to date with the github repository. </summary>
            current,
            /// <summary> The current version is behind the github repository and should be updated. </summary>
            behind,
            /// <summary> The current version is ahead of the github repository, or is a custom version of butterflow-ui that cannot be compared to the github repository. </summary>
            custom,
            /// <summary> Github failed to respond with the current version. This could be because of rate limits or a network failure. </summary>
            unknown
        }

        #endregion
    }
}
