


namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    using AsteroidsGame.Logic.Modules;

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
            var rootAspect = new RootAspect();
            World = new ProtoWorld(rootAspect);
            
            Systems = new ProtoSystems(World);

            Systems
                .AddModule(new CoreModule(_configService))
                .AddModule(new SpawnModule())
                .AddModule(new PlayerModule())
                .AddModule(new MovementModule())
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