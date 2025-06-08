using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.Models
{
    public class PatientGeneExpression
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("cancer_cohort")]
        public string CancerCohort { get; set; }

        [BsonElement("gene_expressions")]
        public Dictionary<string, double> GeneExpressions { get; set; }
        [BsonElement("diseaseSpecificSurvival")]
        public int? DSS { get; set; }  

        [BsonElement("overallSurvival")]
        public int? OS { get; set; }  

        [BsonElement("clinicalStage")]
        public string ClinicalStage { get; set; }
    }
}
