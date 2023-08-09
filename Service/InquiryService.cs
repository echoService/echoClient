using System.Text;
using Microsoft.VisualBasic;

namespace Client;

public class InquiryService
{
    public InquiryService()
    {
        
    }
    public Dictionary<int, string> DeserializeFrom(MemoryStream stream)
    {
        var countBytes = new byte[4];
        stream.Read(countBytes, 0, sizeof(int));
        var count = BitConverter.ToInt32(countBytes);
        var roomInfos = new Dictionary<int, string>();
        for (int i = 0; i < count; i++)
        {
            var titleLengthBytes = new byte[4];
            stream.Read(titleLengthBytes, 0, sizeof(int));
            var length = BitConverter.ToInt32(titleLengthBytes);
            var titleBytes = new byte[length];
            stream.Read(titleBytes, 0, length);
            var title = Encoding.UTF8.GetString(titleBytes);
            var roomIdByte = new byte[4];
            stream.Read(roomIdByte, 0, 4);
            var roomId = BitConverter.ToInt32(roomIdByte);
            roomInfos.Add(roomId, title);
        }
        return roomInfos;
    }
}