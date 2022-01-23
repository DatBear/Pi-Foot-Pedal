using System.Device.Gpio;
using Microsoft.Extensions.FileProviders;
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

var reactBuildPath = Path.Combine(builder.Environment.ContentRootPath, "ClientApp", "build");
if (Directory.Exists(reactBuildPath))
{
    //todo this whole setup feels pretty hacky :(
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/app",
        FileProvider = new PhysicalFileProvider(reactBuildPath)
    });

    app.Map("/home", x =>
    {
        x.Run(z =>
        {
            z.Response.Redirect("/app/index.html");
            return Task.FromResult(0);
        });
    });
}


app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
});

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