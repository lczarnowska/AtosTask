using AtosTask.Infrastructure.Swagger;
using AtosTask.Model;
using AtosTask.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AtosTask
{

    internal static class StartupHelperExtensions
    {             
        public static WebApplication ConfigureServices(
        this WebApplicationBuilder builder)
        {
            builder.Services
            .AddControllers(configure =>
             {
                 configure.ReturnHttpNotAcceptable = true;
             })
            .AddXmlDataContractSerializerFormatters()
            .AddJsonOptions(options =>
            {

                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // create a validation problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();

                    var validationProblemDetails = problemDetailsFactory
                        .CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                    // add additional info not added by default
                    validationProblemDetails.Detail =
                        "See the errors field for details.";
                    validationProblemDetails.Instance =
                        context.HttpContext.Request.Path;

                    // report invalid model state responses as validation issues
                    validationProblemDetails.Type =
                        "https://atos.com/modelvalidationproblem";
                    validationProblemDetails.Status =
                        StatusCodes.Status422UnprocessableEntity;
                    validationProblemDetails.Title =
                        "One or more validation errors occurred.";

                    return new UnprocessableEntityObjectResult(
                        validationProblemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EnumSchemaFilter>();
            });
            builder.Services.AddSingleton<ICustomerRepository, MockCustomerRepository>();

            var app = builder.Build();
            return app;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(
                            "An unexpected fault happened. Try again later.");
                    });
                });
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            return app;
        }        
    }
}
