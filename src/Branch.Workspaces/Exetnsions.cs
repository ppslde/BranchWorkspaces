using Branch.Workspaces.Core.Models;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Branch.Workspaces.Plugin
{
    static class Exetnsions
    {

        public static async Task GetWorkspaceTreeItemsAsync(this Community.VisualStudio.Toolkit.Solutions s, Func<Type, Task<object>> serviceProvider)
        {
            //TODO: Find out, how to get the hierarchy, as the items are empty after the root object!
            var dte = (EnvDTE80.DTE2)await serviceProvider(typeof(DTE));
            Assumes.Present(dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            UIHierarchy solutionExplorer = dte.ToolWindows.SolutionExplorer;
            if (solutionExplorer.UIHierarchyItems.Count <= 0)
                return;

            UIHierarchyItem rootNode = solutionExplorer.UIHierarchyItems.Item(1);

            Collapse(rootNode, ref solutionExplorer);

            foreach (UIHierarchyItem item in dte.ToolWindows.SolutionExplorer.UIHierarchyItems)
            {


            }



            //foreach (Project item in dte.Solution.Projects)
            //{

            //}
        }

        public static void Collapse(UIHierarchyItem item, ref UIHierarchy solutionExplorer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (UIHierarchyItem innerItem in item.UIHierarchyItems)
            {
                if (innerItem.UIHierarchyItems.Count > 0)
                {
                    Collapse(innerItem, ref solutionExplorer);
                    if (innerItem.UIHierarchyItems.Expanded)
                    {
                        innerItem.UIHierarchyItems.Expanded = false;
                        if (innerItem.UIHierarchyItems.Expanded)
                        {
                            innerItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                            solutionExplorer.DoDefaultAction();
                        }
                    }
                }
            }
        }

        public static async Task<BranchWorkspaceSolution> GetWorkspaceSolutionAsync(this Community.VisualStudio.Toolkit.Solutions s, Community.VisualStudio.Toolkit.Solution solution = null)
        {
            solution = solution ?? await s.GetCurrentSolutionAsync();

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

        public static async Task SetWorkspaceDocumentsAsync(this Community.VisualStudio.Toolkit.Windows w, IEnumerable<BranchWorkspaceDocument> documents, Func<Type, Task<object>> serviceProvider)
        {

            var dte = (EnvDTE80.DTE2)await serviceProvider(typeof(DTE));
            Assumes.Present(dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            dte.Documents.CloseAll(vsSaveChanges.vsSaveChangesNo);
            foreach (var doc in documents)
            {
                dte.Documents.Open(doc.Path);
            }
            //            dte.Documents.CloseAll(vsSaveChanges.vsSaveChangesNo);
            //            foreach (var document in documents)

            //                // dte.ExecuteCommand("File.OpenFile", document.Path);
            //                dte.Documents.Open(document.Path);
            //.            
        }

        public static async Task<IEnumerable<BranchWorkspaceBreakpoint>> GetWorkspaceBreakpointsAsync(this Community.VisualStudio.Toolkit.Debugger d, Func<Type, Task<object>> serviceProvider)
        {
            var dte = (EnvDTE80.DTE2)await serviceProvider(typeof(DTE));
            Assumes.Present(dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            return dte.Debugger.Breakpoints.Cast<Breakpoint>()
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
