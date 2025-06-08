using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinioMongoService.IService;
using MinioMongoService.Models;

[ApiController]
[Route("api/visualization")]
public class VisualizationController : ControllerBase
{
    private readonly IMongoService _mongoService;
    private readonly IMinioService _minioService;

    public VisualizationController(IMongoService mongoService, IMinioService minioService)
    {
        _mongoService = mongoService;
        _minioService = minioService;
    }

    [HttpGet("cohort/{cohortName}")]
    public async Task<ActionResult<List<PatientGeneExpression>>> GetPatientsByCohort(string cohortName)
    {
        
        var patients = await _mongoService.GetPatientsByCohortAsync(cohortName);
        return Ok(patients);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<PatientGeneExpression>> GetPatientById(string patientId)
    {
        var patient = await _mongoService.GetPatientByIdAsync(patientId);
        if (patient == null) return NotFound(new { message = "Patient not found.", patientId });

          return Ok(patient);
       
    }

    [HttpGet("gene/{geneName}")]
    public async Task<ActionResult<Dictionary<string, double>>> GetGeneExpressionAcrossPatients(string geneName)
    {
        var geneData = await _mongoService.GetGeneExpressionAcrossPatientsAsync(geneName);
        return Ok(geneData);
    }


    [HttpPost("genes")]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, double>>>> GetMultipleGeneExpressions([FromBody] List<string> geneNames)
    {
        var geneData = await _mongoService.GetMultipleGeneExpressionsAsync(geneNames);
        return Ok(geneData);
    }
    [HttpPost("combine_data")]
    public async Task<ActionResult> CombineData()
    {
        string folderPath = @"D:\Faks\3. godina\PPPK\Python\extracted_tsv\TCGA_clinical_survival_data.tsv";
        var objectName = "clinical data";
        await _minioService.UploadToMinIO(folderPath, objectName);

        await _mongoService.MergeClinicalDataFromMinIOAsync(objectName);
        return Ok();
    }
   
}