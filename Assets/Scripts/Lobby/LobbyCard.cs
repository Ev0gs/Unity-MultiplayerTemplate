using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText = default;
    [SerializeField] private TextMeshProUGUI playerNameText = default;
    private Lobby lobby = default;
    public void EnterLobby()
    {
        LobbyManager.Instance.EnterLobby(lobby);
    }

    public void SetLobbyInfo(Lobby lobby, string lobbyName, string playerName)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobbyName;
        playerNameText.text = playerName;   
    }
}
