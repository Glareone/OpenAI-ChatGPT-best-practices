namespace DealerAssistant.DAL;

public class Entities
{
    public record ChatMessage(string role, string content);
}