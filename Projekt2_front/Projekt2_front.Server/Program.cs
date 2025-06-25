using MinioMongoAPI.Repository;
using MinioMongoService.Config;
using MinioMongoService.IService;
using MinioMongoService.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddScoped<GeneExpressionRepository>();
builder.Services.AddScoped<IMongoService, MongoService>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GeneStoreConfig>(builder.Configuration.GetSection("GeneStoreConfig"));

var app = builder.Build();

var kafkaConsumer = app.Services.GetRequiredService<KafkaConsumerService>();
var cts = new CancellationTokenSource();
//bez await radi u pozadini i slusa a sa await moram cekati dok ne cuje i odradi. 
kafkaConsumer.StartListening(cts.Token);

Console.CancelKeyPress += (sender, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
};

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
