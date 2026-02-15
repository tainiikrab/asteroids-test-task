namespace AsteroidsGame.Logic
{
    using System;
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    public sealed class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem
    {
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private ProtoWorld _world;
        private InputData _currentInput;
        private ProtoIt _iterator;
        
        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;
        private IInputService _inputService;
        private float DeltaTime => _deltaTimeService.DeltaTime;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _inputService = svc[typeof(IInputService)] as IInputService;


            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
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
            _currentInput = _inputService.GetInput();
            
            foreach (var e in _iterator)
            {
                ref var v = ref _transformAspect.VelocityPool.Get(e);
                ref var rot = ref _transformAspect.RotationPool.Get(e);
                ref var ang = ref _transformAspect.AngularVelocityPool.Get(e);

                var rotationSpeed = _configService.PlayerConfig.RotationSpeed;
                var acceleration = _configService.PlayerConfig.Acceleration;
                var maxSpeed = _configService.PlayerConfig.Speed;

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
            }
        }
    }
}