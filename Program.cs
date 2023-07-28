using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
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
                

                string SendMessage = null;

                // 서버로부터 메시지를 받는 비동기 메서드 호출
                receiveTask = ReceiveMessages(client);

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
                    byte[] length = BitConverter.GetBytes(sendBytes.Length);
                    client.Send(length);
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
        static async Task ReceiveMessages(Socket client)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                int readPosition = 0;
                int writePosition = 0;

                while (client.Connected)
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = await client.ReceiveAsync(buffer, SocketFlags.None);
                    if (receivedBytes == 0)
                        break;
                    memoryStream.Position = writePosition;
                    memoryStream.Write(buffer, 0, receivedBytes);
                    writePosition += receivedBytes;
                    
                    while (true)
                    {
                        memoryStream.Position = readPosition;
                        if (memoryStream.Length < 4)
                        {
                            break;
                        }

                        byte[] lengthBytes = new byte[4];
                        memoryStream.Read(lengthBytes, 0, 4);
                        int length = BitConverter.ToInt32(lengthBytes, 0);
                        //Console.WriteLine("========================================= {0}", length);

                        // 데이터가 모두 도착했는지 확인
                        if (memoryStream.Length < (length + 4))
                        {
                            break;
                        }

                        // 실제 데이터를 읽어옴
                        readPosition += 4;
                        memoryStream.Position = readPosition;
                        byte[] dataBytes = new byte[length];
                        memoryStream.Read(dataBytes, 0, length);

                        // 바이트 데이터를 문자열로 디코딩하여 콘솔에 표시
                        string message = Encoding.UTF8.GetString(dataBytes);
                        Console.WriteLine("Received: " + message);

                        long remainingLength = memoryStream.Length - (length + 4);
                        if (remainingLength > 0)
                        {
                            byte[] remainingData = new byte[remainingLength];
                            readPosition += length;
                            memoryStream.Position = readPosition;
                            memoryStream.Read(remainingData, 0, (int)remainingLength);

                            // 남은 데이터를 메모리 내에서 이동
                            byte[] innerBuffer = memoryStream.GetBuffer();
                            Buffer.BlockCopy(remainingData, 0, innerBuffer, 0, (int)remainingLength);

                            // MemoryStream의 길이를 조정하여 남은 데이터를 삭제
                            memoryStream.SetLength(remainingLength);
                            
                            readPosition = 0;
                            writePosition = 0;
                        }
                        else
                        {
                            memoryStream.SetLength(0);
                            
                            readPosition = 0;
                            writePosition = 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
