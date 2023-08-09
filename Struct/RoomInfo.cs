using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Client;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class RoomInfo
{
    private string _title { get; set; }
    private int _roomNum { get; set; }
    private List<Socket> _sockets { get; set; }

    public RoomInfo(string title, int roomNum, List<Socket> sockets)
    {
        this._title = title;
        this._roomNum = roomNum;
        this._sockets = sockets;
    }

    public string GetTitle()
    {
        return _title;
    }

    public int GetRoomNum()
    {
        return _roomNum;
    }

    public List<Socket> GetSocketList()
    {
        return _sockets;
    }
}