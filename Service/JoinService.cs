namespace Client;

public class JoinService
{
    private readonly HeaderService _headerService;

    public JoinService(HeaderService headerService)
    {
        _headerService = headerService;
    }

    public void SerializeTo(JoinRoom.JoinRoomReq request, MemoryStream stream)
    {
        var roomNumBytes = BitConverter.GetBytes(request.GetRoomNum());
        Header header = new Header(Command.Join, sizeof(int));
        _headerService.SerializeTo(header, stream);
        stream.Write(roomNumBytes, 0, 4);
    }

    public JoinRoom.JoinRoomAns DeserializeFrom(MemoryStream stream)
    {
        var roomNumBytes = new byte[4];
        stream.Read(roomNumBytes, 0, sizeof(int));
        var roomNum = BitConverter.ToInt32(roomNumBytes);
        JoinRoom.JoinRoomAns joinRoomAns = new JoinRoom.JoinRoomAns(roomNum);
        return joinRoomAns;
    }
}