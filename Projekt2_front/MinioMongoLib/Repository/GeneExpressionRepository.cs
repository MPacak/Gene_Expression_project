using Microsoft.Extensions.Options;
using MinioMongoService.Config;
using MinioMongoService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MinioMongoAPI.Repository
{
    public class GeneExpressionRepository
    {
        private readonly IMongoCollection<PatientGeneExpression> _geneCollection;

        public GeneExpressionRepository(IOptions<GeneStoreConfig> geneStoreConfig)
        {
            var mongoClient = new MongoClient(geneStoreConfig.Value.ConnectionString);
            var db = mongoClient.GetDatabase(geneStoreConfig.Value.DatabaseName);
            _geneCollection = db.GetCollection<PatientGeneExpression>(geneStoreConfig.Value.GeneCollectionName);
        }
        public async Task<List<PatientGeneExpression>> GetAllAsync()
        {
            return await _geneCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<PatientGeneExpression> GetByIdAsync(string patientId)
        {
            var filter = Builders<PatientGeneExpression>.Filter.Eq(p => p.PatientId, patientId);
            return await _geneCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<List<PatientGeneExpression>> GetPatientsByCohortAsync(string cohortName)
        {
            var filter = Builders<PatientGeneExpression>.Filter.Eq(p => p.CancerCohort, cohortName);
            return await _geneCollection.Find(filter).ToListAsync();
        }


        public async Task CreateOneAsync(PatientGeneExpression patient)
        {
            await _geneCollection.InsertOneAsync(patient);
        }

        public async Task InsertManyAsync(List<PatientGeneExpression> patients)
        {
            if (patients != null && patients.Count > 0)
            {
                await _geneCollection.InsertManyAsync(patients);
            }
        }
        public async Task UpdateOneAsync(string patientId, int? dss, int? os, string clinicalStage)
        {
            //var filter = Builders<PatientGeneExpression>.Filter.Eq(p => p.PatientId, patientId);
            var filter = Builders<PatientGeneExpression>.Filter.Regex(p => p.PatientId, new BsonRegularExpression($"^{patientId}"));
            var updateDefinition = new List<UpdateDefinition<PatientGeneExpression>>();

            if (dss.HasValue) updateDefinition.Add(Builders<PatientGeneExpression>.Update.Set(p => p.DSS, dss));
            if (os.HasValue) updateDefinition.Add(Builders<PatientGeneExpression>.Update.Set(p => p.OS, os));
            updateDefinition.Add(Builders<PatientGeneExpression>.Update.Set(p => p.ClinicalStage, clinicalStage));

            var update = Builders<PatientGeneExpression>.Update.Combine(updateDefinition);
            var result = await _geneCollection.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0)
            {
                Console.WriteLine($"⚠ No match found for Patient ID {patientId}. Data not updated.");
            }
          
        }
    }
}

