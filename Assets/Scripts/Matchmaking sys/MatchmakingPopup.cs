using System;
using System.Threading.Tasks;
using EGYGames.Core.Extensions.Components.UI;
using TMPro;
using UnityEngine;
using Zenject;

public class MatchmakingPopup : UIPopup
{
    private IMatchmakingService _matchmakingService;
    private MatchmakingState _matchmakingState = MatchmakingState.Disconnected;
    private bool _isBusy;
    
    [Header("Input")]
    [SerializeField] private TMP_InputField roomNameInput;
    
    [Header("Max Players")]
    [SerializeField] private GameSlider maxPlayersSlider;
    
    [SerializeField] private LocalizedTMPText statusText;

    [Header("Buttons")]
    [SerializeField] private GameButton createRoomButton;
    [SerializeField] private GameButton joinRoomButton;
    

    [Inject]
    public void Construct(IMatchmakingService service)
    {
        _matchmakingService = service;
    }

    private void Start()
    {
        SetStatus("Disconnected");
        
        maxPlayersSlider.minValue = MatchmakingConstants.MIN_PLAYERS;
        maxPlayersSlider.maxValue = MatchmakingConstants.MAX_PLAYERS;
        maxPlayersSlider.wholeNumbers = true;

        maxPlayersSlider.value = MatchmakingConstants.MIN_PLAYERS;
    }

    private void OnEnable()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
    }

    private void OnDisable()
    {
        createRoomButton.onClick.RemoveListener(OnCreateRoomClicked);
        joinRoomButton.onClick.RemoveListener(OnJoinRoomClicked);
    }
    
    private void SetStatus(string message)
    {
        statusText.SetLocalizedString(message);
    }

    public async void OnCreateRoomClicked()
    {
        if (_isBusy) return;
        
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            SetStatus("EnterNameOrRoomName");
            return;
        }
        
        _isBusy = true;

        try
        {
            MatchmakingState connection = await _matchmakingService.Connect();

            if (connection != MatchmakingState.Connected)
            {
                DebugX.ChainedRichText.Text("Disconnected")
                    .Color("red")
                    .LogError();
                return;
            }

            MatchmakingState result = await _matchmakingService.CreateRoom(roomNameInput.text, (int) maxPlayersSlider.value);

            if (result == MatchmakingState.CreatedRoom)
            {
                DebugX.ChainedRichText.Text("Successfully room created")
                    .Color("cyan")
                    .Log();
            }
        }
        catch (Exception e)
        {
           // Debug.LogException(e);
            DebugX.ChainedRichText.Text("Failed to create room")
                .Append(e.Message)
                .Color("red")
                .LogError();
        }
        finally
        {
            _isBusy = false;
        }
    }

    public async void OnJoinRoomClicked()
    {
        if (_isBusy) return;
        
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            SetStatus("EnterNameOrRoomName");
            return;
        }
        
        _isBusy = true;

        try
        {
            MatchmakingState connection = await _matchmakingService.Connect();

            if (connection != MatchmakingState.Connected)
            {
                DebugX.ChainedRichText.Text("Disconnected")
                    .Color("red")
                    .LogError();
                return;
            }

            MatchmakingState result = await _matchmakingService.JoinRoom(roomNameInput.text);
            
            if (result == MatchmakingState.InRoom)
            {
                DebugX.ChainedRichText.Text("Joined room successfully!")
                    .Color("cyan")
                    .Log();
            }
        }
        catch (Exception e)
        {
            DebugX.ChainedRichText.Text("Failed to join room")
                .Append(e.Message)
                .Color("red")
                .LogError();
        }
        finally
        {
            _isBusy = false;
        }
    }

    public async void OnLeaveRoomClicked()
    {
        if (_isBusy) return;

        _isBusy = true;

        try
        {
            MatchmakingState result = await _matchmakingService.LeaveRoom();

            if (result == MatchmakingState.LeftRoom)
            {
                DebugX.ChainedRichText.Text("Left room successfully!")
                    .Color("cyan")
                    .Log();
            }
        }
        catch (Exception e)
        {
           
            DebugX.ChainedRichText.Text("Failed to leave room")
                .Append(e.Message)
                .Color("red")
                .LogError();
        }
        finally
        {
            _isBusy = false;
        }
    }
}