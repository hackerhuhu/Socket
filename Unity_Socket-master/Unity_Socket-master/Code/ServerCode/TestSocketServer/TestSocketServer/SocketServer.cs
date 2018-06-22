using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TestSocketServer
{
    /// <summary>
    /// 服务器类
    /// </summary>
    class SocketServer
    {
        //监听socket对象
        public Socket listenSocket;
        //客户端连接池
        public ConnectClient[] clientList;
        //容纳客户端数量
        public int maxClient = 50;

        /// <summary>
        /// 从连接池中 获取客户端连接对象 ，如果列表中以满 则获取失败
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            //如果连接池为空 则新建连接池 返回第一个连接对象
            if (clientList == null)
            {
                clientList = new ConnectClient[maxClient];
                clientList[0] = new ConnectClient();
                return 0;
            }
            else
            {
                //遍历连接池 ， 返回未使用连接对象的索引
                for (int i = 0; i < clientList.Length; i++)
                {
                    if (clientList[i] == null)
                    {
                        clientList[i] = new ConnectClient();
                        return i;
                    }
                    else if (clientList[i].isUse == false)
                    {
                        return i;
                    }
                }
                //如果都有对象且在使用中，则返回-1. 代表获取失败
                return -1;
            }
        }

        //接收回调
        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenSocket.EndAccept(ar);
                int index = GetIndex();
                if (index< 0)
                {
                    socket.Close();
                    Console.WriteLine("服务器连接已满，请稍候再试");
                }
                else
                {
                    ConnectClient client = clientList[index];
                    client.Init(socket);
                    client.socket.BeginReceive(client.readBuff, client.bufferCount, client.BufferRemain(), SocketFlags.None, ReceiveCallBack,client);
                    Console.WriteLine("客户端连接成功 = " + client.Address());
                }
                //重新开始异步接收请求
                listenSocket.BeginAccept(AcceptCallBack, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("客户端请求异常! Exception = " + e.Message);
            }

        }
        //返回回调
        private void ReceiveCallBack(IAsyncResult ar)
        {
            ConnectClient client = (ConnectClient)ar.AsyncState;
            try
            {
                int count = client.socket.EndReceive(ar);
                //断开连接
                if (count <= 0)
                {
                    Console.WriteLine("断开连接  = " + client.Address());
                    client.Close();
                }
                else
                {
                    string receiveString = System.Text.Encoding.UTF8.GetString(client.readBuff, 0, count);
                    Console.WriteLine("接收 " + client.Address() + "    的数据 =  " + receiveString);
                    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(client.Address() + " :   " + receiveString);

                    //广播信息
                    for (int i = 0; i < clientList.Length; i++)
                    {
                        if (clientList[i] == null)
                        {
                            continue;
                        }

                        if (clientList[i].isUse == false)
                        {
                            continue;
                        }
                        clientList[i].socket.Send(sendBytes);
                        Console.WriteLine("广播 " + client.Address() + " 的数据 给 " + clientList[i].Address());
                    }
                }
                //继续接收数据
                client.socket.BeginReceive(client.readBuff, client.bufferCount, client.BufferRemain(), SocketFlags.None, ReceiveCallBack, client);
            }
            catch (Exception e)
            {
                Console.WriteLine("[接收数据异常]  client = " + client.Address());
                Console.WriteLine(" Execption = " + e.Message);
                client.Close();
            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="maxClient">容纳客户端数量 (默认50)</param>
        public void Start(string host , int port , int maxClient = 50)
        {
            //初始化连接池
            this.maxClient = maxClient;
            clientList = new ConnectClient[this.maxClient];

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipa = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ipa, port);
            listenSocket.Bind(ipe);
            listenSocket.Listen(maxClient);
            //开启异步接收连接
            listenSocket.BeginAccept(AcceptCallBack, null);

            Console.WriteLine("服务器启动成功！");
        }


    }
}
