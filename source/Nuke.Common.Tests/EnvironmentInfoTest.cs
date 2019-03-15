// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Nuke.Common.Tests
{
    public class ExternalFilesUtilityTest
    {
        [Theory]
        [InlineData("https://github.com/nuke-build/nuke", 10)]
        [InlineData("https://github.com/nuke-build/nuke/tree/master/.teamcity", 10)]
        public void TestGetFiles(string url, int count)
        {
            // ExternalFilesUtility.GetFiles(url).Should().HaveCount(count);
        }

    }

    public class EnvironmentInfoTest
    {
        [Theory]
        [InlineData("arg0 arg1 arg2", new[] { "arg0", "arg1", "arg2" })]
        [InlineData("\"arg0 arg1\" arg2", new[] { "arg0 arg1", "arg2" })]
        [InlineData("'arg0 arg1' arg2", new[] { "arg0 arg1", "arg2" })]
        [InlineData("'arg0 \"arg1' arg2", new[] { "arg0 \"arg1", "arg2" })]
        [InlineData("'arg0 \"arg1\"' arg2", new[] { "arg0 \"arg1\"", "arg2" })]
        [InlineData("\"arg0 'arg1\" arg2", new[] { "arg0 'arg1", "arg2" })]
        [InlineData("\"arg0 'arg1'\" arg2", new[] { "arg0 'arg1'", "arg2" })]
        [InlineData("\"arg0 \\\"arg1\\\"\" arg2", new[] { "arg0 \"arg1\"", "arg2" })]
        [InlineData("'arg0 \\'arg1\\'' arg2", new[] { "arg0 'arg1'", "arg2" })]
        [InlineData("\\\\ \\ \\\\", new[] { "\\\\", "\\", "\\\\" })]
        public void TestParseCommandLineArguments(string commandLine, string[] expected)
        {
            var arguments = EnvironmentInfo.ParseCommandLineArguments(commandLine);

            arguments.Should().BeEquivalentTo(expected);
        }
    }
}
