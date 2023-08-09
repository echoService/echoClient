using System.Runtime.InteropServices;

namespace Client;

public class Inquiry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GetRoomListAns
    {
        private List<RoomInfo> _list { get; set; }

        public GetRoomListAns(List<RoomInfo> list)
        {
            this._list = list;
        }
    
        public GetRoomListAns()
        {
        
        }

        public List<RoomInfo> getRoomList()
        {
            return _list;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GetRoomListReq
    {
        public GetRoomListReq()
        {
        }
    }
}