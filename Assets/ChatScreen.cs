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
        Debug.Log("MyClient Instance: " + (MyClient.Instance != null));
        Debug.Log("Chat Display: " + (chatDisplay != null));
        // Add a listener to the button click event
        sendButton.onClick.AddListener(SendMessage);
    }
    public void AddMessageFromServer(string message)
    {
        AddMessageToChat("Server: " + message);
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
                AddMessageToChat("You: " + messageToSend + "\n");

            }
            else
            {
                TcpServer.Instance.SendData(messageToSend);
                messageInputField.text = "";
                AddMessageToChat("Server: " + messageToSend + "\n");
            }
        }
    }
    private void OnDestroy()
    {
        MyClient.OnMessageReceived -= AddMessageFromServer;
        sendButton.onClick.RemoveListener(SendMessage);
    }
    public void AddMessageFromClient(string message)
    {
        AddMessageToChat("Client: " + message);
    }

    void Update()
    {
        // Update logic (if any)
    }
}
