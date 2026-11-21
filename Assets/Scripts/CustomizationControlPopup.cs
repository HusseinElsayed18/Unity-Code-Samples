using System;
using EGYGames.Constants.CustomizationControls;
using EGYGames.Core.Exrensions.Attributes;
using EGYGames.Core.Exrensions.Components.UI;
using EGYGames.Core.Services.Controls;
using EGYGames.Core.UI.Components;
using UnityEngine;
using Zenject;

// The actual customization menu.
//
// +--------------------------------+
// |      CUSTOMIZE CONTROLS        |
// |                                |
// |   [ FIRE ]       [ JUMP ]      |
// |                                |
// |        ( JOYSTICK )             |
// |                                |
// | [Preset] [Reset] [Save] [Back] |
// +--------------------------------+
//


public class CustomizationControlPopup : UIPopup  
{
    [SerializeField] private GameButton closeButton;
    [SerializeField] private GameButton backButton;
    
    [SerializeField] private GameSlider elementSizeSlider;
    [SerializeField] private GameSlider elementTransparencySlider;
    [SerializeField,ReadOnly] CustomizableControl _selectedControl;
    
    [SerializeField] private GameButton saveButton;
    [SerializeField] private GameButton resetButton;
    
    [Header("movement arrows")]
    [SerializeField,Range(2f,10f)] float movementStep = 5f;
    [SerializeField] GameButton upArrow, downArrow, leftArrow, rightArrow;
    
    [Header("screen preset button")]
    [SerializeField] ButtonPopup screenPresetButtonPopup;
    [SerializeField] CustomizableControlsContainer screenPresetsContainer; 
    [SerializeField,ReadOnly] ScreenPresets screenPreset = ScreenPresets.MainMenu;
    
    [Header("Layout preset button")]
    [SerializeField] ButtonPopup layoutPresetButtonPopup;
    [SerializeField,ReadOnly] LayoutPresets layoutPreset = LayoutPresets.DefaultLayout;

    private IControlLayoutService _layoutService;
    private MessageManager   _messageManager;
    
    [Inject]
    public void Construct(IControlLayoutService service , MessageManager manager) 
    {
        _layoutService = service;
        _messageManager = manager;
    }
    
    private void ShowSaveMessage()
    {
        _messageManager.Show<SimpleMessage>(new SimpleMessageData<PopupCustomizationControlMessageKey>(PopupCustomizationControlMessageKey.SaveLayout) );
        _messageManager.SetOKButtonAction<SimpleMessage>( LocalizedButtonState.Save , SaveAndExit );
    }
    private void ShowResetMessage()
    {
        _messageManager.Show<SimpleMessage>(new SimpleMessageData<PopupCustomizationControlMessageKey>(PopupCustomizationControlMessageKey.ResetLayout) );
        _messageManager.SetOKButtonAction<SimpleMessage>( LocalizedButtonState.Reset , ResetLayout );
    }
    private void SaveAndExit()
    {
        _layoutService.SaveLayout();
        _messageManager.CloseCurrent();
    }

    private void ResetLayout()
    {
        _layoutService.ResetLayout();
        _messageManager.CloseCurrent();
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (_layoutService == null || _messageManager == null)
        {
            DebugX.ChainedRichText.Text("Missing some services")
                .Color("red")
                .Log();
            return;
        }

        InitScreenPresetButtonPopup();
        InitLayout();
    }

    #region Editor

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (closeButton == null || backButton == null || 
            elementSizeSlider == null || elementTransparencySlider == null
            ||saveButton == null || resetButton == null
            || upArrow == null || downArrow == null || leftArrow == null || rightArrow == null
            || screenPresetButtonPopup == null || screenPresetsContainer == null
            || layoutPresetButtonPopup == null)
        {
            DebugX.ChainedRichText.Text("Missing some Fields , check your inspector")
                .Color("red")
                .Log();
        }
    }
#endif
    
    #endregion

    private void OnEnable()
    { 
        if(_layoutService != null)
          _layoutService.OnSelectedControlChanged += SelectControl;
        screenPresetButtonPopup.OnItemClickedEventInt += OnScreenPresetButtonPopupChanged;
        layoutPresetButtonPopup.OnItemClickedEventInt += OnLayoutPresetButtonPopupChanged;
        
        closeButton.onClick.AddListener(OnCloseClicked);
        backButton.onClick.AddListener(OnBackClicked);
        saveButton.onClick.AddListener(ShowSaveMessage);
        resetButton.onClick.AddListener(ShowResetMessage);
        elementSizeSlider.onValueChanged.AddListener(OnElementSizeChanged);
        elementTransparencySlider.onValueChanged.AddListener(OnElementTransparencyChanged);
        upArrow.onClick.AddListener(MoveUp);
        downArrow.onClick.AddListener(MoveDown);
        leftArrow.onClick.AddListener(MoveLeft);
        rightArrow.onClick.AddListener(MoveRight);
        _layoutService?.SetEditMode(true);
    }
    private void OnDisable()
    {
        if(_layoutService != null)
            _layoutService.OnSelectedControlChanged -= SelectControl;
        screenPresetButtonPopup.OnItemClickedEventInt -= OnScreenPresetButtonPopupChanged;
        layoutPresetButtonPopup.OnItemClickedEventInt -= OnLayoutPresetButtonPopupChanged;
        
        closeButton.onClick.RemoveListener(OnCloseClicked);
        backButton.onClick.RemoveListener(OnBackClicked);
        saveButton.onClick.RemoveListener(ShowSaveMessage);
        resetButton.onClick.RemoveListener(ShowResetMessage);
        elementSizeSlider.onValueChanged.RemoveListener(OnElementSizeChanged);
        elementTransparencySlider.onValueChanged.RemoveListener(OnElementTransparencyChanged);
        upArrow.onClick.RemoveListener(MoveUp);
        downArrow.onClick.RemoveListener(MoveDown);
        leftArrow.onClick.RemoveListener(MoveLeft);
        rightArrow.onClick.RemoveListener(MoveRight);
        _layoutService?.SetEditMode(false);
    }
   
    public void SelectControl(CustomizableControl control) 
    {
        if (control == null)
            return;
        //on select control element
        //change slider value to == element scale
        _selectedControl = control;
        elementSizeSlider.value = _selectedControl.Rect.localScale.x;
        elementTransparencySlider.value = _selectedControl.CanvasGroup.alpha;
    }
    public void OnElementSizeChanged(float value)  
    {
        //on change slider
        //change element scale
        _selectedControl?.SetScale(value);
    }
    public void OnElementTransparencyChanged(float value) 
    {
        //on change slider
        //change element opacity/transparency
        _selectedControl?.SetOpacity(value);
    }

    #region movement arrows
  
    private void MoveSelectedControl(Vector2 direction)
    {
        if (_selectedControl == null)
        {
            return;
        }
        
        _selectedControl.Rect.anchoredPosition += direction * movementStep;
    }

    private void MoveUp() => MoveSelectedControl(Vector2.up);
    private void MoveDown() => MoveSelectedControl(Vector2.down);
    private void MoveLeft() => MoveSelectedControl(Vector2.left);
    private void MoveRight() => MoveSelectedControl(Vector2.right);
    
    #endregion

    #region Screen preset button
   
    private void OnScreenPresetButtonPopupChanged(int index)
    {
        
        if (!Enum.IsDefined(typeof(ScreenPresets), index))
        {
            DebugX.ChainedRichText.Text($"Invalid screen preset index: {index}")
                .Color("red")
                .Log();
            return;
        }

        screenPresetsContainer.EnableScreenPreset((int)screenPreset, false);
        screenPreset = (ScreenPresets) index;
        screenPresetsContainer.EnableScreenPreset(index, true);
    }

    private void InitScreenPresetButtonPopup()
    {
        //make first item selected when pop up shown
        screenPresetButtonPopup.RaiseOnItemClickedEvent(0);
    }
    #endregion
    
    #region Layout preset button
 
    private void OnLayoutPresetButtonPopupChanged(int index)
    {  
        if (!Enum.IsDefined(typeof(LayoutPresets), index))
        {
            DebugX.ChainedRichText.Text($"Invalid layout preset index: {index}")
                .Color("red")
                .Log();
            return;
        }
        
        layoutPreset = (LayoutPresets) index;
        _layoutService.CurrentLayoutName = layoutPreset.ToString();
        _layoutService.ApplySavedLayoutForAllControls();
    }

    private void InitLayout()
    {
        string savedLayout = PlayerPrefs.GetString(PlayerPrefsControlsKeys.CurrentLayoutKey, nameof(LayoutPresets.DefaultLayout));

        if (!Enum.TryParse(savedLayout, out layoutPreset))
            layoutPreset = LayoutPresets.DefaultLayout;
        
        _layoutService.CurrentLayoutName = layoutPreset.ToString();
        layoutPresetButtonPopup.RaiseOnItemClickedEvent((int)layoutPreset);
    }
    #endregion
    private void OnCloseClicked()
    {
        popupManager.CloseCurrent();
    }
    private void OnBackClicked()
    {
        popupManager.Show<SettingsPopup>();
    }
}
