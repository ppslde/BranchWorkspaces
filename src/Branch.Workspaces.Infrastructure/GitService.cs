using System.IO;
using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;
using Branch.Workspaces.Core.Models;
using LibGit2Sharp;

namespace Branch.Workspaces.Infrastructure
{
    public class GitService : IVersionControlService
    {
        public Task<VersionControlInfo> GetRepositoryInfosAsync(string repositoryFolder)
        {
            if (!Directory.Exists(Path.Combine(repositoryFolder, ".git")) && !Repository.IsValid(repositoryFolder))
                return null;

            using (Repository repo = new Repository(path: repositoryFolder))
            {
                return Task.FromResult(new VersionControlInfo
                {
                    Name = repo.Head.FriendlyName
                });
            };
        }


    }
}
