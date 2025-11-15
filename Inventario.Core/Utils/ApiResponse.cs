namespace Inventario.Core.Utils
{
    public class ApiResponse<T> where T : class
    {

        public ApiResponse(T? data)
        {
            Success = true;
            Data = data;
        }

        public ApiResponse(List<string> message)
        {
            Success = false;
            Message = message;
        }

        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string>? Message { get; set; }

    }
}