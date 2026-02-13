

namespace AsteroidsGame.CompositionRoot
{
    using AsteroidsGame.Contracts;
    using AsteroidsGame.Logic;
    using AsteroidsGame.Presentation;
    using UnityEngine;
    public class CompositionRoot : MonoBehaviour
    {
        private IEcsBootstrap _bootstrap;
        private IEcsRunner _runner;
        private IViewSync _viewSync;

        [SerializeField] private GlobalConfigService _globalConfigService;
        [SerializeField] private UnityViewUpdater _viewUpdater;

        void Start()
        {
            IDeltaTimeControllerService deltaTimeService = new DeltaTimeService();
            IInputControllerService inputService = new InputService();
            
            IInputReader inputReader = new UnityInputReader();

            _bootstrap = new EcsBootstrap(deltaTimeService, _globalConfigService, inputService);
            _bootstrap.Init();
            
            _runner = new EcsRunner(_bootstrap.Systems, deltaTimeService, inputReader, inputService);
            _viewSync = new EcsViewSync(_bootstrap.World, _viewUpdater);

        }

        void Update()
        {
            _runner.Tick(Time.deltaTime);
            _viewSync.SyncView();
        }

        void OnDestroy()
        {
            _bootstrap.Destroy();
        }
    }
}