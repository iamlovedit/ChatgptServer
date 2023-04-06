namespace ChatGptService.Hubs
{
    public interface IChatClient
    {
        Task ReceiveAnswer(string answer);
    }
}
