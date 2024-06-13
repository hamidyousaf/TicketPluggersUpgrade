namespace TP.Upgrade.Application.DTOs.Response
{
    public class ResponseModel
    {
        public ResponseModel(bool isSuccess = false, string message = "", object data = null)
        {
            Message = message;
            IsSuccess = isSuccess;
            Data = data;
        }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
    }
}
