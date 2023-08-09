using System.Runtime.InteropServices;

namespace Client;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Chat
{
    private string _message { get; set; }
    private int _roomId { get; set; }

    public Chat(string message, int roomId)
    {
        this._message = message;
        this._roomId = roomId;
    }

    public string GetMessage()
    {
        return this._message;
    }

    public int GetRoomID()
    {
        return this._roomId;
    }
    
    public Chat()
    {
    }
}