using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CorsProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapPost("/proxy/post", async ([FromBody] JsonElement body, [FromQuery] string url) =>
            {
                using var client = new HttpClient();
                using var response = await client.PostAsJsonAsync(url, body);

                string jsonString = await response.Content.ReadAsStringAsync();

                var doc = JsonDocument.Parse(jsonString);

                return doc;
            });

            app.UseHttpsRedirection();

            app.UseCors(pol =>
            {
                pol.AllowAnyOrigin();
                pol.AllowAnyMethod();
                pol.AllowAnyHeader();
            });

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}