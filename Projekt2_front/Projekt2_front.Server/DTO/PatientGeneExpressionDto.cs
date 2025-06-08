namespace MinioMongoAPI.DTO
{
    public class PatientGeneExpressionDto
    {
        public string PatientId { get; set; }
        public string CancerCohort { get; set; }
        public Dictionary<string, double> GeneExpressions { get; set; }
    }
}
