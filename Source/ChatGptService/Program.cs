using ChatGptService.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;

namespace ChatGptService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            services.AddControllers();
            services.AddSignalR();

            services.AddHttpClient("chatgpt", config =>
            {
                var section = builder.Configuration.GetSection("ChatGpt");
                config.BaseAddress = new Uri(section["ChatUrl"]);
                config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", section["Key"]);
            });

            services.AddSingleton(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var app = builder.Build();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/chat");

            app.Run();
        }
    }
}