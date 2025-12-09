using Library.Shared.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Library.Shared.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PositiveAttribute : Attribute
    {
    }

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


        public static void ValidateModel(object model)
        {
            if (model == null) throw new BadRequestException("Model cannot be null.");

            var errors = new List<string>();
            var props = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var value = prop.GetValue(model);

                //Required check
                if (Attribute.IsDefined(prop, typeof(RequiredAttribute)))
                {
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                        errors.Add($"{prop.Name} is required.");
                }

                //String Length check
                var strLen = prop.GetCustomAttribute<StringLengthAttribute>();
                if (strLen != null && value is string strValue)
                {
                    if (strValue.Length > strLen.MaximumLength)
                        errors.Add($"{prop.Name} cannot be longer than {strLen.MaximumLength} characters.");
                    if (strValue.Length < strLen.MinimumLength)
                        errors.Add($"{prop.Name} must be at least {strLen.MinimumLength} characters.");
                }

                //Positive integers (optional helper reuse)
                if (Attribute.IsDefined(prop, typeof(PositiveAttribute)) && value is int intValue && intValue <= 0)
                {
                    errors.Add($"{prop.Name} must be greater than zero.");
                }
            }

            if (errors.Any())
                throw new BadRequestException(string.Join(Environment.NewLine, errors));
        }
    }
}
