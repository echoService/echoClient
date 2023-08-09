using System.Runtime.InteropServices;

namespace Client;

public class CreateRoom
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CreateRoomReq
    {
        private string _title { get;  set; }

        public CreateRoomReq(string title)
        {
            this._title = title;
        }

        public CreateRoomReq()
        {
        
        }

        public string GetTitle()
        {
            return this._title;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CreateRoomAns
    {
        private int _roomNum { get; set; }

        public CreateRoomAns(int roomNum)
        {
            this._roomNum = roomNum;
        }

        public CreateRoomAns()
        {
        
        }

        public int GetRoomNum()
        {
            return this._roomNum;
        }
    }
}