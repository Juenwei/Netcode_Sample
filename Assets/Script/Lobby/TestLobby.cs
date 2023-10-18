using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    private const float LOBBY_TIMER_MAX = 15f, LOBBY_POLL_TIMER_MAX = 1.1f;

    private Lobby hostLobby, joinedLobby;
    private float lobbyHeartBeatTimer = 0f, lobbyPollTimer = 0f;
    private string playerName;

	private void Awake()
	{
		playerName = "JuenWei" + UnityEngine.Random.Range(10, 99);
	}

	private async void Start()
    {
        await Authenticate();
	}

	private void Update()
	{
        HandleLobbyHeartBeat();
        HandleLobbyPollForUpdate();
	}

		//Use async await to prevent application freeze while estarblish connection to server. 
    private async Task Authenticate()
    {
        //Create a profile based on the different name for ensure different UserId for each build
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

		await UnityServices.InitializeAsync(initializationOptions);

		//Listen to the sign in event
		AuthenticationService.Instance.SignedIn += () =>
		{
			Debug.Log("Signed in : " + AuthenticationService.Instance.PlayerId + " ,Current PLayer Name : " + playerName);
		};

		//Since Lobby need a account to connect
		//Create a new account for anoymous to sign in 
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
	}
    
	public async void CreateLobbyAsync()
    {
        try {
			string lobbyName = "JuenWeiLobby";
			int lobbyMaxPeople = 4;

            var createLobbyOptions = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = GetPlayer(),
                //Custome Data
                Data = new Dictionary<string, DataObject>()
                {
                    {"GameMode",new DataObject(DataObject.VisibilityOptions.Public,"CaptureTheFlag", DataObject.IndexOptions.S1) },
                    { "Map",new DataObject(DataObject.VisibilityOptions.Public, "de_dust II",DataObject.IndexOptions.S2)}
                }
			};

			var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbyMaxPeople,createLobbyOptions);

			//Register the lobby to the refresh heartbeat function
			hostLobby = lobby;
            joinedLobby = hostLobby;
			Debug.Log("Created Lobby : " + lobby.Name + " with maximum " + lobby.MaxPlayers + " players" + " ID : " + lobby.Id + " LobbyCode : " + lobby.LobbyCode);
            PrintLobbyPlayer(hostLobby);
		}
        catch (LobbyServiceException e){
            Debug.LogError(e);
        }
    }
    
    //A function make the lobby keep update after certian amount of time to prevent lobby get destroy
    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            lobbyHeartBeatTimer -= Time.deltaTime;
            if(lobbyHeartBeatTimer < 0f)
            {
                //Send Heart Beats
                lobbyHeartBeatTimer = LOBBY_TIMER_MAX;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdate()
    {
        if(joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                lobbyPollTimer = LOBBY_POLL_TIMER_MAX;

                //Retrive Lobby data from the host for updates
                var lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    public async void QueryLobbyAsync()
    {
        try {
            QueryLobbiesOptions queryLobbyOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    //Lobby will be sort based on the create time from oldest to latest.
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                },
            };

			var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbyOptions);

			Debug.Log(queryResponse.Results.Count + " record(s) founded");
			foreach (var lobby in queryResponse.Results)
			{
				Debug.Log("Lobby Name : " + lobby.Name + " , Max People : " + lobby.MaxPlayers + " , Game Mode : " + lobby.Data["GameMode"].Value);
			}

		}
		catch (LobbyServiceException e) { 
            Debug.LogError(e);
        }
       
    }

    public async void JoinLobbyAsync(string lobbyCode)
    {
        try
        {
            var joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
			joinedLobby = lobby;


			Debug.Log("Joined Lobby with code : " + lobbyCode);
            PrintLobbyPlayer(lobby);
		}
		catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
		
	}

    public async void QuickJoinLobbyAsync()
    {
        try
        {
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
			joinedLobby = lobby;


			Debug.Log("Quick Joined to the lobby");
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            //Only Host can change Game Mode
            if(hostLobby == null) { return; }

            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public,"Deathmatch",DataObject.IndexOptions.S1)}
                }
            });
            Debug.Log("Updated Game Mode");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError (e);
        }
    }

    public async void UpdatePlayer(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions()
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}
                }
            });

		}
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

	public async void KickPlayer()
	{
		try
		{
            //KIck out the second player since first is the host
			await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
		}
		catch (LobbyServiceException e)
		{
			Debug.LogError(e);
		}
	}

    public async void MigrateHostPermission()
    {
		try
		{
			hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions()
			{
				HostId = joinedLobby.Players[1].Id
			});
		}
		catch (LobbyServiceException e)
		{
			Debug.LogError(e);
		}
	}

    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void PrintLobbyMessage()
    {
        PrintLobbyPlayer(joinedLobby);
    }

	private void PrintLobbyPlayer(Lobby lobby)
    {
        Debug.Log("Players in Lobby : " + lobby.Name + "Game Mode : " + lobby.Data["GameMode"].Value + "Map : " + lobby.Data["Map"].Value);
        foreach(var player in lobby.Players)
        {
            Debug.Log("Player ID : " + player.Id + " PlayerName : " + player.Data["PlayerName"].Value);

		}
    }

    public Player GetPlayer()
    {
        return new Player()
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }
}
