using System.Net.Sockets;

namespace Client;

public class ChatMessageHandler : IMessageHandler
{
    private readonly ChatService _chatService;

    public ChatMessageHandler(ChatService chatService)
    {
        _chatService = chatService;
    }

    public void HandleRequest(string message, Socket client)
    {
        MemoryStream sendBuffer = new MemoryStream();
        Chat chat = new Chat(message, Program.RoomNum);
        _chatService.SerializeTo(chat, sendBuffer);
        client.Send(sendBuffer.ToArray());
    }

    public void HandleResponse(MemoryStream stream)
    {
        Chat chat = _chatService.DeserializeFrom(stream);
        Console.WriteLine("Received: {0}", chat.GetMessage());
    }
}