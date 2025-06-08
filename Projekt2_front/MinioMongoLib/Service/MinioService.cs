
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using MinioMongoService.IService;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.Service
{
    public class MinioService : IMinioService
    {
        private const string ENDPOINT = "localhost:9000";
        private const string ACCESS_KEY = "admin";
        private const string SECRET_KEY = "specialpassword!";
        private const string BUCKET_NAME = "gene-expression-bucket";
        private readonly IMinioClient _minioClient;



        public MinioService()
        {
            _minioClient = new MinioClient()
                                .WithEndpoint(ENDPOINT)
                                .WithCredentials(ACCESS_KEY, SECRET_KEY)
                                .Build();

        }
        public async Task EnsureBucketExistsAsync()
        {
            try
            {
                bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BUCKET_NAME));
                if (!found)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BUCKET_NAME));
                }
            }
            catch (MinioException ex)
            {
                Console.WriteLine($" Error checking/creating bucket: {ex.Message}");
            }
        }
        public async Task UploadToMinIO(string filePath, string cohortFullName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(BUCKET_NAME)
                        .WithFileName(filePath)
                        .WithContentType("application/tsv")
                        .WithObject($"{cohortFullName}")
                );

            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
            }
        }
        public async Task<MemoryStream> GetObjectAsync(string objectId)
        {
            MemoryStream memoryStream = new();

            try
            {
                await _minioClient.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(BUCKET_NAME)
                        .WithObject(objectId)
                        .WithCallbackStream(stream =>
                        {
                            stream.CopyTo(memoryStream);
                        })
                );

                memoryStream.Position = 0;
                Console.WriteLine($"Successfully loaded {objectId} into memory.");

                return memoryStream;
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Error getting object: {ex.Message}");
                return (null);
            }
        }

    }
}
