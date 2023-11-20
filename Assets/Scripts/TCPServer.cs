using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

                // Here pass the client to another method or class 
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
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

    public void StopServer()
    {
        isServerRunning = false;
        server?.Stop();
        serverThread?.Abort(); // Consider a more graceful approach
        serverThread = null;
        Debug.Log("Server stopped.");
    }

    private void OnDestroy()
    {
        StopServer();
    }

    // Here are methods like HandleClient(TcpClient client) to manage client connections
    private void HandleClient(TcpClient client)
    {
        try
        {
 
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // Decode the data received from the client
                string messageReceived = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received from client: " + messageReceived);

                chatScreen.AddMessageFromClient(messageReceived);
                // Process the message here
                string response = ProcessClientMessage(messageReceived);

                // Send a response back to the client
                byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Client handling error: " + ex.Message);
            if (client == null)
            {

                Debug.Log("Client null");
            }
        }
        finally
        {
            client.Close();
        }
    }

    private string ProcessClientMessage(string message)
    {
        // Here the message and return a response
        // For example, just echo the message back
        return "Server echo: " + message;
    }

}
