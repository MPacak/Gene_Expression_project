using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MinioMongoService.IService;
using MinioMongoService.Service;

[ApiController]
[Route("api/upload")]
public class UploadMinioController : ControllerBase
{
    private readonly IMinioService _minioService;
    private readonly KafkaProducerService _kafkaProducerService;
    private readonly IMongoService _mongoService;

    public UploadMinioController(IMinioService minioService, KafkaProducerService kafkaProducerService, IMongoService mongoService)
    {
        _minioService = minioService;
        _kafkaProducerService = kafkaProducerService;
        _mongoService = mongoService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartUpload()
    {
        string folderPath = @"D:\Faks\3. godina\PPPK\Python\extracted_tsv";
        var tsvFiles = Directory.GetFiles(folderPath, "*.tsv");

        List<string> uploadedFiles = new();
        foreach (var filePath in tsvFiles)
        {
            string fileName = Path.GetFileName(filePath);

            string shortName = ExtractShortName(fileName);
            if (shortName == null) continue;

            string fullName = CohortNaming.GetFullCohortName(shortName);

            await _minioService.UploadToMinIO(filePath, fullName);
            uploadedFiles.Add(fullName);
        }

        _kafkaProducerService.SendUploadCompleteMessage(uploadedFiles);
        return Ok(new { message = "Upload started", files = uploadedFiles });
    }

    private string ExtractShortName(string fileName)
    {
        foreach (var shortName in CohortNaming.GetAllShortNames())
        {
            if (fileName.Contains($".{shortName}."))
            {
                return shortName;
            }
        }
        return null;
    }
  
}
