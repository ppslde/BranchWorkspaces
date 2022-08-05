using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Branch.Workspaces.Core;
using Branch.Workspaces.Infrastructure;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace Branch.Workspaces.Plugin
{
    [Guid("d74c7bff-800c-4ccc-904a-dd695dc36f8c")]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class BranchWorkspacesPackage : AsyncPackage
    {
        private BranchWorkSpaces _workSpaces;
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            //await JoinableTaskFactory.SwitchToMainThreadAsync();

            if (await VS.Solutions.IsOpenAsync())
            {
                var solution = await VS.Solutions.GetCurrentSolutionAsync();
                HandleOpenSolution(solution);
            }

            _workSpaces = new BranchWorkSpaces(new GitService(), new JsonStorageService());

            VS.Events.SolutionEvents.OnAfterOpenSolution += HandleOpenSolution;
        }


        private void HandleOpenSolution(Solution solution)
        {
            JoinableTaskFactory
                .RunAsync(async () => await _workSpaces.OnNewSolutionOpened(solution.FullPath))
                .FireAndForget();
        }
    }
}
