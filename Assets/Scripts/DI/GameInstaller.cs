using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        Container.Bind<PlayerInputManager>().FromComponentInHierarchy() 
            .AsSingle();
        Container.Bind<PlayerManager>().FromComponentInHierarchy() 
            .AsSingle();
        Container.Bind<PlayerLocomotionManager>()
            .FromComponentInHierarchy() 
            .AsSingle();
        Container.Bind<PlayerAnimationManager>()
            .FromComponentInHierarchy() 
            .AsSingle();
        Container.Bind<PlayerCamera>()
            .FromComponentInHierarchy() 
            .AsSingle();
        Container.Bind<ParticlePoolManager>().FromComponentInHierarchy()
            .AsSingle();
        Container.Bind<BlockSpawner>().FromComponentInHierarchy()
            .AsSingle();
        
    }
}