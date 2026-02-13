

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
        private float _cachedW, _cachedH;
        private Camera _camera;
        private const float Epsilon = 0.01f;
        
        void Start()
        {
            _bootstrap = new EcsBootstrap(_globalConfigService);
            _bootstrap.Init();
            
            IInputReader inputReader = new UnityInputReader();
            _runner = new EcsRunner(_bootstrap.Systems, inputReader);
            
            _viewSync = new EcsViewSync(_bootstrap.World, _viewUpdater);

            _camera = Camera.main;
        }

        void Update()
        {
            TryUpdateScreenSize();
            
            _runner.Tick(Time.deltaTime);
            _viewSync.SyncView();
        }
        

        void TryUpdateScreenSize()
        {
            var h = 2f * _camera.orthographicSize;
            var w = h * _camera.aspect;
            
            if (Mathf.Abs(w - _cachedW) < Epsilon && Mathf.Abs(h - _cachedH) < Epsilon)
                return;
            
            _cachedW = w; _cachedH = h;
            
            _runner.UpdateScreenSize(w, h);
        }
        void OnDestroy()
        {
            _bootstrap.Destroy();
        }
    }
}