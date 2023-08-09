using System.Net.Sockets;

namespace Client
{

    class Program
    {
        public static int RoomNum;

        static async Task Main(string[] args)
        {
            Socket client = null;
            var headerService = new HeaderService();
            var chatService = new ChatService(headerService);
            var createService = new CreateService(headerService);
            var joinService = new JoinService(headerService);
            var inquiryService = new InquiryService();
            var chatProcess = new ChatMessageHandler(chatService);
            var createProcess = new CreateMessageHandler(createService);
            var joinProcess = new JoinMessageHandler(joinService);
            var inquiryProcess = new InquiryMessageHandler(inquiryService, headerService);
            var requestDispatcher = new Dispatcher(chatProcess, createProcess, joinProcess, inquiryProcess);
            
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                MessageReceiver messageReceiver = new MessageReceiver(client, requestDispatcher, headerService);
                client.Connect("localhost", 5555);
                Console.WriteLine("연결 성공!");
                
                Task.Run(() => messageReceiver.ReceiveMessagesAsync());
                
                while (true)
                {
                    requestDispatcher.DispatchRequest(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (client != null) client.Close();
            }
        }
    }
}