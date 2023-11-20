using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using UnityEditor.PackageManager;
using UnityEngine;

public class TcpServer : MonoBehaviour
{
    // Server Listner
    private TcpListener server;
    public static TcpServer Instance;
    public int portLink;
    public string hostLink;
    public static Action OnServerCreated;
    ChatScreen chatScreen;

    private bool isServerRunning = false;
    private Thread serverThread = null;

    private NetworkStream stream = null;

    public static Action<string> OnMessageReceivedServer;
    private void Awake()
    {
        Instance = this;
    }

    public void CreateServer(string ipAddress, int port)
    {
        if (isServerRunning)
        {
            Debug.LogWarning("Server is already running.");
            return;
        }

        this.portLink = port;
        this.hostLink = ipAddress;

        isServerRunning = true;
        serverThread = new Thread(ReceiverThread);
        serverThread.Start();

        if (OnServerCreated != null)
        {
            OnServerCreated.Invoke();
        }
    }

    private void ReceiverThread()
    {
        try
        {
            IPAddress iPAddress = IPAddress.Parse(hostLink);
            Debug.Log("Starting server on " + iPAddress.ToString() + ":" + portLink);
            server = new TcpListener(iPAddress, portLink);
            server.Start();
            Debug.Log("Server created successfully");

            while (isServerRunning)
            {
                if (!server.Pending())
                {
                    Thread.Sleep(100); // Reduce CPU usage
                    continue;
                }

                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Client connected: " + client.Client.RemoteEndPoint);
                stream = client.GetStream();
                //read message from the network stream 
                //print
                Byte[] bytes = new byte[1024];
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                if (bytesRead > 0)
                {
                    string msgFromClient = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    OnMessageReceivedServer?.Invoke(msgFromClient);
                    Debug.Log("Message from Client: " + msgFromClient);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Server encountered an error: " + ex.Message);
        }
        finally
        {
            server.Stop();
            Debug.Log("Server stopped listening.");
        }
    }

    public void SendData(string msg)
    {
        try
        {
            //isConnected = true;
            Byte[] bytes = Encoding.ASCII.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
            Debug.Log("Server Sent: " + msg);

        }
        catch (Exception e)
        {
            Debug.LogError("Error in sending data: " + e.Message);
        }
    }

    public void StopServer()
    {
        isServerRunning = false;
        server?.Stop();
        serverThread?.Abort();
        serverThread = null;
        Debug.Log("Server stopped.");
    }

    private void OnDestroy()
    {
        StopServer();
    }


}
