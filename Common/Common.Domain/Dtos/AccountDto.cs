namespace Common.Domain
{
    public class AccountDto : BaseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public CAccountType AccountType { get; set; }
        public CUserStatus Status { get; set; }
        public DocumentProperty Avatar { get; set; }
    }

    public class SaveAccountDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public CAccountType AccountType { get; set; }
    }
}
