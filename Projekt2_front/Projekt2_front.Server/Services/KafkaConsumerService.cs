using Confluent.Kafka;
using MinioMongoService.IService;
using MinioMongoService.Service;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class KafkaConsumerService
{
    private const string KAFKA_SERVER = "localhost:9092";
    private static readonly string[] KAFKA_TOPICS = { "gene_expression_ready", "upload_complete" };
    private const string GROUP_ID = "upload_group";
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    public KafkaConsumerService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = KAFKA_SERVER,
            GroupId = GROUP_ID,
           AutoOffsetReset = AutoOffsetReset.Earliest
           // EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        Console.WriteLine($"Consumer initialized - BootstrapServers: {config.BootstrapServers}, GroupId: {config.GroupId}");

        try
        {
            var adminConfig = new AdminClientConfig { BootstrapServers = KAFKA_SERVER };
            using var adminClient = new AdminClientBuilder(adminConfig).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            Console.WriteLine($"Connected to brokers: {string.Join(", ", metadata.Brokers.Select(b => $"{b.Host}:{b.Port}"))}");
            Console.WriteLine($"Available topics: {string.Join(", ", metadata.Topics.Select(t => t.Topic))}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to Kafka: {ex.Message}");
        }
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;

    }


    public async Task StartListening(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(KAFKA_TOPICS);
        
        _consumer.Consume(TimeSpan.FromSeconds(1));
        await Task.Delay(1000);
        Console.WriteLine("Listening for messages...");
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult != null)
                {
                    Console.WriteLine("heard the messages...");
                    Console.WriteLine($"{consumeResult.Topic}");
                    if (consumeResult.Topic == "gene_expression_ready")
                    {
                       
                        await TriggerMinIOUpload();
                    }
                    else if (consumeResult.Topic == "upload_complete")
                    {
                        await ProcessUploadCompleteMessage(consumeResult.Value);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Kafka Consumer Stopped.");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task TriggerMinIOUpload()
    {
        using var scope = _scopeFactory.CreateScope();
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.PostAsync("http://localhost:5116/api/upload/start", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("MinIO Upload Triggered.");
        }
        else
        {
            Console.WriteLine(" Error triggering MinIO Upload.");
        }
    }
    private async Task ProcessUploadCompleteMessage(string kafkaMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var mongoService = scope.ServiceProvider.GetRequiredService<IMongoService>();
        var message = JsonSerializer.Deserialize<KafkaUploadMessage>(kafkaMessage);
        if (message?.UploadedFiles != null)
        {
            foreach (var fullName in message.UploadedFiles)
            {
                await mongoService.ParseAndInsertTSVFromMinIOAsync(fullName);
            }
        }
    }
}


public class KafkaUploadMessage
{
    [JsonPropertyName("uploaded_files")]
    public List<string> UploadedFiles { get; set; }
}
