using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TcpTest : MonoBehaviour
{
    string editString = "hello wolrd"; //编辑框文字

    GameObject cube;

    public Text myText;

    TcpClientHandle tcpClient;

    void Start()
    {
        tcpClient = gameObject.AddComponent<TcpClientHandle>();

        //初始化
        tcpClient.InitSocket();

        cube = GameObject.Find("Cube");
    }

    void OnGUI()
    {
        editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
        GUI.Label(new Rect(10, 30, 600, 40), tcpClient.GetRecvStr());
        if (GUI.Button(new Rect(10, 70, 120, 40), "send"))
        {
            tcpClient.SocketSend(editString);
        }
    }

    public void mySendMsg()
    {
        tcpClient.SocketSend(myText.text);
    }

    void Update()
    {
        if (tcpClient.GetRecvStr() != null)
        {
            switch (tcpClient.GetRecvStr())
            {
                case "leftrotate":
                    cube.transform.Rotate(Vector3.up, 50 * Time.deltaTime);
                    break;
                case "rightrotate":
                    cube.transform.Rotate(Vector3.down, 50 * Time.deltaTime);
                    break;
            }
        }
    }

    void OnApplicationQuit()
    {
        if(tcpClient!=null)
        {
            //退出时关闭连接
            tcpClient.SocketQuit();
        }
    }
}
