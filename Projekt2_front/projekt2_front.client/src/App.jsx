import { useEffect, useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from "recharts";
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
            setError("Patient not found or error fetching data.");
            console.error("Error:", error);
        }
    };

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Gene Expression Data Visualization</h2>

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

            {error && <p className="text-danger">{error}</p>}

            {patientData && (
                <div className="card mt-4">
                    <div className="card-body">
                        <h5 className="card-title">Patient ID: {patientData.patientId}</h5>
                        <h6 className="card-subtitle mb-2 text-muted">Cohort: {patientData.cancerCohort}</h6>

                        <h6>Clinical Data:</h6>
                        <p><strong>DSS:</strong> {patientData.dss !== null && patientData.dss !== undefined ? (patientData.dss === 1 ? "Survived" : "Not Survived") : "N/A"}</p>
                        <p><strong>OS:</strong> {patientData.os !== null && patientData.os !== undefined ? (patientData.os === 1 ? "Survived" : "Not Survived") : "N/A"}</p>
                        <p><strong>Clinical Stage:</strong> {patientData.clinicalStage || "N/A"}</p>

                        <h6>Gene Expression Levels:</h6>

                        <ResponsiveContainer width="100%" height={300}>
                            <BarChart data={Object.entries(patientData.geneExpressions).map(([gene, value]) => ({ gene, value }))} margin={{ top: 20, right: 20, bottom: 20, left: 60 }}>
                                <XAxis dataKey="gene" angle={-45} textAnchor="end" height={80} />
                                <YAxis />
                                <Tooltip />
                                <CartesianGrid strokeDasharray="3 3" />
                                <Bar dataKey="value" fill="#007bff" />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            )}
        </div>
    );
}

export default App;