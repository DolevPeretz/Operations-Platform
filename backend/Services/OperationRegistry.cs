
using Operations_Platform_Assignment.Operations;

namespace Operations_Platform_Assignment.Services
{
    public class OperationRegistry
    {
        private readonly Dictionary<string, IOperation> _operations;

        public OperationRegistry(IEnumerable<IOperation> operations)
        {
            _operations = operations.ToDictionary(
                operation => operation.Id,
                StringComparer.OrdinalIgnoreCase
            );
        }

        public IReadOnlyCollection<IOperation> GetAll()
        {
            return _operations.Values.ToList().AsReadOnly();
        }

        public IOperation? GetById(string id)
        {
            return _operations.TryGetValue(id, out var operation)
                ? operation
                : null;
        }
    }
}
