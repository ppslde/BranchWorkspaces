using System.Threading.Tasks;

namespace Branch.Workspaces.Core.Interfaces
{
    public interface IPersistenceService
    {
        Task GetSolutionAsync(string solutionIdentifier);
    }
}
