
interface OperationResultProps {
    data: unknown;
}

function OperationResult({ data }: OperationResultProps) {
    function formatResult(value: unknown): string {
        if (value === null || value === undefined) {
            return "No result";
        }

        return typeof value === "object"
            ? JSON.stringify(value, null, 2)
            : String(value);
    }

    return (
        <section className= "card" >
        <h2>Result </h2>
        < pre className = "result" > { formatResult(data) } </pre>
            </section>
  );
}

export default OperationResult;
