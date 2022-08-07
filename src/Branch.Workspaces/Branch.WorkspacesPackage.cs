using Branch.Workspaces.Core;
using Branch.Workspaces.Infrastructure;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                _workSpaces = new BranchWorkSpaces(new GitService(), new JsonStorageService());

                if (await VS.Solutions.IsOpenAsync())
                {
                    var solution = await VS.Solutions.GetCurrentSolutionAsync();
                    HandleOpenSolution(solution);
                }

                VS.Events.SolutionEvents.OnBeforeCloseSolution += HandleCloseSolution;
                VS.Events.SolutionEvents.OnAfterOpenSolution += HandleOpenSolution;
                VS.Events.ShellEvents.ShutdownStarted += HandleCloseSolution;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
            }
        }

        private void HandleCloseSolution()
        {

            _ = JoinableTaskFactory
                .RunAsync(async () =>
                {
                    try
                    {
                        await JoinableTaskFactory.SwitchToMainThreadAsync();

                        var solution = await VS.Solutions.GetCurrentSolutionAsync();

                        foreach (var item in await VS.Windows.GetAllDocumentWindowsAsync())
                        {
                            var doc = await item.GetDocumentViewAsync();

                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Error.WriteLineAsync(ex.Message);
                    }
                });
        }

        private void HandleOpenSolution(Solution solution)
        {
            _ = JoinableTaskFactory
                .RunAsync(async () =>
                await _workSpaces.OnNewSolutionOpened(solution.FullPath));
        }
    }
}
