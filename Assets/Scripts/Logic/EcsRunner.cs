namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    public class EcsRunner : IEcsRunner
    {
        private readonly IProtoSystems _systems;
        private readonly IDeltaTimeControllerService _deltaTimeService;
        private readonly IInputReader _inputReader;
        private readonly IInputControllerService _inputService;


        public EcsRunner(IProtoSystems systems, IDeltaTimeControllerService deltaTimeService, IInputReader inputReader, IInputControllerService inputService)
        {
            _systems = systems;
            _deltaTimeService = deltaTimeService;
            _inputReader = inputReader;
            _inputService = inputService;
        }
        
        public void Tick(float deltaTime)
        {
            _deltaTimeService.SetDeltaTime(deltaTime);
            var input = _inputReader.ReadInput();
            _inputService.SetInput(input);
            
            _systems.Run();
        }
    }

    public interface IEcsRunner
    {
        void Tick(float deltaTime);
    }
}