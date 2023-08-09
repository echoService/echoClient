using System.Runtime.InteropServices;

namespace Client;

public class JoinRoom
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JoinRoomReq
    {
        private int _roomNum { get; set; }

        public JoinRoomReq(int roomNum)
        {
            _roomNum = roomNum;
        }
    
        public JoinRoomReq()
        {
        }

        public int GetRoomNum()
        {
            return _roomNum;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class JoinRoomAns
    {
        private int _roomNum { get; set; }

        public JoinRoomAns(int roomNum)
        {
            this._roomNum = roomNum;
        }

        public int GetRoomNum()
        {
            return this._roomNum;
        }

        public JoinRoomAns()
        {
        
        }
    }
}