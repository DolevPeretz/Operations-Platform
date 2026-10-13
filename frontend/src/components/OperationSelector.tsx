
import type { Operation } from "../types/operation";

interface OperationSelectorProps {
    operations: Operation[];
    selectedOperationId: string;
    disabled: boolean;
    onChange: (operationId: string) => void;
}

function OperationSelector({
    operations,
    selectedOperationId,
    disabled,
    onChange,
}: OperationSelectorProps) {
    return (
        <>
        <label htmlFor= "operation-select" >
        Select Operation
            </label>

            < select
    id = "operation-select"
    value = { selectedOperationId }
    disabled = { disabled }
    onChange = {(event) => onChange(event.target.value)
}
      >
{
    operations.map((operation) => (
        <option key= { operation.id } value = { operation.id } >
        { operation.name }
        </option>
    ))
}
    </select>
    </>
  );
}

export default OperationSelector;
