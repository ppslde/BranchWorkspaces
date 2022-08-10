using System.Collections.Generic;

namespace Branch.Workspaces.Core.Models
{
    public class BranchWorkspace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
        public string WorkDir { get; set; }

        public BranchWorkspaceSolution Solution { get; set; }
        public IEnumerable<BranchWorkspaceDocument> Documents { get; set; }
        public IEnumerable<BranchWorkspaceBreakpoint> Breakpoints { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is BranchWorkspace ws))
                return false;

            return Id == ws.Id && ws.WorkDir == ws.WorkDir && ws.Name == ws.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ WorkDir.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
