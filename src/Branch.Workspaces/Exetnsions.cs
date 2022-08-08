using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Models;
using Microsoft;
using Microsoft.VisualStudio.Shell;

namespace Branch.Workspaces.Plugin
{
    static class Exetnsions
    {

        public static async Task<BranchWorkspaceSolution> GetWorkspaceSolutionAsync(this Community.VisualStudio.Toolkit.Solutions s)
        {
            var solution = await s.GetCurrentSolutionAsync();

            return new BranchWorkspaceSolution
            {
                Path = solution.FullPath,
                SolutionFile = Path.GetFileName(solution.FullPath)
            };
        }

        public static async Task<IEnumerable<BranchWorkspaceDocument>> GetWorkspaceDocumentsAsync(this Community.VisualStudio.Toolkit.Windows w)
        {
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var documents = new List<BranchWorkspaceDocument>();
            foreach (var item in await w.GetAllDocumentWindowsAsync())
            {
                var doc = await item.GetDocumentViewAsync();

                documents.Add(new BranchWorkspaceDocument
                {
                    Path = doc.FilePath
                });
            }

            return documents;
        }

        public static async Task<IEnumerable<BranchWorkspaceBreakpoint>> GetWorkspaceBreakpointsAsync(this Community.VisualStudio.Toolkit.Debugger d, Func<Type, Task<object>> serviceProvider)
        {
            var dte = (EnvDTE80.DTE2)await serviceProvider(typeof(EnvDTE.DTE));
            Assumes.Present(dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            return dte.Debugger.Breakpoints.Cast<EnvDTE.Breakpoint>()
                .Select(b =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return new BranchWorkspaceBreakpoint
                    {
                        File = b.File,
                        Line = b.FileLine,
                        Column = b.FileColumn,
                        Type = b.Type.ToString(),
                        Condition = b.Condition,
                        ConditionType = b.ConditionType.ToString()
                    };
                })
                .ToArray();
        }
    }
}
