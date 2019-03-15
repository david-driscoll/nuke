// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.Git;
using Octokit;

namespace Nuke.Common.Utilities
{
    internal static class ExternalFilesUtility
    {
        public static IEnumerable<string> GetDownloadUrls(string url)
        {
            var client = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)));
            // var repo = await client.Repository.Get("nuke-build", "nuke");
            // var treeResponse = await client.Git.Tree.GetRecursive("nuke-build", "nuke", repo.DefaultBranch);
            //
            // treeResponse.Tree
            //     .Where(x => x.Type == TreeType.Blob)
            //     .ForEach(x => Logger.Info(x.Path));

            var repo2 = GitRepository.FromUrl("https://github.com/nuke-build/nuke/tree/master/.teamcity");
            Colorful.Console.WriteLine(repo2.GetGitHubOwner());
            Colorful.Console.WriteLine(repo2.GetGitHubName());
            Colorful.Console.WriteLine(repo2.Branch);

            return null;
        }
    }
}
