
namespace Operations_Platform_Assignment.Operations
{
    public class OperationParameter
    {
        public required string Name { get; init; }

        public required string Label { get; init; }

        public required string Type { get; init; }

        public bool Required { get; init; } = true;
    }
}
