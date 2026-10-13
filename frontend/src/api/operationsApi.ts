
import type {
    Operation,
    ExecuteOperationResponse,
} from "../types/operation";

const API_BASE_URL = "https://localhost:7170/api/operations";

export async function getOperations(): Promise<Operation[]> {
    const response = await fetch(API_BASE_URL);

    if (!response.ok) {
        throw new Error("Failed to load operations");
    }

    return response.json();
}

export async function executeOperation(
    operationId: string,
    parameters: Record<string, string | number>
): Promise<ExecuteOperationResponse> {
    const response = await fetch(`${API_BASE_URL}/execute`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            operationId,
            parameters,
        }),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);

        throw new Error(
            error?.detail ??
            error?.title ??
            "Failed to execute operation"
        );
    }

    return response.json();
}
