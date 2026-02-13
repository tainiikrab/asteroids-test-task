namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;

    public class InputService : IInputControllerService
    {
        private InputData _currentInput;
        
        
        public InputData GetInput()
        {
            return _currentInput;
        }
        public void SetInput(InputData input)
        {
            _currentInput = input;
        }

    }
    public interface IInputControllerService : IInputService
    {
        void SetInput(InputData input);
    }
    public interface IInputService
    {
        InputData GetInput();
    }
}