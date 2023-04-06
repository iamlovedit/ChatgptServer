using ChatGptService.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatGptService.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _serializerSettings;
        public ChatHub(IHttpClientFactory httpClientFactory, JsonSerializerSettings serializerSettings)
        {
            _httpClientFactory = httpClientFactory;
            _serializerSettings = serializerSettings;
        }

        public async IAsyncEnumerable<string> GenerateAnswerStream(string question, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("chatgpt");
            var request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);
            var messages = new List<ChatMessage>() { new ChatMessage(question) };
            var body = new
            {
                model = "gpt-3.5-turbo",
                stream = true,
                messages
            };
            var bodyJson = JsonConvert.SerializeObject(body, _serializerSettings);
            var strContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            request.Content = strContent;

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    var buffer = new byte[1024];
                    var bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    while (bytes > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var responseText = Encoding.UTF8.GetString(buffer, 0, bytes);
                        yield return responseText;
                        bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    }
                }
            }
            else
            {
                yield return $"网络请求错误 {response.StatusCode}";
            }
        }


        public async Task GenerateAnswer(string question)
        {
            var httpClient = _httpClientFactory.CreateClient("chatgpt");
            var request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);
            var messages = new List<ChatMessage>() { new ChatMessage(question) };
            var body = new
            {
                model = "gpt-3.5-turbo",
                stream = true,
                messages
            };
            var bodyJson = JsonConvert.SerializeObject(body, _serializerSettings);
            var strContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            request.Content = strContent;
            try
            {
                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[1024];
                    var bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    while (bytes > 0)
                    {
                        var responseText = Encoding.UTF8.GetString(buffer, 0, bytes);
                        await Clients.Caller.ReceiveAnswer(responseText);
                        bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.ReceiveAnswer(e.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

    }
}
