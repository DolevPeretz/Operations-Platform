
import { useState } from "react";
import type { FormEvent } from "react";
import type { Operation } from "../types/operation";

type FormValues = Record<string, string>;
type OperationParameters = Record<string, string | number>;

interface OperationFormProps {
    operation: Operation;
    isSubmitting: boolean;
    onExecute: (parameters: OperationParameters) => Promise<void>;
}

function buildParameters(
    operation: Operation,
    values: FormValues
): OperationParameters {
    const parameters: OperationParameters = {};

    for (const parameter of operation.parameters) {
        const value = values[parameter.name] ?? "";

        if (parameter.required && value.trim() === "") {
            throw new Error(`${parameter.label} is required.`);
        }

        if (value.trim() === "") {
            continue;
        }

        if (parameter.type === "number") {
            const numberValue = Number(value);

            if (!Number.isFinite(numberValue)) {
                throw new Error(`${parameter.label} must be a valid number.`);
            }

            parameters[parameter.name] = numberValue;
        } else {
            parameters[parameter.name] = value;
        }
    }

    return parameters;
}

function OperationForm({
    operation,
    isSubmitting,
    onExecute,
}: OperationFormProps) {
    const [values, setValues] = useState<FormValues>({});
    const [validationError, setValidationError] = useState<string | null>(
        null
    );

    function handleInputChange(name: string, value: string) {
        setValues((previous) => ({
            ...previous,
            [name]: value,
        }));

        setValidationError(null);
    }

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (isSubmitting) return;

        setValidationError(null);

        try {
            const parameters = buildParameters(operation, values);
            await onExecute(parameters);
        } catch (error) {
            setValidationError(
                error instanceof Error
                    ? error.message
                    : "Invalid input."
            );
        }
    }

    return (
        <form onSubmit= { handleSubmit } >
        <h2>{ operation.name } </h2>

    {
        operation.parameters.map((parameter) => (
            <div className= "form-field" key = { parameter.name } >
            <label htmlFor={`parameter-${parameter.name}`}>
            { parameter.label }
                </label>

                < input
    id = {`parameter-${parameter.name}`
}
type = { parameter.type === "number" ? "number" : "text" }
step = { parameter.type === "number" ? "any" : undefined }
required = { parameter.required }
disabled = { isSubmitting }
value = { values[parameter.name] ?? "" }
onChange = {(event) =>
handleInputChange(parameter.name, event.target.value)
            }
          />
    </div>
      ))}

{
    validationError && (
        <p className="error-message" role = "alert" >
        { validationError }
            </p>
      )
}

<button type="submit" disabled = { isSubmitting } >
{ isSubmitting? "Calculating...": "Calculate" }
    </button>
    </form>
  );
}

export default OperationForm;
