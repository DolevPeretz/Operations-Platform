
import type { OperationHistory as History } from "../types/operation";

interface OperationHistoryProps {
    history: History;
}

function formatJson(json: string | null): string {
    if (json === null) return "—";

    try {
        const value: unknown = JSON.parse(json);

        return typeof value === "string"
            ? value
            : JSON.stringify(value);
    } catch {
        return json;
    }
}

function formatDate(dateString: string): string {
    const hasTimezone = /(?:Z|[+-]\d{2}:\d{2})$/.test(dateString);
    const utcDateString = hasTimezone ? dateString : `${dateString}Z`;

    return new Date(utcDateString).toLocaleString();
}

function OperationHistory({ history }: OperationHistoryProps) {
    return (
        <section className= "card" >
        <h2>Last 3 Executions </h2>

    {
        history.lastThree.length === 0 ? (
            <p>No execution history yet.</p>
      ) : (
            <div className= "history-list" >
            {
                history.lastThree.map((item) => (
                    <div className= "history-item" key = { item.id } >
                    <div>
                    <strong>{ item.status } </strong>
                    < p > { formatDate(item.executedAtUtc)
            } </p>
            </div>

            < div >
            <strong>Result: </strong>{" "}
        { formatJson(item.resultJson) }
        </div>
            </div>
          ))
    }
    </div>
      )
}

<div className="monthly-count" >
    Executions this month: { " " }
<strong>{ history.monthlyCount } </strong>
    </div>
    </section>
  );
}

export default OperationHistory;
