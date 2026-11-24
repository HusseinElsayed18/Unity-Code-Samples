using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonMatchmakingService : MonoBehaviourPunCallbacks,IMatchmakingService
{
    private TaskCompletionSource<MatchmakingState> _connectTcs;
    private TaskCompletionSource<MatchmakingState> _roomTcs;
    private TaskCompletionSource<MatchmakingState> _leaveTcs;
    public Task<MatchmakingState> Connect()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return Task.FromResult(MatchmakingState.Connected);

        if (_connectTcs != null)
            return _connectTcs.Task;

        _connectTcs = new TaskCompletionSource<MatchmakingState>();

        PhotonNetwork.ConnectUsingSettings();

        return _connectTcs.Task;
    }
   
    public Task<MatchmakingState> FindRandomMatch()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            DebugX.ChainedRichText.Text("Failed to connect and find a random room ")
                .Color("red")
                .DisplayDialog("Photon Matchmaking Service")
                .LogError();
            return Task.FromResult(MatchmakingState.Disconnected);
        }
        if (_roomTcs != null)
            return _roomTcs.Task;

        _roomTcs = new TaskCompletionSource<MatchmakingState>();
        
        PhotonNetwork.JoinRandomRoom();
        
        return _roomTcs.Task;
    }

    public Task<MatchmakingState> CreateRoom(string roomName, int maxPlayers)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            DebugX.ChainedRichText.Text("Failed to connect and create a room ")
                .Color("red")
                .DisplayDialog("Photon Matchmaking Service")
                .LogError();
            return Task.FromResult(MatchmakingState.Disconnected);
        }
        
        if (_roomTcs != null)
            return _roomTcs.Task;

        _roomTcs = new TaskCompletionSource<MatchmakingState>();

        var options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers
        };

        PhotonNetwork.CreateRoom(roomName, options);
        _roomTcs.TrySetResult(MatchmakingState.CreatingRoom);
        
        return _roomTcs.Task;
    }

    public Task<MatchmakingState> JoinRoom(string roomName)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            DebugX.ChainedRichText.Text($"Failed to connect and joining this room {roomName}")
                .Color("red")
                .DisplayDialog("Photon Matchmaking Service")
                .LogError();
            return Task.FromResult(MatchmakingState.Disconnected);
        }

        if (_roomTcs != null)
            return _roomTcs.Task;

        _roomTcs = new TaskCompletionSource<MatchmakingState>();

        PhotonNetwork.JoinRoom(roomName);
        _roomTcs.TrySetResult(MatchmakingState.JoiningRoom);
        
        return _roomTcs.Task;

    }

    public Task<MatchmakingState> LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
        {
            return Task.FromResult(MatchmakingState.Disconnected);
        }
        
        if (_leaveTcs != null)
            return _leaveTcs.Task;

        _leaveTcs = new TaskCompletionSource<MatchmakingState>();

        PhotonNetwork.LeaveRoom();
        _leaveTcs.TrySetResult(MatchmakingState.LeavingRoom);
        
        return _leaveTcs.Task;
    }

    #region callbacks

    public override void OnConnectedToMaster()
    {
        // Successfully connected
        DebugX.ChainedRichText.Text("Connected to Photon Master")
            .Color("cyan")
            .Log();
        _connectTcs?.TrySetResult(MatchmakingState.Connected);
        _connectTcs = null;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        // Disconnected
        DebugX.ChainedRichText.Text("Disconnected to Photon Master")
            .NewLine()
            .Append(cause.ToString())
            .Color("red")
            .DisplayDialog("Photon Matchmaking Service")
            .LogError();
        
        _connectTcs?.TrySetResult(MatchmakingState.Disconnected);
        _roomTcs?.TrySetResult(MatchmakingState.Disconnected);
        _leaveTcs?.TrySetResult(MatchmakingState.Disconnected);

        _connectTcs = null;
        _roomTcs = null;
        _leaveTcs = null;
    }
    public override void OnCreatedRoom()
    {
        // Successfully room created
        _roomTcs?.TrySetResult(MatchmakingState.CreatedRoom);
        _roomTcs = null;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Room creation failed
        DebugX.ChainedRichText.Text("Room creation failed")
            .Color("red")
            .DisplayDialog("Photon Matchmaking Service")
            .LogError();
        _roomTcs?.TrySetResult(MatchmakingState.FailedToCreateRoom);
        _roomTcs = null;
    }

    public override void OnJoinedRoom()
    {
        // Successfully joined a room
        DebugX.ChainedRichText.Text("Successfully joined the room")
            .Color("cyan")
            .Log();
        _roomTcs?.TrySetResult(MatchmakingState.InRoom);
        _roomTcs = null;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Failed to join a room
        DebugX.ChainedRichText.Text("Failed to join room")
            .Color("red")
            .DisplayDialog("Photon Matchmaking Service")
            .LogError();
        _roomTcs?.TrySetResult(MatchmakingState.FailedToJoinRoom);
        _roomTcs = null;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Failed to join a random room
        DebugX.ChainedRichText.Text($"No random room found: {message}")
            .Color("red")
            .DisplayDialog("Photon Matchmaking Service")
            .LogError();
        _roomTcs?.TrySetResult(MatchmakingState.NoRoomAvailable);
        _roomTcs = null;
    }
    public override void OnLeftRoom()
    {
        // Successfully left the room
        DebugX.ChainedRichText.Text("Successfully left the room")
            .Color("cyan")
            .Log();
        _leaveTcs?.TrySetResult(MatchmakingState.LeftRoom);
        _leaveTcs = null;
    }

   
    

    #endregion
}