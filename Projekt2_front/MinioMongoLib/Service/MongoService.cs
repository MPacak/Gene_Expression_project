using MinioMongoAPI.Repository;
using MinioMongoLib.Helper;
using MinioMongoService.Exceptions;
using MinioMongoService.IService;
using MinioMongoService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.Service
{
    public class MongoService : IMongoService
    {
        private readonly GeneExpressionRepository _geneRepository;
        private readonly IMinioService _minioService;

        public MongoService(GeneExpressionRepository geneRepository, IMinioService minioService)
        {
            _geneRepository = geneRepository;
            _minioService = minioService;
        }

        public async Task ParseAndInsertTSVFromMinIOAsync(string objectName)
        {
            MemoryStream memoryStream = await _minioService.GetObjectAsync(objectName);
            if (memoryStream == null)
            {
                throw new MemoryStreamException($"Failed to load {objectName} from MinIO.");
            }

            List<PatientGeneExpression> patientData = ParseFromMemory.ParseTSVFromMemory(memoryStream, objectName);

            if (patientData.Count > 0)
            {
                await _geneRepository.InsertManyAsync(patientData);
            }
            else
            {
                throw new InvalidOperationException($"No valid patient records found in {objectName}.");
            }
        }
    
        public async Task<List<PatientGeneExpression>> GetAllPatientsAsync()
        {
            return await _geneRepository.GetAllAsync();
        }

        public async Task<PatientGeneExpression> GetPatientByIdAsync(string patientId)
        {
            return await _geneRepository.GetByIdAsync(patientId);
        }

        public async Task<List<PatientGeneExpression>> GetPatientsByCohortAsync(string cohortName)
        {
            return await _geneRepository.GetPatientsByCohortAsync(cohortName);
        }

        public async Task<Dictionary<string, double>> GetGeneExpressionAcrossPatientsAsync(string geneName)
        {
            var patients = await _geneRepository.GetAllAsync();
            Dictionary<string, double> geneData = new();
            foreach (var patient in patients)
            {
                if (patient.GeneExpressions.ContainsKey(geneName))
                {
                    geneData[patient.PatientId] = patient.GeneExpressions[geneName];
                }
            }

            return geneData;
        }

        public async Task<Dictionary<string, Dictionary<string, double>>> GetMultipleGeneExpressionsAsync(List<string> geneNames)
        {
            var patients = await _geneRepository.GetAllAsync();
            var result = new Dictionary<string, Dictionary<string, double>>();

            foreach (var patient in patients)
            {
                var geneValues = patient.GeneExpressions
                    .Where(g => geneNames.Contains(g.Key))
                    .ToDictionary(k => k.Key, v => v.Value);

                if (geneValues.Count > 0)
                {
                    result[patient.PatientId] = geneValues;
                }
            }

            return result;
        }

        public async Task MergeClinicalDataFromMinIOAsync(string objectName)
        {
            MemoryStream memoryStream = await _minioService.GetObjectAsync(objectName);

            if (memoryStream == null)
            {
                throw new Exception($"Failed to load {objectName} from MinIO.");
            }

            using StreamReader reader = new(memoryStream);
            string headerLine = reader.ReadLine();
            if (headerLine == null) throw new Exception("Clinical data TSV is empty.");

            var headers = headerLine.Split('\t');
            int barcodeIndex = Array.IndexOf(headers, "bcr_patient_barcode");
            int dssIndex = Array.IndexOf(headers, "DSS");
            int osIndex = Array.IndexOf(headers, "OS");
            int stageIndex = Array.IndexOf(headers, "ajcc_pathologic_tumor_stage");

            if (barcodeIndex == -1 || dssIndex == -1 || osIndex == -1 || stageIndex == -1)
            {
                throw new Exception("Missing required columns in clinical data.");
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var columns = line?.Split('\t');

                if (columns == null || columns.Length < 4) continue;

                string patientId = columns[barcodeIndex];
                if (string.IsNullOrEmpty(patientId)) continue;

                int? dss = !string.IsNullOrWhiteSpace(columns[dssIndex]) && columns[dssIndex].ToLower() != "n/a"
          ? int.Parse(columns[dssIndex])
          : null;

                int? os = !string.IsNullOrWhiteSpace(columns[osIndex]) && columns[osIndex].ToLower() != "n/a"
                    ? int.Parse(columns[Array.IndexOf(headers, "OS")])
                    : null;
                string clinicalStage = columns[stageIndex];
                Console.WriteLine($"Updating patient {patientId} with DSS: {dss}, OS: {os}, Stage: {clinicalStage}");
                await _geneRepository.UpdateOneAsync(patientId, dss, os, clinicalStage);
            }

           
        }
    }
}

