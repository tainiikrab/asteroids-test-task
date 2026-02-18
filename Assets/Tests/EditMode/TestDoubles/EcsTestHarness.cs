namespace AsteroidsGame.Tests.EditMode
{
    using System;
    using AsteroidsGame.Contracts;
    using AsteroidsGame.Logic;
    using Leopotam.EcsProto;

    internal sealed class EcsTestHarness : IDisposable
    {
        public RootAspect RootAspect { get; }
        public ProtoWorld World { get; }
        public IProtoSystems Systems { get; }

        public TestConfigService ConfigService { get; }
        public DeltaTimeService DeltaTimeService { get; }
        public GameViewSizeService GameViewSizeService { get; }
        public InputService InputService { get; }

        public EcsTestHarness(params object[] systems)
        {
            RootAspect = new RootAspect();
            World = new ProtoWorld(RootAspect);
            Systems = new ProtoSystems(World);

            ConfigService = new TestConfigService();
            DeltaTimeService = new DeltaTimeService();
            GameViewSizeService = new GameViewSizeService();
            InputService = new InputService();

            Systems
                .AddService(ConfigService, typeof(IConfigService))
                .AddService(DeltaTimeService, typeof(IDeltaTimeService))
                .AddService(GameViewSizeService, typeof(IGameViewSizeService))
                .AddService(InputService, typeof(IInputService));

            foreach (IProtoSystem system in systems) Systems.AddSystem(system);

            Systems.Init();
        }

        public void Run(float deltaTime)
        {
            DeltaTimeService.SetDeltaTime(deltaTime);
            Systems.Run();
        }

        public void Dispose()
        {
            Systems.Destroy();
            World.Destroy();
        }
    }
}