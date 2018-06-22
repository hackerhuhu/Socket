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
    /// 客户端连接对象类
    /// </summary>
    class ConnectClient
    {
        //缓冲区大小+
        public const int BUFFER_SIZE = 1024;
        public Socket socket;
        //是否使用
        public bool isUse = false; 
        //缓冲区
        public byte[] readBuff = new byte[BUFFER_SIZE];
        //数据大小
        public int bufferCount;

        //构造
        public ConnectClient()
        {
            readBuff = new byte[BUFFER_SIZE];
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="soc"></param>
        public void Init(Socket soc)
        {
            this.socket = soc;
            isUse = true;
            bufferCount = 0;
        }
        
        /// <summary>
        /// 缓冲区剩余字节空间
        /// </summary>
        /// <returns></returns>
        public int BufferRemain()
        {
            return BUFFER_SIZE - bufferCount;
        }

        /// <summary>
        /// 获取socket连接地址
        /// </summary>
        /// <returns></returns>
        public string Address()
        {
            if (!isUse)
            {
                return null;
            }
            else
            {
                return socket.RemoteEndPoint.ToString();
            }

        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (!isUse)
            {
                return;
            }
            else
            {
                Console.WriteLine(Address() + " [ 断开连接 ]");
                socket.Close();
                isUse = false;
            }
        }
    }
}
