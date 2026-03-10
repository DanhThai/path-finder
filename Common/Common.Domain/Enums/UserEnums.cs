
namespace Common.Domain
{
    public enum CUserStatus
    {
        Active = 0,
        Inactive = 1,
    }

    public enum CAccountType
    {
        Admin = 0,
        Learner = 1 << 0,
    }
}
