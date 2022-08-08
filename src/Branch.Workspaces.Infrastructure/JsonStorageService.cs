using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;

namespace Branch.Workspaces.Infrastructure
{
    public class JsonStorageService : IPersistenceService
    {
        public Task GetSolutionAsync(string solutionIdentifier)
        {
            //throw new System.NotImplementedException();

            return Task.CompletedTask;
        }
    }
}
