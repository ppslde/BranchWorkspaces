using System.Threading.Tasks;
using Branch.Workspaces.Core.Models;

namespace Branch.Workspaces.Core.Interfaces
{
    public interface IPersistenceService
    {
        Task<BranchWorkspace> LoadWorkspace(BranchWorkspace workspace);
        Task SaveWorkspace(BranchWorkspace workspace);
    }
}
