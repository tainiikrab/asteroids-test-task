using AsteroidsGame.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private UnityInputReader _inputReader;
    [SerializeField] private UnityViewUpdater _viewUpdater;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_inputReader).As<IInputReader>();
        builder.RegisterInstance(_viewUpdater).As<IViewUpdater>();

        // builder.RegisterComponentInHierarchy<UnityInputReader>()
        //     .As<IInputReader>();
        // builder.RegisterComponentInHierarchy<UnityViewUpdater>()
        //     .As<IViewUpdater>();
    }
}