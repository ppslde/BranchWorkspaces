using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Branch.Workspaces.Core;
using Branch.Workspaces.Infrastructure;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace Branch.Workspaces.Plugin
{
    [Guid("d74c7bff-800c-4ccc-904a-dd695dc36f8c")]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class BranchWorkspacesPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            //await JoinableTaskFactory.SwitchToMainThreadAsync();
            if (await IsSolutionLoadedAsync())
            {
                HandleOpenSolution();
            }

            var x = new BranchWorkSpaces(new GitService(), new JsonStorageService());

            SolutionEvents.OnAfterOpenSolution += HandleOpenSolution;
        }


        private async Task<bool> IsSolutionLoadedAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Assumes.Present(solService);

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private void HandleOpenSolution(object sender = null, EventArgs e = null)
        {
            // Handle the open solution and try to do as much work
            // on a background thread as possible
        }

        private void HandleOpenSolution()
        {
            JoinableTaskFactory.Run(async () =>
            {

            });

            // Handle the open solution and try to do as much work
            // on a background thread as possible
        }
    }
}
