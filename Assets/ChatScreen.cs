using UnityEngine;
using System;
using System.Net.Sockets;
using UnityEngine.UI; // If using standard UI Text
using TMPro; // If using TextMeshPro

public class ChatScreen : MonoBehaviour
{
    // Public field to assign your Text component in the editor

    public TMP_Text messagePrefab;
    public Transform messageContainer;
    //public TMP_Text chatDisplay;
    public TMP_InputField messageInputField; // Assign in editor
    public Button sendButton; // Assign in editor
    //public TcpClient client;
    //public TcpServer server;
    public ScrollRect scrollRect;
    string userName;
    


    void Start()
    {
        MyClient.OnMessageReceived += AddMessageFromServer;
        TcpServer.OnMessageReceivedServer += AddMessageFromClient;
        Debug.Log("MyClient Instance: " + (MyClient.Instance != null));
        //Debug.Log("Chat Display: " + (chatDisplay != null));
        // Add a listener to the button click event
        sendButton.onClick.AddListener(SendMessage);
        userName = LoginScreen.Instance.Name.text;
    }
    public void AddMessageFromServer(string message,string userName)
    {
        AddMessageToChat(userName + message);
    }
    public void AddMessageFromClient(string message, string userName)
    {
        AddMessageToChat(userName + message);
    }
    // Method to add a message to the chat display
    public void AddMessageToChat(string message)
    {
        TMP_Text newMessage = Instantiate(messagePrefab, messageContainer);
        newMessage.text = message;
        Canvas.ForceUpdateCanvases(); // Update the canvas immediately
        scrollRect.verticalNormalizedPosition = 0; // Scroll to bottom
        //chatDisplay.text += message + "\n";
    }

    public void SendMessage()
    {
        string messageToSend = messageInputField.text;
        if (!string.IsNullOrWhiteSpace(messageToSend))
        {
            // Send the message through the client or the server
            string senderName = userName + "";
            if (!UIController.Instance.IsServer)
            {
                MyClient.Instance.SendMessageToServer(senderName+":"+messageToSend);
            }
            else
            {
                TcpServer.Instance.SendDataServer(senderName+":"+messageToSend);
            }

            // Clear the input field
            messageInputField.text = "";
            // Add message to chat display
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                string prefix = UIController.Instance.IsServer ? senderName + ":" : senderName + ":";
                AddMessageToChat(prefix + messageToSend);
            });
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
