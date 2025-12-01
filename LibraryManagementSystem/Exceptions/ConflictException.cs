using System;

namespace LibraryManagementSystem.Exceptions
{
    public class ConflictException : ApiException
    {
        public ConflictException(string message) : base(message) { }
    }
}
