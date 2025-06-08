using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoService.Exceptions
{
    public class MemoryStreamException : Exception
    {
        public MemoryStreamException(string message) : base(message)
        {
        }

    }
}
