namespace AsteroidsGame.Logic.Modules
{
    using System;
    using Contracts;
    using Leopotam.EcsProto;

    public class CoreModule : IProtoModule
    {
        private readonly IConfigService _configService;

        public CoreModule(IConfigService configService)
        {
            _configService = configService;
        }

        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(_configService, typeof(IConfigService))
                .AddService(new SequentialIdGeneratorService(), typeof(IIdGeneratorService))
                .AddService(new GameViewSizeService(), typeof(IGameViewSizeService))
                .AddService(new DeltaTimeService(), typeof(IDeltaTimeService))
                .AddService(new RandomService(systems), typeof(IRandomService));
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return null;
        }
    }

    public class PlayerModule : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new InputService(), typeof(IInputService))
                .AddSystem(new PlayerSpawnSystem())
                .AddSystem(new PlayerInputSystem());
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return new Type[] { typeof(CoreModule) };
        }
    }

    public class MovementModule : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new RotationSystem())
                .AddSystem(new MovementSystem())
                .AddSystem(new SaucerHomingSystem());
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return new Type[] { typeof(CoreModule) };
        }
    }

    public class CollisionModule : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new CollisionDetectionSystem())
                .AddSystem(new LaserCollisionSystem())
                .AddSystem(new CollisionResolutionSystem());
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return new Type[] { typeof(CoreModule) };
        }
    }

    public class SpawnModule : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddService(new ObstacleSpawnService(systems), typeof(IObstacleSpawnService))
                .AddSystem(new AsteroidSpawnSystem())
                .AddSystem(new AsteroidFragmentationSystem())
                .AddSystem(new SaucerSpawnSystem())
                .AddSystem(new BulletSpawnSystem())
                .AddSystem(new LaserSpawnSystem());
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return new Type[] { typeof(CoreModule) };
        }
    }

    public class DestroyModule : IProtoModule
    {
        public void Init(IProtoSystems systems)
        {
            systems
                .AddSystem(new TeleportCounterCleanupSystem())
                .AddSystem(new TimerCleanupSystem())
                .AddSystem(new DestroyByTagSystem());
        }

        public IProtoAspect[] Aspects()
        {
            return null;
        }

        public Type[] Dependencies()
        {
            return new Type[] { typeof(CoreModule) };
        }
    }
}