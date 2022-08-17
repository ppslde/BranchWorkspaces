﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Models;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;

namespace Branch.Workspaces.Plugin
{
    static class Extensions
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
                dte.Documents.Open(doc.Path);
        }

        public static async Task SetWorkspaceBreakpointsAsync(this Community.VisualStudio.Toolkit.Debugger d, IEnumerable<BranchWorkspaceBreakpoint> workspaceBreakpoints, Func<Type, Task<object>> serviceProvider)
        {
            var dte = (EnvDTE80.DTE2)await serviceProvider(typeof(DTE));
            Assumes.Present(dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            foreach (var item in dte.Debugger.Breakpoints.Cast<Breakpoint>())
                item.Delete();

            foreach (var b in workspaceBreakpoints)
                dte.Debugger.Breakpoints.Add(
                    File: b.File,
                    Line: b.Line,
                    Column: b.Column,
                    Condition: b.Condition,
                    ConditionType: (dbgBreakpointConditionType)Enum.Parse(typeof(dbgBreakpointConditionType), b.ConditionType)
                );

            foreach (var b in workspaceBreakpoints.Where(b => !b.Enabled))
            {
                var dbg = dte.Debugger.Breakpoints
                    .Cast<Breakpoint>()
                    .SingleOrDefault(db =>
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        return db.File == b.File && db.FileLine == b.Line && db.FileColumn == b.Column;
                    });

                dbg.Enabled = b.Enabled;
            }
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
                        ConditionType = b.ConditionType.ToString(),
                        Enabled = b.Enabled
                    };
                })
                .ToArray();
        }
    }
}