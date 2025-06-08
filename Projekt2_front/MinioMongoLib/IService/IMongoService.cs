using MinioMongoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.IService
{
    public interface IMongoService
    {
        Task ParseAndInsertTSVFromMinIOAsync(string objectName);
        Task<List<PatientGeneExpression>> GetAllPatientsAsync();
        Task<PatientGeneExpression> GetPatientByIdAsync(string patientId);
        Task<List<PatientGeneExpression>> GetPatientsByCohortAsync(string cohortName);
        Task<Dictionary<string, double>> GetGeneExpressionAcrossPatientsAsync(string geneName);
        Task<Dictionary<string, Dictionary<string, double>>> GetMultipleGeneExpressionsAsync(List<string> geneNames);

        Task MergeClinicalDataFromMinIOAsync(string objectName);
    }
}

