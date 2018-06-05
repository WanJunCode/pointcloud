using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TcpClientHandle : MonoBehaviour
{
    Socket serverSocket; //服务器端socket
    IPAddress ip;
    IPEndPoint ipEnd;
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread;

    public Text mytext;
    string message;

    public Queue<string> queue = new Queue<string>();

    //初始化
    public void InitSocket()
    {
        string ipstr = PlayerPrefs.GetString("IP");
        ip = IPAddress.Parse(ipstr); //可以是局域网或互联网ip，此处是本机
        ipEnd = new IPEndPoint(ip, 7777); //服务器端口号
        //开启线程
        startThread();
    }

    void startThread()
    {
        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    //开启线程
    void SocketReceive()
    {
        SocketConnet();
        print("开启线程......");
        //不断接收服务器发来的数据
        while (true)
        {
            recvData = new byte[1024];
            recvLen = serverSocket.Receive(recvData);
            //如果接受长度 为 0 ，表示出现错误，重新连接
            if (recvLen == 0)
            {
                SocketConnet();
                continue;
            }
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            message = recvStr;
        }
    }

    //线程开启时连接，或者线程出现问题时重新连接
    void SocketConnet()
    {
        print("socket connect");
        //如果serverSocket不为空，说明连接出现了问题，于是先关闭server，再重新连接
        if (serverSocket != null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        print("ready to connect");
        //连接
        serverSocket.Connect(ipEnd);

        //输出初次连接收到的字符串
        recvLen = serverSocket.Receive(recvData);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        print("初次连接或者重新连接后。");
        print(recvStr);
    }

    private void Update()
    {
        if(queue.Count>0)
        {
            //移除并返回位于 Queue 开始处的对象
            //Peek()只返回，不移除
            string data = queue.Dequeue().ToString();
            
            sendData = new byte[1024];
            sendData = Encoding.ASCII.GetBytes(data);
            serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
        }
    }

    //发送数据
    public void SocketSend(string sendStr)
    {
        queue.Enqueue(sendStr);
    }

    //返回接收到的字符串
    public string GetRecvStr()
    {
        string returnStr;
        //加锁防止字符串读取时被更改
        lock (this)
        {
            returnStr = recvStr;
        }
        return returnStr;
    }

    public void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭服务器
        if (serverSocket != null)
            serverSocket.Close();
        print("diconnect");
    }
}
