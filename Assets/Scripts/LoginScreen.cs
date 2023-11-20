using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public TMP_InputField IPAddress;
    public TMP_InputField Port;
    public Button ServerButton;
    public Button ClientButton;


    public static Action onServerConnected;

    void Start()
    {
        ServerButton.onClick.AddListener(ServerButtonClicked);
        ClientButton.onClick.AddListener(ClientButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ServerButtonClicked()
    {
        Debug.Log("SErver CLicked");
        TcpServer.Instance.CreateServer(IPAddress.text, int.Parse(Port.text));
    }

    void ClientButtonClicked()
    {
        Debug.Log("Client CLicked");
        MyClient.Instance.ConnectToServer(IPAddress.text, int.Parse(Port.text));
    }
}
