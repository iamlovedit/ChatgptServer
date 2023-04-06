using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ChatGptService.Models
{
    public class ChatMessage
    {
        public ChatRole Role { get; }

        public string Content { get; }

        public ChatMessage(string content, ChatRole role = ChatRole.User)
        {
            Content = content;
            Role = role;
        }
    }

    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
    public enum ChatRole
    {
        System,
        User,
        Assistant
    }
}
