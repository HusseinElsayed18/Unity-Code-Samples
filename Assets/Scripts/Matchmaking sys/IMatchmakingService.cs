using System.Threading.Tasks;

public interface IMatchmakingService
{
    Task<MatchmakingState> Connect();
    Task<MatchmakingState> FindRandomMatch();
    Task<MatchmakingState> CreateRoom(string roomName, int maxPlayers);

    Task<MatchmakingState> JoinRoom(string roomName);

    Task<MatchmakingState> LeaveRoom();
}
