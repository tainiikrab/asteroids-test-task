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
        private ProtoIt _playerIterator;
        
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

            _playerIterator = new ProtoIt(new[]
            {
                typeof(VelocityCmp),
                typeof(RotationCmp),
                typeof(AngularVelocityCmp),
                typeof(PlayerCmp),
                typeof(EntityIdCmp)
            });
            _playerIterator.Init(_world);

            foreach (var e in _playerIterator)
            {
                ref var entityId = ref _entityAspect.EntityIdPool.Get(e);
                entityId.type = EntityType.Player;
            }
        }

        public void Run()
        {
            _currentInput = _inputService.GetInput();
            
            foreach (var playerEntity in _playerIterator)
            {
                ref var v = ref _transformAspect.VelocityPool.Get(playerEntity);
                ref var rot = ref _transformAspect.RotationPool.Get(playerEntity);
                ref var ang = ref _transformAspect.AngularVelocityPool.Get(playerEntity);

                var rotationSpeed = _configService.PlayerConfig.RotationSpeed;
                var acceleration = _configService.PlayerConfig.Acceleration;
                var maxSpeed = _configService.PlayerConfig.Speed;

                ang.omega = _currentInput.turn * rotationSpeed;

                var currentAcceleration = _currentInput.forward * acceleration;
                var rad = rot.angle * (Math.PI / 180.0);
                var dirX = (float)Math.Cos(rad);
                var dirY = (float)Math.Sin(rad);

                v.x += dirX * currentAcceleration * DeltaTime;
                v.y += dirY * currentAcceleration * DeltaTime;

                //clamp
                var speedSq = v.x * v.x + v.y * v.y;
                var maxSpeedSq = maxSpeed * maxSpeed;

                if (speedSq > maxSpeedSq)
                {
                    var invLength = 1.0f / (float)Math.Sqrt(speedSq);
                    v.x = v.x * invLength * maxSpeed;
                    v.y = v.y * invLength * maxSpeed;
                }

                if (_currentInput.shootBullet)
                {
                    ref var playerComponent = ref _entityAspect.PlayerPool.Get(playerEntity);
                    playerComponent.isShootingBullet = true;
                }
                else if (_currentInput.shootLaser)
                {
                    ref var playerComponent = ref _entityAspect.PlayerPool.Get(playerEntity);
                    playerComponent.isShootingLaser = true;
                }
            }
        }
    }
}