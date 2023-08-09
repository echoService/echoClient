using System.Text;

namespace Client;

// 데이터를 가공하는 로직은 이쪽으로 와야댐
public class ChatService
{
    private readonly HeaderService _headerService;
    
    public ChatService(HeaderService headerService)
    {
        _headerService = headerService;
    }
    
    public void SerializeTo(Chat chat, MemoryStream stream)
    {
        var messageBytes = Encoding.UTF8.GetBytes(chat.GetMessage());
        var messageLengthBytes = BitConverter.GetBytes(messageBytes.Length);
        var roomNumBytes = BitConverter.GetBytes(chat.GetRoomID());

        Header header = new Header(Command.Chat, (messageBytes.Length + sizeof(int) + sizeof(int)));
        
        _headerService.SerializeTo(header, stream);
        stream.Write(messageLengthBytes, 0, sizeof(int));
        stream.Write(messageBytes, 0, BitConverter.ToInt32(messageLengthBytes));
        stream.Write(roomNumBytes, 0, sizeof(int));
    }

    public Chat DeserializeFrom(MemoryStream stream)
    {
        var messageLengthBytes = new byte[4];
        var roomNum = new byte[4];
        
        stream.Read(messageLengthBytes, 0, sizeof(int));
        var messageLength = BitConverter.ToInt32(messageLengthBytes);
        var messageBytes = new byte[messageLength];
        stream.Read(messageBytes, 0, messageLength);
        var message = Encoding.UTF8.GetString(messageBytes);

        stream.Read(roomNum, 0, sizeof(int));
        var roomId = BitConverter.ToInt32(roomNum);

        Chat chat = new Chat(message, roomId);
        return chat;
    }
}