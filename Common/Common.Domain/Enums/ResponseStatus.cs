namespace Common.Domain
{
    public enum ResponseStatus
    {
        Successed = 0,
        RequestError = 101,
        Unauthorized = 102,
        ModelInfoInvalid = 103,
        AccessDenied = 104,
        WarningInfo = 105,
        ValidationFailed = 106,
        Error500Page = 107
    }
}
