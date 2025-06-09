namespace VehicleServiceBook.Models.Exceptions

{

    // Remove the empty shell class (not needed)

    // public class CustomExceptions { }



    public abstract class AppException : Exception

    {

        protected AppException(string message) : base(message) { }

    }



    public class NotFoundException : AppException

    {

        public NotFoundException(string name, object key)

          : base($"{name} with ID {key} was not found.") { }

    }



    public class BadRequestException : AppException

    {

        public BadRequestException(string message) : base(message) { }

    }



    public class UnauthorizedException : AppException

    {

        public UnauthorizedException(string action)

          : base($"Unauthorized: {action}") { }

    }

}