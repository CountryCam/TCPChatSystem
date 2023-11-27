using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyClient : MonoBehaviour
{
    public static MyClient Instance;
    private TcpClient client = null;
   
    
    public string host;
    public int port;
    public Button ClientButton;

    private NetworkStream stream;
    private Thread receiverThread;
    //private bool isConnected = false;

    public static Action OnClientConected;
    public static Action<string> OnMessageReceived;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C) && !isConnected)
        //{
        //    ConnectToServer();
        //}
    }

   public void ConnectToServer(string host, int port)
    {
        try
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
            if (client.Connected)
            {
                //isConnected = true;
                receiverThread = new Thread(ReceiveData);
                receiverThread.Start();
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    OnClientConected?.Invoke();
                });
                    //OnClientConected?.Invoke();
                //SendDataClient("Hi, this is the client!");
                Debug.Log("Connected to server successfully.");
            }
            else
            {
               // isConnected = false;
                Debug.LogError("Failed to connect to server.");
            }


        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect: " + e.Message);
            //isConnected = false;    
        }
    }

    public void SendDataClient(string msg)
    {
        try
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
            Debug.Log("Client Sent: " + msg);

        }
        catch (Exception e)
        {
            Debug.LogError("Error in sending data: " + e.Message);
        }
    }

    public void SendMessageToServer(string message)
    {
        SendDataClient(message); 
    }
    public void ReceiveData()
    {
        try
        {
            while (client != null && client.Connected)
            {
                Byte[] bytes = new byte[256];
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                if (bytesRead > 0)
                {
                    //string msgFromServer = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    string msgFromServer = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OnMessageReceived?.Invoke(msgFromServer);
                    });
                    //OnMessageReceived?.Invoke(msgFromServer);
                    Debug.Log("Message from Server: " + msgFromServer);

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error in receiving data: " + e.Message);
        }
        finally
        {
            
            //isConnected = false;
            
        }
    }

    private void OnApplicationQuit()
    {
        if (client != null)
        {
            if (receiverThread != null)
            {
                receiverThread.Abort();
                receiverThread = null;
            }

            stream?.Close();
            client.Close();
            //isConnected = false;
        }
    }
}
