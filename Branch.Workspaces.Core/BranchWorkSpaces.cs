using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;

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



        }
    }
}
