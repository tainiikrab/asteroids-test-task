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
        private readonly IGameViewSizeControllerService _gameViewSizeService;


        public EcsRunner(IProtoSystems systems, IInputReader inputReader)
        {
            _systems = systems;
            _inputReader = inputReader;
            var svc = systems.Services();
            
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeControllerService;
            
            _inputService = svc[typeof(IInputService)] as IInputControllerService;
            
            _gameViewSizeService = svc[typeof(IGameViewSizeService)] as IGameViewSizeControllerService;

            
            // _deltaTimeService = deltaTimeService;
            // _inputReader = inputReader;
            // _inputService = inputService;
            // _gameViewSizeService = gameViewSizeService;
        }
        
        public void Tick(float deltaTime)
        {
            _deltaTimeService.SetDeltaTime(deltaTime);
            var input = _inputReader.ReadInput();
            _inputService.SetInput(input);
            
            _systems.Run();
        }
        public void UpdateScreenSize(float width, float height)
        {
            _gameViewSizeService.SetSize(width, height);
        }
    }

    public interface IEcsRunner
    {
        void Tick(float deltaTime);
        void UpdateScreenSize(float width, float height);
    }
}