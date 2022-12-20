using System.Threading.Tasks;
using Branch.Workspaces.Core.Models;

namespace Branch.Workspaces.Core.Interfaces
{
    public interface IVersionControlService
    {
        Task<BranchWorkspace> GetRepositoryInfosAsync(BranchWorkspaceSolution solution);
    }
}
