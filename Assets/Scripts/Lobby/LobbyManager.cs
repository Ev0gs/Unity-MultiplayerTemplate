using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Linq;
using Unity.VisualScripting;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject lobbyCardPFB = default;
    [SerializeField] private GameObject noLobbyPFB = default;
    [SerializeField] private Transform lobbyListContentTransform = default;
    [SerializeField] private GameObject PanelLobbyList = default;
    [SerializeField] private GameObject PanelPlayerName = default;
    [SerializeField] private TMP_InputField lobbyNameForm = default;
    [SerializeField] private TMP_InputField playerNameForm = default;
    [SerializeField] private GameObject Canvas = default;
    public static int MAX_PLAYERS = 2;
    public static LobbyManager Instance;
    public Lobby currentLobby = null;
    private string playerName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(playerName)) PanelPlayerName.SetActive(true); // à sauvegarder en player prefs ou easy save
        AuthenticationService.Instance.SignedIn += this.QueryLobbies; //On lance une recherche de lobby qu'au moment ou le joueur est authentifié auprès d'Unity
    }

    public void SetPlayerName()
    {
        if (string.IsNullOrEmpty(playerNameForm.text)) return;
        this.playerName = playerNameForm.text; 
        PanelPlayerName.SetActive(false);
    }

    #region HOSTING LOBBY

    public async void StartLobby()
    {
        string lobbyName = lobbyNameForm.text;
        string joinCode = await RelayManager.Instance.CreateRelayHost(MAX_PLAYERS);
        currentLobby = await CreateLobbyWithHeartbeatAsync(lobbyName, MAX_PLAYERS, playerName, joinCode);
        NetworkLog.LogInfoServer("Hello World from server ! It's " + playerName);
        Canvas.SetActive(false);
    }

    private async Task<Lobby> CreateLobbyWithHeartbeatAsync(string lobbyName, int maxPlayers, string hostPlayerName, string relayJoinCode)
    {

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.Player = new Player(
            id: AuthenticationService.Instance.PlayerId,
            data: new Dictionary<string, PlayerDataObject>()
            {
                {
                    "PlayerName", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Public,
                        value: hostPlayerName)
                }
            });
        options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public,
                        value: relayJoinCode)
                },
                {
                    "HostName", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public,
                        value: hostPlayerName)
                },
            };
        options.IsPrivate = false;

        var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
        return lobby;
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    #endregion HOSTING LOBBY

    #region JOINING LOBBY

    public void RefreshLobbies()
    {
        QueryLobbies();
    }

    private async void QueryLobbies()
    {
        BlockUI.Instance.Block();
        Debug.Log("Query Lobbies...");
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;


            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
    {
        new QueryOrder(
            asc: false,
            field: QueryOrder.FieldOptions.Created)
    };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            for (int i = 0; i < lobbyListContentTransform.childCount; i++)
            {
                Destroy(lobbyListContentTransform.GetChild(i).gameObject);
            }

            lobbies.Results.ForEach(lobby =>
            {
                GameObject newLobbyGO = Instantiate(lobbyCardPFB, lobbyListContentTransform);
                LobbyCard newLobby = newLobbyGO.GetComponent<LobbyCard>();
                newLobby.SetLobbyInfo(lobby, lobby.Name, lobby.Data["HostName"].Value);
            });
            if(lobbies.Results.Count == 0)
            {
                Instantiate(noLobbyPFB, lobbyListContentTransform);
            }
            BlockUI.Instance.UnBlock();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            BlockUI.Instance.UnBlock();
        }
    }

    public async void EnterLobby(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            options.Player = new Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                {
                    "PlayerName", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Public,
                        value: playerName)
                }
                });
#if !UNITY_EDITOR
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            string joinCode = joinedLobby.Data["joinCode"].Value;
            List<Player> players = joinedLobby.Players;
#elif UNITY_EDITOR
            string joinCode = lobby.Data["joinCode"].Value;
            List<Player> players = lobby.Players;
#endif
            Debug.Log("Received code: " + joinCode);
            players.ForEach(player =>
            {
                Debug.Log("Join player " + player.Data["PlayerName"].Value);
            });
            await RelayManager.Instance.StartRelayClient(joinCode);
            Canvas.SetActive(false);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    #endregion JOINING LOBBY
}