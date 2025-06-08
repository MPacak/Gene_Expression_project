using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.Config
{
   public class GeneStoreConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string GeneCollectionName { get; set; }
    }
}
