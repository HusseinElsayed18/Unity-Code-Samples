using UnityEngine;
using Zenject;

public class AdsInstaller : MonoInstaller
{
   [SerializeField] private AdsConfigData adsConfigData;
   [SerializeField] private LevelPlayAdsConfigData levelPlayAdsConfigData;
   
   public override void InstallBindings()
   {
      Container.BindInstance(adsConfigData).AsSingle();
      Container.BindInstance(levelPlayAdsConfigData).AsSingle();

      Container.Bind<IAdsService>()
         .WithId("UnityAds")
         .To<AdsService>()
         .AsSingle()
         .WithArguments(adsConfigData);
      
      Container.Bind<IAdsService>()
         .WithId("LevelPlayAds")
         .To<LevelPlayAdsService>()
         .AsSingle()
         .WithArguments(levelPlayAdsConfigData);

      Container.Bind<IGameAdsService>()
         .To<GameAdsService>()
         .AsSingle();
   }
}
