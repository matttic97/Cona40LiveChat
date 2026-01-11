namespace Cona40LiveChat.Hubs;

public record ChatMessage(string User, string Text, DateTime Time);

public class ChatState
{
    public const int MaxQueueSize = 20;
    private Queue<ChatMessage> ChatMessages { get; set; } = new();
    
    public List<ChatMessage> GetLastMassages()
    {
        return ChatMessages.ToList();
    }

    public void AddMessage(ChatMessage message)
    {
        ChatMessages.Enqueue(message);

        if (ChatMessages.Count > MaxQueueSize)
            ChatMessages.Dequeue();
    }
}