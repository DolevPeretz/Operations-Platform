
import { useEffect, useState } from "react";
import {
    getOperations,
    executeOperation,
} from "../api/operationsApi";

import type {
    Operation,
    ExecuteOperationResponse,
} from "../types/operation";

export function useOperations() {
    const [operations, setOperations] = useState<Operation[]>([]);
    const [selectedOperationId, setSelectedOperationId] = useState("");
    const [execution, setExecution] =
        useState<ExecuteOperationResponse | null>(null);

    const [loading, setLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let active = true;

        async function loadOperations() {
            try {
                const data = await getOperations();

                if (!active) return;

                setOperations(data);

                if (data.length > 0) {
                    setSelectedOperationId(data[0].id);
                }
            } catch (err) {
                if (active) {
                    setError(
                        err instanceof Error
                            ? err.message
                            : "Failed to load operations."
                    );
                }
            } finally {
                if (active) {
                    setLoading(false);
                }
            }
        }

        loadOperations();

        return () => {
            active = false;
        };
    }, []);

    const selectedOperation = operations.find(
        (operation) => operation.id === selectedOperationId
    );

    async function handleExecute(
        parameters: Record<string, string | number>
    ) {
        if (!selectedOperation || isSubmitting) return;

        setIsSubmitting(true);
        setError(null);
        setExecution(null);

        try {
            const response = await executeOperation(
                selectedOperation.id,
                parameters
            );

            setExecution(response);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "An unexpected error occurred."
            );
        } finally {
            setIsSubmitting(false);
        }
    }

    function handleOperationChange(operationId: string) {
        setSelectedOperationId(operationId);
        setExecution(null);
        setError(null);
    }

    return {
        operations,
        selectedOperationId,
        selectedOperation,
        execution,
        loading,
        isSubmitting,
        error,
        handleExecute,
        handleOperationChange,
    };
}
