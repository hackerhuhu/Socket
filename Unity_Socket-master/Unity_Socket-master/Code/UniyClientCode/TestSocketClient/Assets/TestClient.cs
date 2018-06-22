using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using System;

public class TestClient : MonoBehaviour {

    private Socket m_Socket;

    public InputField HostField;
    public InputField PortField;
    public InputField MessageField;
    public InputField LinkMessageField;
    public InputField ReceiveFiled;
    private byte[] readBuff = new byte[1024];

    private string reveString; //接收的字符数据
    private bool isReceived = false; //数据接收完成
	// Use this for initialization
	void Start ()
    {
        //设置后台运行，数据就会立马同步更新。否则其他客户端发送一条消息，服务端进行广播，但是Unity进程被挂起了，就无法实时更新
        Application.runInBackground = true;
	}
	
    public void LinkServer()
    {      
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            m_Socket.Connect(HostField.text, int.Parse(PortField.text));
            LinkMessageField.text = "连接成功-----" + m_Socket.LocalEndPoint.ToString();
        }
        catch (Exception)
        {
            LinkMessageField.text = "连接失败！！";
            throw;
        }
        
        m_Socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, null);
    }


    public void SendMessageToServer()
    {
        try
        {
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(MessageField.text);
            m_Socket.Send(sendBytes);
        }
        catch (Exception)
        {
            throw;
        }

    }

    //服务器返回回调
    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int count = m_Socket.EndReceive(ar);
            reveString = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            isReceived = true;
            //之所以不直接在这里赋值，是因为线程问题，会报错，该回调不是在unity主线程中执行的，所以赋值放在update中
            //if (ReceiveFiled.text.Length > 500)
            //{
            //    ReceiveFiled.text = "";
            //}
            //ReceiveFiled.text += reveString + '\n';
            //继续接收返回信息
            m_Socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, null);
        }
        catch (Exception)
        {
            reveString = m_Socket.LocalEndPoint.ToString() + "连接断开";
            isReceived = true;
            m_Socket.Close();
            throw;
        }

    }

    private void Update()
    {
        if (isReceived)
        {
            if (ReceiveFiled.text.Length > 500)
            {
                ReceiveFiled.text = "";
            }
            ReceiveFiled.text += reveString + '\n';
            isReceived = false;
        }
    }

}
