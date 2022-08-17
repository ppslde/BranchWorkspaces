namespace Branch.Workspaces.Core.Models
{
    public struct BranchWorkspaceBreakpoint
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Type { get; set; }
        public string Condition { get; set; }
        public string ConditionType { get; set; }
        public bool Enabled { get; set; }
    }
}
