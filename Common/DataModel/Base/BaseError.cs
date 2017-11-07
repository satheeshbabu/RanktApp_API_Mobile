namespace DataModel.Base
{
    public class BaseError
    {
        //TODO create Logger
        
        public const int Success = 0;
        public const int Fail = 100;

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public BaseError()
        {
            ErrorCode = Success;
            ErrorMessage = "";
        }

        public BaseError(int errorCode)
        {
            ErrorCode = errorCode;
            ErrorMessage = "";
        }

        public BaseError(string errorMessage)
        {
            ErrorCode = Fail;
            ErrorMessage = errorMessage;
        }

        public BaseError(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}