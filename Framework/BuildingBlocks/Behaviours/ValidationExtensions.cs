using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Behaviours;

public static class ValidationExtensions
{
   
    // https://khalidabuhakmeh.com/minimal-api-validation-with-fluentvalidation
    public class Validated<T>
    {
        private ValidationResult Validation { get; }

        private Validated(T value, ValidationResult validation)
        {
            Value = value;
            Validation = validation;
        }

        public T Value { get; }
        public bool IsValid => Validation.IsValid;

        public IDictionary<string, string[]> Errors =>
            Validation
                .Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

        public void Deconstruct(out bool isValid, out T value)
        {
            isValid = IsValid;
            value = Value;
        }

        // ReSharper disable once UnusedMember.Global
        public static async ValueTask<Validated<T>> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            // only JSON is supported right now, no complex model binding
            var value = await context.Request.ReadFromJsonAsync<T>();
            var validator = context.RequestServices.GetRequiredService<IValidator<T>>();

            if (value is null) {
                throw new ArgumentException(parameter.Name);
            }

            var results = await validator.ValidateAsync(value);

            return new Validated<T>(value, results);
        }
    }
    
    // OpenAi
    public static async Task<IResult?> ValidateRequest<T>(this T model, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(model);
        return result.IsValid
            ? null
            : Results.ValidationProblem(result.ToDictionary());
    }

}

// Using
// app.MapPost("/users", async (UserDto user, IValidator<UserDto> validator) =>
// {
//     var validationResult = await user.ValidateRequest(validator);
//     if (validationResult is not null) return validationResult;
//
//     return Results.Ok("User is valid!");
// });

// app.MapPost("/person", (Validated<Person> req) =>
// {
//     // deconstruct to bool & Person
//     var (isValid, value) = req;
//
//     return isValid 
//         ? Ok(value) 
//         : ValidationProblem(req.Errors);
// });