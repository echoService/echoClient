using System.Net.Sockets;

namespace Client;

public interface IMessageHandler
{
    void HandleRequest(string message, Socket client);

    void HandleResponse(MemoryStream stream);
}