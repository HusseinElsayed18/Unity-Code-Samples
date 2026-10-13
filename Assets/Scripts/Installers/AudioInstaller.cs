using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class AudioInstaller : MonoInstaller
{
    [Header("Audio Database")]
    [SerializeField] private AudioDatabase _database;
   [Header(("audio mixer"))]
   [SerializeField] private AudioMixer audioMixer;
   
   [Header("Sfx /Music /.. Coroutine Runner ")]
   // empty MonoBehaviour to call Coroutine , we use it for every coroutine needed at any one
   [SerializeField] private MonoBehaviour _coroutineRunner;
   
   [Header(("Sfx pool"))]
   [SerializeField] private AudioSource _sfxPrefab;
   [SerializeField]  private Transform _sfxRoot;
   [Range(4,20)]
   [SerializeField]  private int _sfxInitialPoolSize;
   
   [Header("Music Service")]
   [SerializeField] private AudioSource _musicAudioSource;
   [SerializeField] private AudioSource  _musicAudioSource2;
   [Range(1f,4f)]
   [SerializeField] private float _musicFadeDuration;
   
   [Header(("Ui pool"))]
   [SerializeField] private AudioSource _uiPrefab;
   [SerializeField]  private Transform _uiRoot;
   [Range(4,20)]
   [SerializeField]  private int _uiInitialPoolSize;
   
   public override void InstallBindings()
   {
       //data
       Container.Bind<AudioMixer>()
           .FromInstance(audioMixer)
           .AsSingle();
       
       Container.Bind<AudioDatabase>()
           .FromInstance(_database)
           .AsSingle();

       #region Sfx Pool

       Container.Bind<AudioSource>()
           .FromComponentInNewPrefab(_sfxPrefab)
           .AsTransient();
       
       Container.Bind<Transform>()
           .FromInstance(_sfxRoot)
           .AsTransient();

       Container.Bind<AudioPool>()
           .WithId("Sfx_Pool")
           .AsCached()
           .WithArguments(_sfxPrefab,_sfxRoot,_sfxInitialPoolSize,"Sfx");
       
       Container.Bind<IInitializable>()
           .To<AudioPool>()
           .FromMethod(ctx => ctx.Container.ResolveId<AudioPool>("Sfx_Pool"));
       #endregion
       
       #region UI Pool >> use SfxPool

       Container.Bind<AudioSource>()
           .FromComponentInNewPrefab(_uiPrefab)
           .AsTransient();
       
       Container.Bind<Transform>()
           .FromInstance(_uiRoot)
           .AsTransient();
       
       Container.Bind<AudioPool>()
           .WithId("Ui_Pool")
           .AsCached()
           .WithArguments(_uiPrefab,_uiRoot,_uiInitialPoolSize,"Ui");
       
       
       Container.Bind<IInitializable>()
           .To<AudioPool>()
           .FromMethod(ctx => ctx.Container.ResolveId<AudioPool>("Ui_Pool"));
         
       #endregion
     
       
       //services

       Container.Bind<IMusicService>()
           .To<MusicService>()
           .AsSingle()
           .WithArguments(_musicAudioSource, _musicAudioSource2, _coroutineRunner, _musicFadeDuration)
           .NonLazy();

       Container.Bind<ISfxService>()
           .To<SfxService>()
           .AsSingle()
           .WithArguments(_coroutineRunner);
       
       Container.Bind<IUiAudioService>()
           .To<UiAudioService>()
           .AsSingle()
           .WithArguments(_coroutineRunner);
       
       Container.Bind<IVoiceChatAudioService>()
           .To<VoiceChatAudioService>()
           .AsSingle();
       
       Container.Bind<IAmbientService>()
           .To<AmbientService>()
           .AsSingle();
       
       Container.Bind<IAudioService>()
           .To<AudioService>()
           .AsSingle();
      
   }
}
