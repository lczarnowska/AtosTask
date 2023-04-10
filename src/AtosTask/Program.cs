using AtosTask;
using AtosTask.Infrastructure.Swagger;
using AtosTask.Model;
using AtosTask.Repository;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder
       .ConfigureServices()
       .ConfigurePipeline();

// run the app
app.Run();
