
namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;

    public class EcsBootstrap : IEcsBootstrap
    {
        private readonly IConfigService _configService;
        
        public ProtoWorld World { get; private set; }
        public IProtoSystems Systems { get; private set; }

        public EcsBootstrap(IConfigService configService)
        {
            _configService = configService;
        }

        public void Init()
        {
            // world
            var rootAspect = new RootAspect();
            World = new ProtoWorld(rootAspect); 
            
            // systems
            Systems = new ProtoSystems(World);
            var playerInputSystem = new PlayerInputSystem();
            var movementSystem = new MovementSystem();
            var rotationSystem = new RotationSystem();
            var asteroidSpawnSystem = new AsteroidSpawnSystem();
            var playerSpawnSystem = new PlayerSpawnSystem();

            // services
            var idGeneratorService = new SequentialIdGeneratorService();
            var gameViewSizeService = new GameViewSizeService();
            var inputService = new InputService();
            var deltaTimeService = new DeltaTimeService();

            // init
            Systems
                .AddService(idGeneratorService, typeof(IIdGeneratorService))
                .AddService(gameViewSizeService, typeof(IGameViewSizeService))
                .AddService(deltaTimeService, typeof(IDeltaTimeService))
                .AddService(inputService, typeof(IInputService))
                .AddService(_configService, typeof(IConfigService))
                .AddSystem(playerSpawnSystem)
                .AddSystem(asteroidSpawnSystem)
                .AddSystem(playerInputSystem)
                .AddSystem(rotationSystem)
                .AddSystem(movementSystem)
                .Init();
        }

        public void Destroy()
        {
            Systems.Destroy();
            World.Destroy();
            Systems = null;
            World = null;
        }
    }
    
    public interface IEcsBootstrap
    {
        void Init();
        void Destroy();
        ProtoWorld World { get; }
        IProtoSystems Systems { get; }
    }
    
}