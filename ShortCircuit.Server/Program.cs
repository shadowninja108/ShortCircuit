
using FirstStrike;
using ShortCircuit.Server.Controllers;

namespace ShortCircuit.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddOpenApi();

            var app = builder.Build();
            
            app.UseDefaultFiles();
            app.UseStaticFiles();

            /* HTTP? in 2024? Nah. */
            app.UseHttpsRedirection();

            /* Host Swagger UI so that people can easily read the API docs. Explicitly want to do this in prod. */
            app.UseSwaggerUI();

            /* Host OpenApi doc where Swashbuckle's SwaggerUI expects. */
            // app
            //     .MapOpenApi("swagger/v1/swagger.json")
            //     .CacheOutput();

            /* Map our save API. */
            app.MapSaveApi();

            app.MapFallbackToFile("/index.html");

            /* Load dictionary for FirstStrike. */
            HashDb.Load(new FileInfo("main.strings"));

            app.Run();
        }
    }
}
