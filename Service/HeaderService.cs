namespace Client;

public class HeaderService
{
    public HeaderService()
    {
        
    }
    
    public void SerializeTo(Header header, MemoryStream stream)
    {
        stream.Write(BitConverter.GetBytes((int)header.GetCommand()));
        stream.Write(BitConverter.GetBytes(header.GetLength()));
    }

    public Header DeserializeFrom(MemoryStream stream)
    {
        var commandBytes = new byte[4];
        var lengthBytes = new byte[4];

        stream.Read(commandBytes, 0, 4);
        stream.Read(lengthBytes, 0, 4);

        Command command = (Command)BitConverter.ToInt32(commandBytes);
        int length = BitConverter.ToInt32(lengthBytes);

        Header header = new Header(command, length);

        return header;
    }
}