using System;
using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;


#if UNITY_EDITOR
using System.Diagnostics;
#endif


namespace AsteroidsGame.Logic
{
    public sealed class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem
    {
        private PositionAspect _positionAspect;
        private EntityAspect _entityAspect;
        private ProtoWorld _world;
        private InputData _currentInput;
        private ProtoIt _iterator;

        // private float _maxSpeed = 6f;
        // private float _turnSpeed = 180f;
        // private float _acceleration = 10f;

        public void SetInput(InputData input)
        {
            _currentInput = input;
        }

        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;
        private float DeltaTime => _deltaTimeService.DeltaTime;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _configService = svc[typeof(IConfigService)] as IConfigService;


            _positionAspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _iterator = new ProtoIt(new[]
            {
                typeof(VelocityCmp),
                typeof(RotationCmp),
                typeof(AngularVelocityCmp),
                typeof(PlayerCmp),
                typeof(EntityIdCmp)
            });
            _iterator.Init(_world);

            foreach (var e in _iterator)
            {
                ref var entityId = ref _entityAspect.EntityIdPool.Get(e);
                entityId.type = EntityType.Player;
            }
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var v = ref _positionAspect.VelocityPool.Get(e);
                ref var rot = ref _positionAspect.RotationPool.Get(e);
                ref var ang = ref _positionAspect.AngularVelocityPool.Get(e);

                var rotationSpeed = _configService.PlayerRotationSpeed;
                var acceleration = _configService.PlayerAcceleration;
                var maxSpeed = _configService.PlayerSpeed;

                ang.omega = _currentInput.turn * rotationSpeed;

                var currentAcceleration = _currentInput.forward * acceleration;
                var rad = rot.angle * (Math.PI / 180.0);
                var dirX = (float)Math.Cos(rad);
                var dirY = (float)Math.Sin(rad);

                v.vx += dirX * currentAcceleration * DeltaTime;
                v.vy += dirY * currentAcceleration * DeltaTime;

                //clamp
                var speedSq = v.vx * v.vx + v.vy * v.vy;
                var maxSpeedSq = maxSpeed * maxSpeed;

                if (speedSq > maxSpeedSq)
                {
                    var invLength = 1.0f / (float)Math.Sqrt(speedSq);
                    v.vx = v.vx * invLength * maxSpeed;
                    v.vy = v.vy * invLength * maxSpeed;
                }

                Debug.WriteLine($"{_currentInput.turn}, {rotationSpeed}");
            }
        }
    }
}