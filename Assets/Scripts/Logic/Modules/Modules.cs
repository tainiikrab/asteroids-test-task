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
                .AddService(new SequentialIdGeneratorService(), typeof(IIdGeneratorService))
                .AddService(new GameViewSizeService(), typeof(IGameViewSizeService))
                .AddService(new DeltaTimeService(), typeof(IDeltaTimeService))
                .AddService(new RandomService(), typeof(IRandomService))
                .AddService(_configService, typeof(IConfigService));
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
                .AddSystem(new PlayerInputSystem())
                .AddSystem(new BulletShootSystem());
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
                .AddSystem(new MovementSystem());
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
            systems.AddSystem(new CollisionDetectionSystem());
            systems.AddSystem(new CollisionResolutionSystem());
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
            systems.AddService(new AsteroidSpawnService(systems), typeof(IAsteroidSpawnService));
            systems.AddSystem(new AsteroidSpawnSystem());
            systems.AddSystem(new AsteroidFragmentationSystem());
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
            systems.AddSystem(new TeleportCounterCleanupSystem());
            systems.AddSystem(new DestroyByTagSystem());
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