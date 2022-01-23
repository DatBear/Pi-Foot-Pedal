using System.Device.Gpio;
using Newtonsoft.Json;
using PiFootPedal.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Add(ServiceDescriptor.Singleton(x =>
{
    var dir = AppDomain.CurrentDomain.BaseDirectory;
    var configPath = Path.Combine(dir, "Config/PollConfig.json");

    var configService = new ConfigService(configPath);
    var config = configService.GetConfig(true);

    if (config == null)
    {
        configService.Save();
    }
    return configService;
}));

builder.Services.Add(ServiceDescriptor.Singleton(x =>
{
    var configService = x.GetService<ConfigService>() ?? throw new InvalidOperationException();
    configService.Setup();//debug defaults
    configService.Save();
    return new PollService(configService);
}));

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var pollService = app.Services.GetService<PollService>();
Task.Run(async () =>
{
    try
    {
        await pollService.Start();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error starting pollService: {e}");
    }
    
});

app.Run();