using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSetupData gameSetupData;
    [SerializeField] private GameThemeConfigData gameThemeConfigData;
    public override void InstallBindings()
    {
        #region Signals

        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<GameSetupDataSignal>();
        
        Container.BindInterfacesAndSelfTo<GameSetupLocalizedTMP>()
            .FromComponentsInHierarchy()
            .AsSingle();

        #endregion
        
        
        // Data / services
        
        //game setup data
        Container.BindInstance(gameSetupData).AsSingle();

        #region settings
        
        Container.Bind<ISettingsStorage>()
            .To<PlayerPrefsSettingsStorage>()
            .AsSingle();

        Container.Bind<ISettingsService>()
            .To<SettingsService>()
            .AsSingle();

        #region Theme
        // Container.Bind<GameThemeConfigData>()
        //     .FromInstance(gameThemeConfigData)
        //     .AsSingle();
        
        Container.Bind<IThemeService>()
            .To<ThemeService>()
            .AsSingle()
            .WithArguments(gameThemeConfigData);
        #endregion

        #region Vibration Service

        Container.Bind<IVibrationService>()
            .To<VibrationService>()
            .AsSingle();

        #endregion
        
        #region Graphics Service

        Container.Bind<IGraphicsService>()
            .To<GraphicsService>()
            .AsSingle()
            .NonLazy();

        #endregion
        
        #endregion
        
        Container.Bind<ProfileLocalStorageService>()
            .AsSingle()
            .NonLazy();
        
        Container.Bind<ICooldownService>()
            .To<CooldownService>()
            .AsSingle();
        
        Container.Bind<LocalizationService>()
            .AsSingle()
            .NonLazy();
        Container.Bind<ProfileService>()
            .AsSingle()
            .NonLazy();
        Container.Bind<FriendsService>()
            .AsSingle()
            .NonLazy();
        Container.Bind<AvatarService>()
            .AsSingle()
            .NonLazy();
     
        //Auth Initializer services 

        #region Google 

            //  google
            Container.Bind<IServiceInitializer>()
                .To<GoogleInitializer>()
                .AsSingle();
            Container.Bind<IAuthProvider>()
                .WithId("GoogleAuthProvider")
                .To<GoogleAuthProvider>()
                .AsSingle();
        #endregion
    

         #region play game or game center

                #if UNITY_ANDROID
                         //play games
                         Container.Bind<IPlatformAuthInitializer>()
                             .To<AndroidPlatformAuthInitializer>()
                             .AsSingle();
                         Container.Bind<IAuthProvider>()
                             .To<PlayGamesAuthProvider>()
                             .AsSingle();
                #elif UNITY_IOS
                //Game Center
                           Container.Bind<IPlatformAuthInitializer>()
                            .To<IOSPlatformAuthInitializer>()
                            .AsSingle();
                          Container.Bind<IAuthProvider>()
                            .To<GameCenterAuthProvider>()
                            .AsSingle();
                #endif
                         //play games or game center
                         Container.Bind<IServiceInitializer>()
                             .To<PlatformAuthInitializer>()
                             .AsSingle();

         #endregion

         if (gameSetupData.gameAuthBackend == GameAuthBackend.Firebase)
         {
             #region Firebase FirebaseInitializer
             //Firebase 
             Container.Bind<IServiceInitializer>()
                 .To<FirebaseInitializer>()
                 .AsSingle();
             #endregion
         }
         else if (gameSetupData.gameAuthBackend == GameAuthBackend.UGS)
         {
             #region UGS UGSInitializer

             //UGS
             Container.Bind<IServiceInitializer>()
                 .To<UGSInitializer>()
                 .AsSingle();
        
             #endregion
         }
         
   
        // Initialization Coordinator
        Container.Bind<InitializationCoordinator>()
            .AsSingle();
        
        if (gameSetupData.gameAuthBackend == GameAuthBackend.Firebase)
        {
            #region Firebase FirebaseAuthService
            //Firebase   // after InitializationCoordinator(injected inside it)
            Container.Bind<IAuthenticationService>()
                .To<FirebaseAuthService>()
                .AsSingle();
            #endregion
        }
        else if (gameSetupData.gameAuthBackend == GameAuthBackend.UGS)
        {
            #region UGS Service
            //UGS
            Container.Bind<IAuthenticationService>()
                .To<UGSAuthService>()
                .AsSingle();
            #endregion
        }
        
        
        // Profile UI
        Container.Bind<ProfileSideMenu>()
            .FromComponentInHierarchy()
            .AsSingle();
        Container.Bind<ProfileScreen>()
            .FromComponentInHierarchy()
            .AsSingle();
        Container.Bind<SmallProfileUI>()
            .FromComponentInHierarchy()
            .AsSingle();
        Container.Bind<ProfilePageController>()
            .FromComponentInHierarchy()
            .AsSingle();
        //UI popup
        Container.Bind<PopupManager>()
            .FromComponentInHierarchy()
            .AsSingle();
        //UI messages
        Container.Bind<MessageManager>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
    
    public override async void Start()
    {
        var _initializationCoordinator = Container.Resolve<InitializationCoordinator>();
        
        try
        {
            await _initializationCoordinator.InitializeAsync();
         
            Debug.Log("App is ready.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Application initialization failed: {ex}");
        }
        
        /// 
        Container.Resolve<SignalBus>().Fire<GameSetupDataSignal>(new GameSetupDataSignal()
        {
            GameAuthBackend = gameSetupData.gameAuthBackend,
        });
    }
}