using UnityEngine;
using System.Collections.Generic;
using AsteroidsGame.Contracts;
using AsteroidsGame.Logic;
using AsteroidsGame.Presentation;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Bootstrap
{
    public class CompositionRoot : MonoBehaviour
    {
        private ProtoWorld _world;
        private IProtoSystems _systems;

        private IInputReader _inputReader;
        private ViewUpdater _viewUpdater;
        private PlayerInputSystem _playerInputSystem;
        private MovementSystem _movementSystem;
        private RotationSystem _rotationSystem;

        private int nextEntityId = 1;

        private void Start()
        {
            _inputReader = FindFirstObjectByType<UnityInputReader>();
            _viewUpdater = FindFirstObjectByType<ViewUpdater>();

            _world = new ProtoWorld(new GameAspect());
            _systems = new ProtoSystems(_world);

            _playerInputSystem = new PlayerInputSystem();
            _movementSystem = new MovementSystem();
            _rotationSystem = new RotationSystem();

            _systems
                .AddSystem(_playerInputSystem)
                .AddSystem(_rotationSystem)
                .AddSystem(_movementSystem)
                .Init();

            CreateEntity();
        }

        private void CreateEntity()
        {
            var aspect = (GameAspect)_world.Aspect(typeof(GameAspect));
            ref var pos = ref aspect.PositionPool.NewEntity(out var entity);
            pos.x = -1;
            pos.y = -1;
            var packed = _world.PackEntity(entity);

            ref var idComp = ref aspect.EntityIdPool.Add(entity);
            idComp.Id = nextEntityId++;
            idComp.Packed = packed;
            ref var vel = ref aspect.VelocityPool.Add(entity);
            vel.vx = 0;
            vel.vy = 0;

            ref var rot = ref aspect.RotationPool.Add(entity);
            rot.angle = 0f;

            ref var ang = ref aspect.AngularVelocityPool.Add(entity);
            ang.omega = 0f;
        }

        private void Update()
        {
            var input = _inputReader.ReadInput();

            Debug.Log(input.forward + " " + input.turn + " " + input.shootLaser + " " + input.shootBullet);

            _playerInputSystem.AddInput(input);
            _rotationSystem.SetDeltaTime(Time.deltaTime);
            _movementSystem.SetDeltaTime(Time.deltaTime);

            _systems.Run();
            SyncView();
        }

        private void SyncView()
        {
            var viewsList = new List<ViewData>();
            var aspect = (GameAspect)_world.Aspect(typeof(GameAspect));

            var it = new ProtoIt(new[] { typeof(EntityIdComponent), typeof(PositionData), typeof(RotationData) });
            it.Init(_world);

            foreach (var e in it)
            {
                ref var idComp = ref aspect.EntityIdPool.Get(e);
                ref var p = ref aspect.PositionPool.Get(e);
                ref var rot = ref aspect.RotationPool.Get(e);
                viewsList.Add(new ViewData
                {
                    id = idComp.Id,
                    x = p.x,
                    y = p.y,
                    angle = rot.angle
                });
            }

            _viewUpdater.Apply(viewsList.ToArray());
        }

        private void OnDestroy()
        {
            _systems.Destroy();
            _world.Destroy();
        }
    }
}