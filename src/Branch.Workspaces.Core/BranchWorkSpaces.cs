using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;
using Branch.Workspaces.Core.Models;

namespace Branch.Workspaces.Core
{
    public class BranchWorkSpaces
    {
        private readonly IVersionControlService _versionControlService;
        private readonly IPersistenceService _persistenceService;

        public BranchWorkSpaces(IVersionControlService versionControlService, IPersistenceService persistenceService)
        {
            _versionControlService = versionControlService;
            _persistenceService = persistenceService;
        }

        public async Task<BranchWorkspace> LoadSolutionItems(BranchWorkspaceSolution solution)
        {
            try
            {
                var workspace = await _versionControlService.GetRepositoryInfosAsync(Path.GetDirectoryName(solution.Path));
                return await _persistenceService.LoadWorkspace(workspace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public async Task SaveSolutionItems(BranchWorkspaceSolution solution, IEnumerable<BranchWorkspaceDocument> openFiles, IEnumerable<BranchWorkspaceBreakpoint> breakpoints)
        {
            var workspace = await _versionControlService.GetRepositoryInfosAsync(Path.GetDirectoryName(solution.Path));
            workspace.Solution = solution;
            workspace.Documents = openFiles;
            workspace.Breakpoints = breakpoints;
            workspace.Updated = DateTimeOffset.Now;

            await _persistenceService.SaveWorkspace(workspace);
        }
    }
}
