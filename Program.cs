using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = null;
            Task receiveTask = null;

            try
            {
                // 클라이언트 Socket 생성
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 서버에 접속 시도
                client.Connect("localhost", 5555);
                Console.WriteLine("연결 성공!");

                // 데이터 송수신에 필요한 버퍼 생성
                var buffer = new Memory<byte>(new byte[1024]);

                string SendMessage = null;

                // 스트림에 처음 전달하는 문자열이 닉네임이 될 수 있도록 안내
                Console.WriteLine("닉네임을 입력해주세요:");

                // 서버로부터 메시지를 받는 비동기 메서드 호출
                receiveTask = ReceiveMessages(client, buffer);

                while (true)
                {
                    SendMessage = Console.ReadLine();
                    if (SendMessage == "exit")
                    {
                        break;
                    }

                    if (SendMessage == "")
                    {
                        continue;
                    }

                    // 문자열을 바이트로 인코딩하여 보내기
                    byte[] sendBytes = Encoding.UTF8.GetBytes(SendMessage);
                    client.Send(sendBytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // 연결 종료 및 비동기 메시지 수신 메서드 완료 대기
                if (client != null) client.Close();
                if (receiveTask != null) receiveTask.Wait();
            }
        }

        // 서버로부터 메시지를 받는 비동기 메서드
        static async Task ReceiveMessages(Socket client, Memory<byte> buffer)
        {
            try
            {
                while (true)
                {
                    // 서버로부터 비동기적으로 바이트 데이터를 받음
                    int receivedBytes = await client.ReceiveAsync(buffer, SocketFlags.None);
                    if (receivedBytes == 0)
                        break;

                    // 바이트 데이터를 문자열로 디코딩하여 콘솔에 표시
                    string message = Encoding.UTF8.GetString(buffer.Span.Slice(0, receivedBytes));
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
