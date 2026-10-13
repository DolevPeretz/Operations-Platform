
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;
using Operations_Platform_Assignment.Operations;

namespace Operations_Platform_Assignment.Services;

public sealed class OperationValidator
{
    public void Validate(
        IOperation operation,
        JsonElement parameters)
    {
        var errors = new Dictionary<string, string[]>();

        if (parameters.ValueKind != JsonValueKind.Object)
        {
            errors["parameters"] =
                ["Parameters must be a JSON object."];

            throw new OperationValidationException(errors);
        }

        foreach (var parameter in operation.Parameters)
        {
            if (!parameters.TryGetProperty(
                    parameter.Name,
                    out var value))
            {
                if (parameter.Required)
                {
                    errors[parameter.Name] =
                        [$"'{parameter.Name}' is required."];
                }

                continue;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                if (parameter.Required)
                {
                    errors[parameter.Name] =
                        [$"'{parameter.Name}' cannot be null."];
                }

                continue;
            }

            bool isValid = parameter.Type switch
            {
                "number" =>
                    value.ValueKind == JsonValueKind.Number &&
                    value.TryGetDouble(out var number) &&
                    double.IsFinite(number),

                "text" =>
                    value.ValueKind == JsonValueKind.String,

                _ => false
            };

            if (!isValid)
            {
                errors[parameter.Name] =
                    [$"'{parameter.Name}' must be a valid {parameter.Type}."];
            }
        }

        if (errors.Count > 0)
        {
            throw new OperationValidationException(errors);
        }
    }
}
