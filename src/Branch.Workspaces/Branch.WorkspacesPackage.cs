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
    public sealed class BranchWorkspacesPackage : ToolkitPackage
    {
        private BranchWorkSpaces _workSpaces;
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                _workSpaces = new BranchWorkSpaces(new GitService(), new JsonStorageService(@"C:\Tmp\ws.json"));

                VS.Events.SolutionEvents.OnAfterBackgroundSolutionLoadComplete += HandlePostSolutionLoading;
                VS.Events.SolutionEvents.OnBeforeCloseSolution += HandleCloseSolution;
                VS.Events.ShellEvents.ShutdownStarted += HandleCloseSolution;

                if (await VS.Solutions.IsOpenAsync())
                    HandlePostSolutionLoading();
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
            }
        }

        private void HandlePostSolutionLoading()
        {
            _ = JoinableTaskFactory
               .RunAsync(async () =>
               {
                   try
                   {
                       var workspace = await _workSpaces.LoadSolutionItems(await VS.Solutions.GetWorkspaceSolutionAsync());
                       if (workspace == null)
                           return;

                       await VS.Windows.SetWorkspaceDocumentsAsync(workspace.Documents, GetServiceAsync);
                       await VS.Debugger.SetWorkspaceBreakpointsAsync(workspace.Breakpoints, GetServiceAsync);
                   }
                   catch (Exception ex)
                   {
                       await ex.LogAsync();
                   }
               });
        }

        private void HandleCloseSolution()
        {
            _ = JoinableTaskFactory
                .RunAsync(async () =>
                {
                    try
                    {
                        await VS.Solutions.GetWorkspaceTreeItemsAsync(GetServiceAsync);

                        var solution = await VS.Solutions.GetWorkspaceSolutionAsync();
                        var documents = await VS.Windows.GetWorkspaceDocumentsAsync();
                        var breakpoints = await VS.Debugger.GetWorkspaceBreakpointsAsync(GetServiceAsync);

                        await _workSpaces.SaveSolutionItems(solution, documents, breakpoints);
                    }
                    catch (Exception ex)
                    {
                        await ex.LogAsync();
                    }
                });
        }
    }
}
