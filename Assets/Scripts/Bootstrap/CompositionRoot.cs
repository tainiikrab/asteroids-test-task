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

        private int nextEntityId = 1;

        private PositionAspect _positionAspect;
        private EntityAspect _entityAspect;
        private RootAspect _rootAspect;

        private void Awake()
        {
            _rootAspect = new RootAspect();

            _world = new ProtoWorld(_rootAspect);
            _positionAspect = _rootAspect.PositionAspect;
            _entityAspect = _rootAspect.EntityAspect;

            _systems = new ProtoSystems(_world);

            _viewIterator = new ProtoIt(new[]
                { typeof(EntityIdComponent), typeof(PositionData), typeof(RotationData) });
            _viewIterator.Init(_world);

            _playerInputSystem = new PlayerInputSystem();
            _movementSystem = new MovementSystem();
            _rotationSystem = new RotationSystem();

            _systems
                .AddSystem(_playerInputSystem)
                .AddSystem(_rotationSystem)
                .AddSystem(_movementSystem)
                .Init();
        }

        private void Start()
        {
            var entity = CreateEntity();
            ref var player = ref _entityAspect.PlayerPool.Add(entity);
        }


        private ProtoEntity CreateEntity()
        {
            ref var pos = ref _positionAspect.PositionPool.NewEntity(out var entity);
            pos.x = 0;
            pos.y = 0;

            ref var vel = ref _positionAspect.VelocityPool.Add(entity);
            vel.vx = 0;
            vel.vy = 0;

            ref var rot = ref _positionAspect.RotationPool.Add(entity);
            rot.angle = 0f;

            ref var ang = ref _positionAspect.AngularVelocityPool.Add(entity);
            ang.omega = 0f;

            ref var idComp = ref _entityAspect.EntityIdPool.Add(entity);
            var packed = _world.PackEntity(entity);
            idComp.Id = nextEntityId++;
            idComp.Packed = packed;

            return entity;
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var input = _inputReader.ReadInput();
            _playerInputSystem.SetInput(input);

            _playerInputSystem.DeltaTime = dt;
            _movementSystem.DeltaTime = dt;
            _rotationSystem.DeltaTime = dt;

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
                    id = idComp.Id,
                    x = p.x,
                    y = p.y,
                    angle = rot.angle,
                    type = idComp.Type
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