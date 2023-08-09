using System.Net.Sockets;

namespace Client;

public class InquiryMessageHandler : IMessageHandler
{
    private readonly InquiryService _inquiryService;
    private readonly HeaderService _headerService;

    public InquiryMessageHandler(InquiryService inquiryService, HeaderService headerService)
    {
        _inquiryService = inquiryService;
        _headerService = headerService;
    }

    public void HandleRequest(string message, Socket client)
    {
        MemoryStream stream = new MemoryStream();
        var header = new Header(Command.Inquiry, 0);
        _headerService.SerializeTo(header, stream);
        client.Send(stream.ToArray());
    }

    public void HandleResponse(MemoryStream stream)
    {
        var roomList = _inquiryService.DeserializeFrom(stream);
        foreach (var room in roomList)
        {
            Console.WriteLine("{0}번 : {1}", room.Key, room.Value);
        }
    }
}