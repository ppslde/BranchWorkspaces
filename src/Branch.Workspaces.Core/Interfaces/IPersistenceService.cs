using System;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Models;

namespace Branch.Workspaces.Core.Interfaces
{
    public interface IPersistenceService : IDisposable
    {
        Task<BranchWorkspace> LoadWorkspace(BranchWorkspace workspace);
        Task SaveWorkspace(BranchWorkspace workspace);
    }
}
