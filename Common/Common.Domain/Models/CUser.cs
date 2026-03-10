namespace Common.Domain
{
    public class CUser
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public CAccountType Type { get; set; }
        public CUserStatus Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
