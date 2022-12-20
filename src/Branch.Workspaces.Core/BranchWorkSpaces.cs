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
                var workspace = await GetSolutionInfos(solution);
                return await _persistenceService.LoadWorkspace(workspace);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading branched solution: {solution.SolutionFile}", ex);
            }
        }

        public async Task SaveSolutionItems(BranchWorkspaceSolution solution, IEnumerable<BranchWorkspaceDocument> openFiles, IEnumerable<BranchWorkspaceBreakpoint> breakpoints)
        {
            try
            {
                var workspace = await GetSolutionInfos(solution);
                if (workspace == null)
                    return;

                workspace.Solution = solution;
                workspace.Documents = openFiles;
                workspace.Breakpoints = breakpoints;
                workspace.Updated = DateTimeOffset.Now;

                await _persistenceService.SaveWorkspace(workspace);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving branched solution: {solution.SolutionFile}", ex);
            }
        }

        private async Task<BranchWorkspace> GetSolutionInfos(BranchWorkspaceSolution solution)
        {
            var workspace = await _versionControlService.GetRepositoryInfosAsync(solution);
            return workspace ?? new BranchWorkspace
            {
                Id = solution.Path,
                Name = solution.Path,
                Display = solution.SolutionFile,
                GitDir = null,
                WorkDir = Path.GetDirectoryName(solution.Path)
            };
        }
    }
}
