
namespace Operations_Platform_Assignment.Contracts;

public sealed class OperationValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public OperationValidationException(
        IDictionary<string, string[]> errors)
        : base("Operation parameters are invalid.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}
