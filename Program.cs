using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
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

                // 데이터를 버퍼링하여 보낼 메모리스트림과 버퍼 크기를 지정
                MemoryStream bufferedStream = new MemoryStream();
                int bufferSize = 1024;

                // 무한 반복
                while (true)
                {
                    // 콘솔에 적는 문자열을 변수에 저장
                    SendMessage = Console.ReadLine();
                    
                    // 나가고 싶을 땐 exit 타이핑
                    if (SendMessage == "exit")
                    {
                        break;
                    }

                    // 공백을 타이핑 했을 경우 continue로 올려버림
                    if (SendMessage == "")
                    {
                        continue;
                    }

                    // 작성한 문자열을 직렬화해서 변수에 저장
                    byte[] messageBytes = Encoding.UTF8.GetBytes(SendMessage);
                    
                    // 문자열의 길이 정수(4바이트)와 실제 문자열의 길이를 합쳐서 보내기 위한 메모리 공간 설정
                    byte[] sendBytes = new byte[4 + messageBytes.Length];
                    // sendBytes 메모리에 복사
                    Buffer.BlockCopy(BitConverter.GetBytes(messageBytes.Length), 0, sendBytes, 0, 4);
                    Buffer.BlockCopy(messageBytes, 0, sendBytes, 4, messageBytes.Length);

                    // bufferedStream에 sendBytes 메모리에 있는 값을 작성
                    bufferedStream.Write(sendBytes, 0, sendBytes.Length);

                    // SendBufferedData 메소드 내부적으로 메모리 크기를 확인하여 1024보다 작을 경우 메소드가 종료되도록 설정
                    // 1024가 넘어갈 경우 I/O 작업이 발생할 수 있음으로 비동기로 실행
                    await SendBufferedData(client, bufferedStream);
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
                if (receiveTask != null) await receiveTask;
            }
        }

        // 서버로부터 메시지를 받는 비동기 메서드
        static async Task ReceiveMessages(Socket client)
        {
            try
            {
                // 메모리를 직접 컨트롤하기 위한 MemoryStream 객체 생성
                MemoryStream receiveMemoryStream = new MemoryStream();
                
                // MemoryStream의 포지션을 다루기 위한 read / write 포지션 변수 선언
                int readPosition = 0;
                int writePosition = 0;

                // 클라이언트 소켓이 연결되어 있는 동안 무한 루프
                while (client.Connected)
                {
                    // 서버 측으로 부터 전달받은 데이터를 저장하기 위한 버퍼 초기화
                    byte[] buffer = new byte[1024];
                    
                    // 서버 소켓에서 직렬화된 데이터의 길이를 변수에 저장함
                    int receivedBytes = await client.ReceiveAsync(buffer, SocketFlags.None);
                    if (receivedBytes == 0)
                        break;
                    
                    // 데이터를 읽어왔음으로 MemoryStream offset이 뒤로 이동 -> 데이터를 쓰기 위해선 다시 앞으로 이동시켜줘야 함
                    receiveMemoryStream.Position = writePosition;
                    // buffer 메모리에 0부터 receivedBytes 변수에 저장된 만큼 Write 해줌
                    receiveMemoryStream.Write(buffer, 0, receivedBytes);
                    // writePosition에 받은 데이터의 길이 만큼 저장 -> 다음에 Write할 경우 해당 offset에서 부터 시작
                    writePosition += receivedBytes;
                    
                    // 무한 루프
                    while (true)
                    {
                        // MemoryStream 포지션을 다시 Read용으로 바꿔줌
                        receiveMemoryStream.Position = readPosition;
                        if (receiveMemoryStream.Length < 4)
                        {
                            break;
                        }

                        // 4바이트의 정수를 먼저 읽어오기 위한 메모리 할당
                        byte[] lengthBytes = new byte[4];
                        
                        // lenfthBytes 변수에 0부터 4의 데이터(문자열 데이터 길이)를 저장
                        int bytesRead = receiveMemoryStream.Read(lengthBytes, 0, 4);
                        
                        if (bytesRead != 4)
                        {
                            // 4바이트를 읽을 수 없으면 루프 종료
                            break;
                        }
                        
                        // 문자열 길이 정수값으로 변환
                        int length = BitConverter.ToInt32(lengthBytes, 0);

                        // 데이터가 모두 도착했는지 확인
                        if (receiveMemoryStream.Length < (length + 4))
                        {
                            break;
                        }

                        // 뒤에 보내진 실제 데이터값을 읽어오기 위해 readPosition 변경
                        readPosition += 4;
                        receiveMemoryStream.Position = readPosition;
                        
                        // 실제 데이터 길이 만큼의 메모리를 할당
                        byte[] dataBytes = new byte[length];
                        
                        // dataBytes 변수에 0번 Offset부터 길이까지를 읽어옴
                        receiveMemoryStream.Read(dataBytes, 0, length);
                        readPosition += length;

                        // 바이트 데이터를 문자열로 역직렬화 해서 콘솔창에 표시
                        string message = Encoding.UTF8.GetString(dataBytes);
                        Console.WriteLine("Received: " + message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // 버퍼에 쌓인 데이터를 전송하는 비동기 메서드
        static async Task SendBufferedData(Socket client, MemoryStream bufferedStream)
        {
            // 1024 바이트까지 버퍼링하기 위해 bufferedStream의 길이가 1024가 넘을 경우에만 로직이 실행되도록 if문 선언
            if (bufferedStream.Length >= 1024)
            {
                // bufferedStream의 길이만큼 메모리 확보
                byte[] sendBytes = new byte[bufferedStream.Length];
                // Offset을 처음부터 읽어야 하기 때문에 0으로 변경
                bufferedStream.Position = 0;
                // sendBytes에 모든 데이터 복사
                bufferedStream.Read(sendBytes, 0, (int)bufferedStream.Length);

                // 버퍼링된 데이터 모두 전송
                await client.SendAsync(sendBytes, SocketFlags.None);

                // 모든 데이터를 보냈음으로 버퍼 비우기
                bufferedStream.SetLength(0);
            }
        }
    }
}
