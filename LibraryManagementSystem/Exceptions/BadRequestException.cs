using System;

namespace LibraryManagementSystem.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string message) : base(message) { }
    }
}
