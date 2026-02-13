using UnityEngine;
using System.Collections.Generic;
using AsteroidsGame.Contracts;
using AsteroidsGame.Logic;
using AsteroidsGame.Presentation;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using VContainer;

namespace AsteroidsGame.Bootstrap
{
    public class CompositionRoot : MonoBehaviour
    {
        private ProtoWorld _world;
        private IProtoSystems _systems;
        private ProtoIt _viewIterator;
        private readonly List<ViewData> _viewsBuffer = new();

        [Inject] private IInputReader _inputReader;
        [Inject] private IViewUpdater _viewUpdater;
        private PlayerInputSystem _playerInputSystem;
        private MovementSystem _movementSystem;
        private RotationSystem _rotationSystem;
        private AsteroidSpawnSystem _asteroidSpawnSystem;

        private PositionAspect _positionAspect;
        private EntityAspect _entityAspect;
        private RootAspect _rootAspect;

        private UnityDeltaTimeService _deltaTimeService;
        private SequentialIdGeneratorService _idGeneratorService;
        [SerializeField] private GlobalConfigService _configService;

        private void Awake()
        {
            // aspects
            _rootAspect = new RootAspect();
            _world = new ProtoWorld(_rootAspect);

            _positionAspect = _rootAspect.PositionAspect;
            _entityAspect = _rootAspect.EntityAspect;


            // systems
            _systems = new ProtoSystems(_world);
            _playerInputSystem = new PlayerInputSystem();
            _movementSystem = new MovementSystem();
            _rotationSystem = new RotationSystem();
            _asteroidSpawnSystem = new AsteroidSpawnSystem();
            var playerSpawnSystem = new PlayerSpawnSystem();

            // iterators
            _viewIterator = new ProtoIt(new[]
                { typeof(EntityIdCmp), typeof(PositionCmp), typeof(RotationCmp) });
            _viewIterator.Init(_world);

            // services
            _idGeneratorService = new SequentialIdGeneratorService();
            _deltaTimeService = new UnityDeltaTimeService();

            // init
            _systems
                .AddService(_idGeneratorService, typeof(IIdGeneratorService))
                .AddService(_configService, typeof(IConfigService))
                .AddService(_deltaTimeService, typeof(IDeltaTimeService))
                .AddSystem(playerSpawnSystem)
                .AddSystem(_asteroidSpawnSystem)
                .AddSystem(_playerInputSystem)
                .AddSystem(_rotationSystem)
                .AddSystem(_movementSystem)
                .Init();
        }

        private void Update()
        {
            _deltaTimeService.SetDeltaTime(Time.deltaTime);
            var input = _inputReader.ReadInput();
            _playerInputSystem.SetInput(input);

            _systems.Run();

            SyncView();
        }

        private void SyncView()
        {
            _viewsBuffer.Clear();

            foreach (var e in _viewIterator)
            {
                ref var idComp = ref _entityAspect.EntityIdPool.Get(e);
                ref var p = ref _positionAspect.PositionPool.Get(e);
                ref var rot = ref _positionAspect.RotationPool.Get(e);
                _viewsBuffer.Add(new ViewData
                {
                    id = idComp.id,
                    x = p.x,
                    y = p.y,
                    angle = rot.angle,
                    type = idComp.type
                });
            }

            _viewUpdater.Apply(_viewsBuffer);
        }

        private void OnDestroy()
        {
            _systems.Destroy();
            _world.Destroy();
        }
    }
}