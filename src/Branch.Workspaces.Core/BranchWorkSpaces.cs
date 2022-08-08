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

        public async Task OnNewSolutionOpened(string solutionFile)
        {
            try
            {
                var solutionDir = Path.GetDirectoryName(solutionFile);
                var info = await _versionControlService.GetRepositoryInfosAsync(solutionDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }

        public async Task OnSolutionClose(BranchWorkspaceSolution solutionFile, IEnumerable<BranchWorkspaceDocument> openFiles, IEnumerable<BranchWorkspaceBreakpoint> breakpoints)
        {
            await _persistenceService.GetSolutionAsync("");
        }
    }
}
