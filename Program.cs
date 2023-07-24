using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int PORT = 5555;
            string IP = "localhost";

            NetworkStream NS = null;
            StreamReader SR = null;
            StreamWriter SW = null;
            TcpClient client = null;

            try
            {
                client = new TcpClient(IP, PORT);
                Console.WriteLine("연결 성공!");
                NS = client.GetStream();
                Console.WriteLine("소켓에서 메시지를 가져오는 스트림 생성");
                SR = new StreamReader(NS, Encoding.UTF8);
                Console.WriteLine("네트워크 스트림으로부터 메시지를 가져오는 스트림 생성");
                SW = new StreamWriter(NS, Encoding.UTF8);
                Console.WriteLine("네트워크 스트림으로 메시지를 보내는 스트림 생성");


                string SendMessage = null;
                string GetMessage = null;
                while ((SendMessage = Console.ReadLine()) != null)
                {
                    SW.WriteLine(SendMessage);
                    Console.WriteLine("스트림에 문자열 입력");
                    SW.Flush();
                    Console.WriteLine("버퍼를 비우고 서버로 문자 전송");

                    GetMessage = SR.ReadLine();
                    Console.WriteLine("서버로부터 메세지 전달받음");
                    Console.WriteLine(GetMessage);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
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