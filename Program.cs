using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    // 클라이언트 흐름: Socket 객체 생성 -> Connect()를 통해 서버에 설정된 IP, PORT로 연결 시도 -> 비동기로 메세지를 수신하는 로직을 다른 스레드에 맡김 -> 반복문을 통해서 채팅을 입력하면 내용을 서버로 전송 -> 무한 반복
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
                Console.WriteLine("닉네임을 입력해주세요:");

                // 서버로부터 메시지를 받는 스레드 별도로 시작
                Task.Run(() => ReceiveMessages(SR));
                

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

        // 서버로부터 메시지를 받는 메서드
        static void ReceiveMessages(StreamReader reader)
        {
            try
            {
                while (true)
                {
                    string message = reader.ReadLine();
                    if (message == null)
                        break;

                    Console.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
