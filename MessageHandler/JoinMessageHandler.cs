using System.Net.Sockets;

namespace Client;

public class JoinMessageHandler : IMessageHandler
{
    private readonly JoinService _joinService;

    public JoinMessageHandler(JoinService joinService)
    {
        _joinService = joinService;
    }

    public void HandleRequest(string title, Socket client)
    {
        MemoryStream sendBuffer = new MemoryStream();
        var roomNum = Int32.Parse(title);
        var joinRoomReq = new JoinRoom.JoinRoomReq(roomNum);
        _joinService.SerializeTo(joinRoomReq, sendBuffer);
        client.Send(sendBuffer.ToArray());
    }

    public void HandleResponse(MemoryStream stream)
    {
        var joinRoomAns = _joinService.DeserializeFrom(stream);
        Console.WriteLine("{0}번 방에 입장하셨습니다.", joinRoomAns.GetRoomNum());
        Program.RoomNum = joinRoomAns.GetRoomNum();
    }
}