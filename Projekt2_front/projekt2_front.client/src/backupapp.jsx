import { useEffect, useState } from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
function App() {
    const [patientId, setPatientId] = useState("");
    const [patientData, setPatientData] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {

    }, []);

    const handleSearch = async () => {
        setError("");
        setPatientData(null);

        try {

            const response = await fetch(`api/visualization/patient/${patientId}`);
            const data = await response.json();
            console.log(data)
            setPatientData(data);
        } catch (error) {
            console.error("Error:", error);
            setError("❌ Patient not found or error fetching data.");
            console.error("Error:", error);
        }
    };

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Gene Expression Data</h2>

            {/* Input Field + Search Button */}
            <div className="input-group mb-3">
                <input
                    type="text"
                    className="form-control"
                    placeholder="Enter Patient ID"
                    value={patientId}
                    onChange={(e) => setPatientId(e.target.value)}
                />
                <button className="btn btn-primary" onClick={handleSearch}>
                    Search
                </button>
            </div>

            {/* Error Message */}
            {error && <p className="text-danger">{error}</p>}

            {/* Display Patient Data */}
            {patientData && (
                <div className="card mt-4">
                    <div className="card-body">
                        <h5 className="card-title">Patient ID: {patientData.patientId}</h5>
                        <h6 className="card-subtitle mb-2 text-muted">Cohort: {patientData.cancerCohort}</h6>

                        <h6>Gene Expression Levels:</h6>
                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>Gene</th>
                                    <th>Expression Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                {Object.entries(patientData.geneExpressions).map(([gene, value]) => (
                                    <tr key={gene}>
                                        <td>{gene}</td>
                                        <td>{value.toFixed(3)}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}
        </div>
    );

}

export default App;