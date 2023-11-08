using System.Collections.Generic;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Components;
using Vipentti.Nuke.Components;
using static Vipentti.Nuke.Components.StandardNames;

namespace build;

[GitHubActions(
    "pull-request",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = new[] { MainBranch, DevelopBranch },
    PublishArtifacts = false,
    FetchDepth = 0, // fetch full history
    InvokedTargets = new[]
    {
        nameof(ITest.Test),
        nameof(IUseLinters.InstallLinters),
        nameof(IUseLinters.Lint),
        nameof(IValidatePackages.ValidatePackages),
    })]
[StandardPublishGitHubActions(
    "publish",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest
    , OnPushBranches = new[] { MainBranch }
)]
class Build : StandardNukeBuild, IUseCsharpier
{
    public Solution CurrentSolution => From<IHazSolution>().Solution;

    public override string OriginalRepositoryName { get; } = "SharpDotEnv";
    public override string MainReleaseBranch { get; } = MainBranch;
    public override IEnumerable<Project> ProjectsToPack => new[]
    {
        CurrentSolution.GetSolutionFolder("src").GetProject("SharpDotEnv"),
        CurrentSolution.GetSolutionFolder("src").GetProject("SharpDotEnv.Extensions.Configuration"),
    };

    public override IEnumerable<IProvideLinter> Linters => new[]
    {
        From<IUseDotNetFormat>().Linter,
        From<IUseCsharpier>().Linter,
    };
    bool IUseCsharpier.UseGlobalTool { get; } = true;

    public override IEnumerable<Project> TestProjects => CurrentSolution.GetAllProjects("*Tests*");

    // Support plugins are available for:
    //   - JetBrains ReSharper        https://nuke.build/resharper
    //   - JetBrains Rider            https://nuke.build/rider
    //   - Microsoft VisualStudio     https://nuke.build/visualstudio
    //   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.From<ICompile>().Compile);
}
