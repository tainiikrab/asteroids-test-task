
namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;

    public class EcsBootstrap : IEcsBootstrap
    {
        private readonly IConfigService _configService;
        private readonly IDeltaTimeService _deltaTimeService;
        private readonly IInputService _inputService;
        
        
        public ProtoWorld World { get; private set; }
        public IProtoSystems Systems { get; private set; }

        public EcsBootstrap(IDeltaTimeService deltaTimeService, IConfigService configService, IInputService inputService)
        {
            _deltaTimeService = deltaTimeService;
            _configService = configService;
            _inputService = inputService;
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

            // init
            Systems
                .AddService(idGeneratorService, typeof(IIdGeneratorService))
                .AddService(_deltaTimeService, typeof(IDeltaTimeService))
                .AddService(_configService, typeof(IConfigService))
                .AddService(_inputService, typeof(IInputService))
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