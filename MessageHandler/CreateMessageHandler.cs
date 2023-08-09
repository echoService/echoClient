using System.Net.Sockets;

namespace Client;

public class CreateMessageHandler : IMessageHandler
{
    private readonly CreateService _createService;

    public CreateMessageHandler(CreateService createService)
    {
        _createService = createService;
    }

    public void HandleRequest(string message, Socket client)
    {
        var sendBuffer = new MemoryStream();
        var createRoomReq = new CreateRoom.CreateRoomReq(message);
        _createService.SerializeTo(createRoomReq, sendBuffer);
        client.Send(sendBuffer.ToArray());
    }

    public void HandleResponse(MemoryStream stream)
    {
        var createRoomAns = _createService.DeserializeFrom(stream);
        Console.WriteLine("{0}번 방에 입장하셨습니다.", createRoomAns.GetRoomNum());
        Program.RoomNum = createRoomAns.GetRoomNum();
    }
}