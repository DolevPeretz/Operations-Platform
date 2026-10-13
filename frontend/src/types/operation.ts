
export interface OperationParameter {
    name: string;
    label: string;
    type: "number" | "text";
    required: boolean;
}

export interface Operation {
    id: string;
    name: string;
    parameters: OperationParameter[];
}

export interface OperationExecution {
    id: string;
    operationId: string;
    inputJson: string;
    resultJson: string | null;
    status: string;
    executedAtUtc: string;
}

export interface OperationHistory {
    lastThree: OperationExecution[];
    monthlyCount: number;
}

export interface ExecuteOperationResponse {
    data: unknown;
    history: OperationHistory;
}
