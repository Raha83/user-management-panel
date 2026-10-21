namespace ManagementPanel
{
    public class Permission
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string code { get; set; } = null!;
        public string? description { get; set; }
    }
}