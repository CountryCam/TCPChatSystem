using System;
using System.Collections.Generic;
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
    //ChatScreen chatScreen;

    private bool isServerRunning = false;
    private Thread serverThread = null;
    private List<TcpClient> clients = new List<TcpClient>();
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
                clients.Add(client);
                Debug.Log("Client connected: " + client.Client.RemoteEndPoint);
                stream = client.GetStream();

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
                
                //read message from the network stream 
                //print
                Byte[] bytes = new byte[1024];
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                if (bytesRead > 0)
                {
                    string msgFromClient = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OnMessageReceivedServer?.Invoke(msgFromClient);
                        Debug.Log("Message from Client: " + msgFromClient);
                    });
                    //OnMessageReceivedServer?.Invoke(msgFromClient);
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

    private void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            while (client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string msgFromClient = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OnMessageReceivedServer?.Invoke(msgFromClient);
                        Debug.Log("Message from Client: " + msgFromClient);
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error handling client: " + ex.Message);
        }
        finally
        {
            // Close the client connection and remove it from the list
            stream.Close();
            client.Close();
            clients.Remove(client);
        }
    }

    public void SendDataServer(string msg)
    {
        try
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(msg);
            foreach (TcpClient client in clients) 
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    if (stream.CanWrite)
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            
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
