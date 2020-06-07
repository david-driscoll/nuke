// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common.Tooling;

namespace Nuke.Common.Tools.MSBuild
{
    partial class MSBuildTasks
    {
        private static string GetToolPath()
        {
            return MSBuildToolPathResolver.Resolve();
        }
        
        internal static void CustomLogger(OutputType type, string output)
        {
            if (type == OutputType.Err)
            {
                Logger.Error(output);
                return;
            }

            var spaces = 0;
            for (var i = 0; i < output.Length && spaces < 3; i++)
            {
                if (output[i] == ' ')
                {
                    spaces++;
                    continue;
                }

                if (i >= 4 &&
                    'e' == output[i - 4] &&
                    'r' == output[i - 3] &&
                    'r' == output[i - 2] &&
                    'o' == output[i - 1] &&
                    'r' == output[i])
                {
                    Logger.Error(output);
                    return;
                }

                if (i >= 6 &&
                    'w' == output[i - 6] &&
                    'a' == output[i - 5] &&
                    'r' == output[i - 4] &&
                    'n' == output[i - 3] &&
                    'i' == output[i - 2] &&
                    'n' == output[i - 1] &&
                    'g' == output[i])
                {
                    Logger.Warn(output);
                    return;
                }
            }

            Logger.Normal(output);
        }
    }
}
