namespace Common.Domain
{
    public class CAPIResponseDto
    {
        /// <summary>
        /// values from StatusCodes
        /// </summary>
        public int Code { get; set; }
        public ResponseStatus Status { get; set; } = ResponseStatus.Successed;
        public string Message { get; set; }
        /// <summary>
        ///  valuses from ErrorCode
        /// </summary>
        public int ErrorCode { get; set; }
        public object Data { get; set; }
    }


    public class CAPIResponseValidationDto : CAPIResponseDto
    {
        public string ValidateKey { get; set; }
    }
}
