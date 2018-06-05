using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class toPointCloud : MonoBehaviour
{

    public Text ipText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setIP()
    {
        //保存 输入框 中的IP 地址
        PlayerPrefs.SetString("IP", ipText.text);
        SceneManager.LoadScene("scene");
    }
}
