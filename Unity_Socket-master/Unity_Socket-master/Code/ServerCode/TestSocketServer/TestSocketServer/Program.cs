using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TestSocketServer
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("请输入服务端IP地址:");
            string host = Console.ReadLine();
            Console.WriteLine("请输入服务端端口号:");
            string port = Console.ReadLine();

            SocketServer server = new SocketServer();
            //只是本机测试，可以写127.0.0.1 ， 但是要让其他机器连接的话，要写实际ip地址
            //server.Start("192.168.0.171", 1234);
            server.Start(host, int.Parse(port));
            while (true)
            {
                string write = Console.ReadLine();
                switch (write)
                {
                    case "quit":
                        return;
                }
            }
        }


    }
}
