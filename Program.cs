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
                // 서버와 통신을 위한 클라이언트 Socket 생성
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 서버에 접속 시도
                client.Connect("localhost", 5555);
                Console.WriteLine("연결 성공!");

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

                    // 작성한 문자열을 직렬화 해서 변수에 저장
                    byte[] sendBytes = Encoding.UTF8.GetBytes(SendMessage);
                    // 작성한 문자열의 길이가 어떻게 되는지 4바이트(정수)로 직렬화
                    byte[] length = BitConverter.GetBytes(sendBytes.Length);
                    // 길이 먼저 보내고
                    client.Send(length);
                    // 해당 문자열 보냄
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
                // 메모리를 직접 컨트롤하기 위한 MemoryStream 객체 생성
                MemoryStream memoryStream = new MemoryStream();
                // MemoryStream의 포지션을 다루기 위한 read / write 포지션 변수 선언
                int readPosition = 0;
                int writePosition = 0;

                // 클라이언트 소켓이 연결되어 있는 동안 무한 루프
                while (client.Connected)
                {
                    // 서버 측으로 부터 전달받은 데이터를 저장하기 위한 버퍼 초기화
                    byte[] buffer = new byte[1024];
                    // 서버 소켓에서 직렬화된 데이터를 읽어와서 변수에 저장함
                    int receivedBytes = await client.ReceiveAsync(buffer, SocketFlags.None);
                    if (receivedBytes == 0)
                        break;
                    // 데이터를 읽어왔음으로 MemoryStream offset이 뒤로 이동 -> 데이터를 쓰기 위해선 다시 앞으로 이동시켜줘야 함
                    memoryStream.Position = writePosition;
                    // buffer 메모리에 0부터 receivedBytes 변수에 저장된 만큼 Write 해줌
                    memoryStream.Write(buffer, 0, receivedBytes);
                    // writePosition에 받은 데이터의 길이 만큼 저장 -> 다음에 Write할 경우 해당 offset에서 부터 시작
                    writePosition += receivedBytes;
                    
                    // 무한 루프
                    while (true)
                    {
                        // MemoryStream 포지션을 다시 Read용으로 바꿔줌
                        memoryStream.Position = readPosition;
                        if (memoryStream.Length < 4)
                        {
                            break;
                        }

                        // 4바이트의 정수를 먼저 읽어오기 위한 메모리 할당
                        byte[] lengthBytes = new byte[4];
                        // lenfthBytes 변수에 0부터 4 Offset의 데이터를 저장
                        memoryStream.Read(lengthBytes, 0, 4);
                        // lengthBytes 변수의 0번 Offset부터 읽어와서 정수로 변환한 값을 length 변수에 저장
                        int length = BitConverter.ToInt32(lengthBytes, 0);
                        //Console.WriteLine("========================================= {0}", length);

                        // 데이터가 모두 도착했는지 확인
                        if (memoryStream.Length < (length + 4))
                        {
                            break;
                        }

                        // 길이 뒤에 보내진 실제 데이터값을 읽어오기 위해 readPosition 변경
                        readPosition += 4;
                        memoryStream.Position = readPosition;
                        // 실제 데이터 길이 만큼의 메모리를 할당
                        byte[] dataBytes = new byte[length];
                        // dataBytes 변수에 0번 Offset부터 길이까지를 읽어옴
                        memoryStream.Read(dataBytes, 0, length);

                        // 바이트 데이터를 문자열로 역직렬화 해서 콘솔창에 표시
                        string message = Encoding.UTF8.GetString(dataBytes);
                        Console.WriteLine("Received: " + message);

                        // 뒤에 남아있는 데이터가 있을 수 있음으로 확인하는 절차 필요 -> memoryStream의 전체 길이에서 현재까지 읽어온 데이터의 길이를 비교
                        long remainingLength = memoryStream.Length - (length + 4);
                        // 남아있는 데이터가 있다면 if문 실행
                        if (remainingLength > 0)
                        {
                            // 남아있는 길이만큼 메모리 할당
                            byte[] remainingData = new byte[remainingLength];
                            // 뒤에 데이터를 더 읽어야 함으로 readPosition 변경
                            readPosition += length;
                            memoryStream.Position = readPosition;
                            // 남아있는 데이터를 읽어와서 remainginData 변수에 저장
                            memoryStream.Read(remainingData, 0, (int)remainingLength);

                            // 남은 데이터를 메모리 내에서 이동
                            byte[] innerBuffer = memoryStream.GetBuffer();
                            Buffer.BlockCopy(remainingData, 0, innerBuffer, 0, (int)remainingLength);

                            // MemoryStream의 길이를 조정하여 남은 데이터를 삭제
                            memoryStream.SetLength(remainingLength);
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
