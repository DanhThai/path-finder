
namespace Common.Domain
{
    public enum CErrorCode : int
    {
        Unknown = 0,
        InvalidInput = 1001,
        Duplication = 1002,
        ResourceNotExist = 1003,
        StatusNotSupport = 1004,
        BusinessError = 1005,
        Required = 10006,
        ForbiddenModify = 10007
    }
}
