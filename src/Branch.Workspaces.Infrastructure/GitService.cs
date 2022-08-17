using System.Threading.Tasks;
using Branch.Workspaces.Core.Interfaces;
using Branch.Workspaces.Core.Models;
using LibGit2Sharp;

namespace Branch.Workspaces.Infrastructure
{
    public class GitService : IVersionControlService
    {
        public Task<BranchWorkspace> GetRepositoryInfosAsync(string solutionFolder)
        {
            BranchWorkspace info = default;

            var nextGitFolder = Repository.Discover(solutionFolder);
            if (nextGitFolder == null || !Repository.IsValid(nextGitFolder))
                return Task.FromResult(info);

            using (Repository repo = new Repository(path: nextGitFolder))
            {
                info = new BranchWorkspace
                {
                    Id = repo.Refs[repo.Head.CanonicalName].TargetIdentifier,
                    Name = repo.Head.CanonicalName,
                    Display = repo.Head.FriendlyName,
                    GitDir = nextGitFolder,
                    WorkDir = solutionFolder
                };
            }
            return Task.FromResult(info);
        }
    }
}
