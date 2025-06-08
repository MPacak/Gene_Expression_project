using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.IService
{
    public interface IMinioService
    {
        Task EnsureBucketExistsAsync();
        Task UploadToMinIO(string filePath, string cohorFullName);
        Task<MemoryStream> GetObjectAsync(string objectId);
    }
}
