using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Client;

public class Dispatcher
{
    private readonly IMessageHandler _chatMessageHandler;
    private readonly IMessageHandler _createMessageHandler;
    private readonly IMessageHandler _joinMessageHandler;
    private readonly IMessageHandler _inquiryMessageHandler;

    private readonly Dictionary<Command, IMessageHandler> _commands = new();

    public Dispatcher(IMessageHandler chatMessageHandler, IMessageHandler createMessageHandler, IMessageHandler joinMessageHandler, IMessageHandler inquiryMessageHandler)
    {
        _chatMessageHandler = chatMessageHandler;
        _createMessageHandler = createMessageHandler;
        _joinMessageHandler = joinMessageHandler;
        _inquiryMessageHandler = inquiryMessageHandler;
    }

    public void Add(Command command, IMessageHandler messageHandler)
    {
        _commands.Add(command, messageHandler);
    }
    
    public void DispatchResponse(Command command, MemoryStream stream)
    {
        switch (command)
        {
            case Command.Create:
                _createMessageHandler.HandleResponse(stream);
                break;
            case Command.Join:
                _joinMessageHandler.HandleResponse(stream);
                break;
            case Command.Chat:
                _chatMessageHandler.HandleResponse(stream);
                break;
            case Command.Inquiry:
                _inquiryMessageHandler.HandleResponse(stream);
                break;
        }
    }
    
    public void DispatchRequest(Socket client)
        {
            var message = Console.ReadLine();
            var index = message.IndexOf(' ');
            if (index < 0)
            {
                index = 0;
            }
            
            var cmd = message.Substring(0, index);
            
            if (index == 0)
            {
                cmd = message;
            }
            
            var cmdPart = Regex.Replace(cmd, @"(^\w)", match => match.Value.ToUpper());
            if (false == Enum.TryParse<Command>(cmdPart, out var command))
            {
                command = cmdPart == "Inquiry" ? Command.Inquiry : Command.Chat;
            }

            string otherParts = null;
            
            otherParts = command is Command.Chat or Command.Inquiry ? null : message.Substring(index + 1, message.Length - (index + 1));

            switch (command)
            {
                case Command.Create:
                    _createMessageHandler.HandleRequest(otherParts, client);
                    break;
                
                case Command.Join:
                    _joinMessageHandler.HandleRequest(otherParts, client);
                    break;
                
                case Command.Inquiry:
                    _inquiryMessageHandler.HandleRequest(otherParts, client);
                    break;

                default:
                    _chatMessageHandler.HandleRequest(message, client);
                    break;
            }
        }
}