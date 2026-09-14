using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    //Text
    public TextMeshProUGUI chatText;
    public TMP_InputField inputText;
    public TMP_InputField nameClient;

    //Panels
    public GameObject buttonsPanel;
    public GameObject chatPanel;

    //vars
    private string clientName;
    private string chatMessage;

    public Scrollbar scrollbar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!string.IsNullOrWhiteSpace(inputText.text))
                {
                    chatMessage = inputText.text;
                    inputText.text = "";
                    SendMessageServerRpc(clientName, chatMessage, NetworkObject.GetHashCode());
                }
            }
        }
    }

    private void LateUpdate()
    {
        scrollbar.value = 0f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string name, string message, int clientId)
    {
        chatText.text += name + ": " + message + "\n";
        ReceiveMessageClientRpc(name, message, clientId);
    }


    [ClientRpc]
    private void ReceiveMessageClientRpc(string name, string message, int clientId)
    {
        if (NetworkObject.GetHashCode() == clientId)
        {
            chatText.text += "<color=red>" + name + ": " + message + "\n" + "</color>";
        }
        else
        {
            chatText.text += name + ": " + message + "\n";
        }
    }

    //Butons
    public void ServerButton()
    {
        buttonsPanel.SetActive(false);
        chatPanel.SetActive(true);
        inputText.gameObject.SetActive(false);
        NetworkManager.Singleton.StartServer();
    }
    public void ClientButton()
    {
        if (!string.IsNullOrEmpty(nameClient.text))
        {
            buttonsPanel.SetActive(false);
            chatPanel.SetActive(true);
            clientName = nameClient.text;
            NetworkManager.Singleton.StartClient();
        }
    }
}
