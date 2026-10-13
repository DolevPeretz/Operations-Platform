
import { useOperations } from "./hooks/useOperations";

import OperationSelector from "./components/OperationSelector";
import OperationForm from "./components/OperationForm";
import OperationResult from "./components/OperationResult";
import OperationHistory from "./components/OperationHistory";

import "./App.css";

function App() {
    const {
        operations,
        selectedOperationId,
        selectedOperation,
        execution,
        loading,
        isSubmitting,
        error,
        handleExecute,
        handleOperationChange,
    } = useOperations();

    return (
        <main className= "app-container" >
        <header>
        <h1>Operations Platform </h1>
            < p > Execute dynamic operations </p>
                </header>

    { loading && <p>Loading operations...</p> }

    { error && <p className="error-message" > { error } </p> }

    {
        !loading && !error && (
            <>
            {
                operations.length === 0 ? (
                    <p>No operations available.</p>
          ) : (
                        <>
                        <section className="card" >
                    <OperationSelector
                  operations={ operations }
                  selectedOperationId = { selectedOperationId }
                  disabled = { isSubmitting }
                  onChange = { handleOperationChange }
                        />

                        { selectedOperation && (
                            <OperationForm
                    key={ selectedOperation.id }
                    operation = { selectedOperation }
                    isSubmitting = { isSubmitting }
                    onExecute = { handleExecute }
                        />
                )}
    </section>

    {
        execution && (
            <>
            <OperationResult data={ execution.data } />

                < OperationHistory history = { execution.history } />
                    </>
              )
    }
    </>
          )
}
</>
      )}
</main>
  );
}

export default App;
