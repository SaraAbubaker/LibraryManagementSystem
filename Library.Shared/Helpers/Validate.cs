using Library.Shared.Exceptions;

namespace Library.Shared.Helpers
{
    public static class Validate
    {
        public static void NotNull(object? value, string name)
        {
            if (value == null)
                throw new BadRequestException($"{name} is required.");
        }

        public static void NotEmpty(string? value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException($"{name} is required.");
        }

        public static void Positive(int value, string name)
        {
            if (value <= 0)
                throw new BadRequestException($"{name} must be greater than zero.");
        }

        public static void Exists(object? entity, string name)
        {
            if (entity == null)
                throw new NotFoundException($"{name} not found.");
        }
    }
}
