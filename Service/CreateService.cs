using System.Text;

namespace Client;

public class CreateService
{
    private readonly HeaderService _headerService;

    public CreateService(HeaderService headerService)
    {
        _headerService = headerService;
    }

    public void SerializeTo(CreateRoom.CreateRoomReq request, MemoryStream stream)
    {
        var titleBytes = Encoding.UTF8.GetBytes(request.GetTitle());
        var lengthBytes = BitConverter.GetBytes(titleBytes.Length);

        Header header = new Header(Command.Create, lengthBytes.Length + titleBytes.Length);
        _headerService.SerializeTo(header, stream);
        stream.Write(lengthBytes, 0, 4);
        stream.Write(titleBytes, 0, titleBytes.Length);
    }

    public CreateRoom.CreateRoomAns DeserializeFrom(MemoryStream stream)
    {
        var roomNumBytes = new byte[4];
        stream.Read(roomNumBytes, 0, sizeof(int));
        var roomNum = BitConverter.ToInt32(roomNumBytes);
        Console.WriteLine("==================" + roomNum);
        var createRoomAns = new CreateRoom.CreateRoomAns(roomNum);
        return createRoomAns;
    }
}