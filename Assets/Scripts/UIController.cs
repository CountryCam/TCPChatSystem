using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public GameObject LoginView;
    public GameObject ChatView;

    public static UIController Instance;

    public bool IsServer = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TcpServer.OnServerCreated += () => 
        { 
            IsServer = true; 
            ShowChat(); 
        };
        MyClient.OnClientConected += () => { IsServer = false; ShowChat(); };
        ShowLogin();

    }

    void Update()
    {

    }

    private void ShowLogin()
    {
        LoginView.SetActive(true);
        ChatView.SetActive(false);
    }

    private void ShowChat()
    {
        LoginView.SetActive(false);
        ChatView.SetActive(true);
    }

}