using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkStream NS = null;
            StreamReader SR = null;
            StreamWriter SW = null;
            Socket client = null;

            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect("localhost", 5555);
                Console.WriteLine("연결 성공!");
                NS = new NetworkStream(client);
                SR = new StreamReader(NS, Encoding.UTF8);
                SW = new StreamWriter(NS, Encoding.UTF8);

                string SendMessage = null;
                string GetMessage = null;

                // 비동기적으로 입력을 처리하도록 변경
                Console.WriteLine("메시지를 입력하세요 (exit를 입력하여 종료):");

                while (true)
                {
                    // 사용자 입력이 있는지 확인
                    if (Console.KeyAvailable)
                    {
                        SendMessage = Console.ReadLine();
                        if (SendMessage == "exit")
                            break;

                        SW.WriteLine(SendMessage);
                        SW.Flush();
                    }

                    // 서버로부터 메시지를 받아오기
                    if (NS.DataAvailable)
                    {
                        GetMessage = SR.ReadLine();
                        //Console.WriteLine("서버로부터 메세지 전달받음");
                        Console.WriteLine(GetMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (SW != null) SW.Close();
                if (SR != null) SR.Close();
                if (client != null) client.Close();
            }
        }
    }
}