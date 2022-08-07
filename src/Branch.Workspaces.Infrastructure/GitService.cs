using Branch.Workspaces.Core.Interfaces;
using Branch.Workspaces.Core.Models;
using LibGit2Sharp;
using System.IO;
using System.Threading.Tasks;

namespace Branch.Workspaces.Infrastructure
{
    public class GitService : IVersionControlService
    {
        public Task<VersionControlInfo> GetRepositoryInfosAsync(string repositoryFolder)
        {
            VersionControlInfo info = null;

            if (!Directory.Exists(Path.Combine(repositoryFolder, ".git")) && !Repository.IsValid(repositoryFolder))
                return Task.FromResult(info);

            using (Repository repo = new Repository(path: repositoryFolder))
            {
                info = new VersionControlInfo
                {
                    Id = repo.Refs[repo.Head.CanonicalName].TargetIdentifier,
                    Name = repo.Head.CanonicalName,
                    Display = repo.Head.FriendlyName,
                    WorkDir = repositoryFolder
                };
            }
            return Task.FromResult(info);
        }
    }
}
