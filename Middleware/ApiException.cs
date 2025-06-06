namespace VehicleServiceBook.Middleware
{
    // Your existing ApiException is good for general errors
    public class ApiException
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string? Details { get; }

        public ApiException(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }

    // New custom exception for a specific business rule violation (e.g., duplicate email)
    public class CustomValidationException : Exception
    {
        public int StatusCode { get; }

        public CustomValidationException(string message, int statusCode = 400) // Default to Bad Request
            : base(message)
        {
            StatusCode = statusCode;
        }

        public CustomValidationException(string message, Exception innerException, int statusCode = 400)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}