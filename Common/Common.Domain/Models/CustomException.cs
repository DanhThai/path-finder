
namespace Common.Domain
{
    public class CustomExceptionBase : Exception
    {
        public int Code { get; set; }
        public CustomExceptionBase() : base()
        {
        }

        public CustomExceptionBase(string message) : base(message)
        {
        }
        public CustomExceptionBase(string message, int code) : base(message)
        {
            Code = code;
        }
    }

    public class AccessDeniedException : CustomExceptionBase
    {
        public AccessDeniedException() : base()
        {
        }

        public AccessDeniedException(string message) : base(message)
        {
        }
    }

    public class Error500Exception : CustomExceptionBase
    {
        public Error500Exception(string message) : base(message)
        {
        }
    }

    public class Error404Exception : CustomExceptionBase
    {
        public Error404Exception() : base()
        {
        }
    }

    public class ErrorHandleException : CustomExceptionBase
    {
        public ErrorHandleException(string message, CErrorCode code = CErrorCode.Unknown) : base(message, (int)code)
        {
        }
    }

    public class DataValidationException : CustomExceptionBase
    {
        public string ValidateKey { get; set; }
        public DataValidationException(string message, string key, CErrorCode code = CErrorCode.Unknown) : base(message, (int)code)
        {
            ValidateKey = key;
        }
    }

    public class NotExistException : CustomExceptionBase
    {
        public NotExistException(string sourceName, CErrorCode code = CErrorCode.Unknown) : base($"The {sourceName} doesn't exist.", (int)code)
        {
        }
    }

    public class WarningHandleException : CustomExceptionBase
    {
        public WarningHandleException(string message) : base(message)
        {
        }
    }

    public enum ValidateType : int
    {
        NameExist = 0,
        NameRequired = 1,
        ExceedMaxLength = 2,
    }

    public class MigrateAndSeedDataException : CustomExceptionBase
    {
        public MigrateAndSeedDataException() : base()
        {

        }
    }
}
