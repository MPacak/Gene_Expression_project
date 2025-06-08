using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MinioMongoService.Service
{
    public class KafkaProducerService
    {
        private const string KAFKA_SERVER = "localhost:9092";
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService()
        {
            var config = new ProducerConfig { BootstrapServers = KAFKA_SERVER };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public void SendUploadCompleteMessage(List<string> uploadedFiles)
        {
            var message = new
            {
                uploaded_files = uploadedFiles
            };

            string jsonMessage = JsonSerializer.Serialize(message);

            _producer.Produce("upload_complete", new Message<string, string>
            {
                Key = "upload_status",
                Value = jsonMessage
            });

            _producer.Flush();
            Console.WriteLine($"Kafka Message Sent (upload_complete): {jsonMessage}");
        }
    }
}
