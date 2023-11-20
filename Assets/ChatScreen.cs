using UnityEngine;
using System;
using System.Net.Sockets;
using UnityEngine.UI; // If using standard UI Text
using TMPro; // If using TextMeshPro

public class ChatScreen : MonoBehaviour
{
    // Public field to assign your Text component in the editor
    public TMP_Text chatDisplay;
    public TMP_InputField messageInputField; // Assign in editor
    public Button sendButton; // Assign in editor
    //public TcpClient client;
    //public TcpServer server;

    void Start()
    {
        MyClient.OnMessageReceived += AddMessageFromServer;
        TcpServer.OnMessageReceivedServer += AddMessageFromClient;
        Debug.Log("MyClient Instance: " + (MyClient.Instance != null));
        Debug.Log("Chat Display: " + (chatDisplay != null));
        // Add a listener to the button click event
        sendButton.onClick.AddListener(SendMessage);
    }
    public void AddMessageFromServer(string message)
    {
        AddMessageToChat("Server: " + message);
    }
    public void AddMessageFromClient(string message)
    {
        AddMessageToChat("Client: " + message);
    }
    // Method to add a message to the chat display
    public void AddMessageToChat(string message)
    {
        chatDisplay.text += message + "\n";
    }

    public void SendMessage()
    {
        string messageToSend = messageInputField.text;
        if (!string.IsNullOrWhiteSpace(messageToSend))
        {
            if (!UIController.Instance.IsServer)
            {
                MyClient.Instance.SendMessageToServer(messageToSend);
                messageInputField.text = "";
                UnityMainThreadDispatcher.Instance().Enqueue(() => { 
                    AddMessageToChat("You: " + messageToSend + "\n");
                    Debug.Log("Added to main thread");
                
                });
                
            }
            else
            {
                TcpServer.Instance.SendData(messageToSend);
                messageInputField.text = "";
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    AddMessageToChat("Server: " + messageToSend + "\n");
                    Debug.Log("Added to main thread");

                });
                
            }
        }
    }
    private void OnDestroy()
    {
        MyClient.OnMessageReceived -= AddMessageFromServer;
        TcpServer.OnMessageReceivedServer -= AddMessageFromClient;
        sendButton.onClick.RemoveListener(SendMessage);
    }

    void Update()
    {
        // Update logic (if any)
    }
}
