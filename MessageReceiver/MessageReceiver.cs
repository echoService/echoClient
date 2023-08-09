using System.Net.Sockets;

namespace Client;

public class MessageReceiver
{
    private readonly Socket _socket;
    private readonly Dispatcher _dispatcher;
    private readonly HeaderService _headerService;

    public MessageReceiver(Socket socket, Dispatcher dispatcher, HeaderService headerService)
    {
        _socket = socket;
        _dispatcher = dispatcher;
        _headerService = headerService;
    }
    public async Task ReceiveMessagesAsync()
    {
        try
        {
            while (_socket.Connected)
            {
                var memoryStream = new MemoryStream();
                var buffer = new byte[1024];
                
                var receivedBytes = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                if (receivedBytes == 0)
                    break;
                
                memoryStream.Position = 0;
                memoryStream.Write(buffer, 0, receivedBytes);

                while (true)
                {
                    memoryStream.Position = 0;
                    
                    if (memoryStream.Length < 8)
                        break;

                    var header = _headerService.DeserializeFrom(memoryStream);
                    
                    if (memoryStream.Length < (header.GetLength() + 8))
                        break;

                    _dispatcher.DispatchResponse(header.GetCommand(), memoryStream);
                    
                    memoryStream.SetLength(0);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}