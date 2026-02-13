namespace AsteroidsGame.Logic
{
    public class DeltaTimeService : IDeltaTimeControllerService
    {
        public float DeltaTime { get; private set; }

        public void SetDeltaTime(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
    public interface IDeltaTimeControllerService : IDeltaTimeService
    {
        public void SetDeltaTime(float deltaTime);
    }
    public interface IDeltaTimeService
    {
        public float DeltaTime { get; }
    }
}