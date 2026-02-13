namespace AsteroidsGame.Logic
{
    public class UnityDeltaTimeService : IDeltaTimeService
    {
        public float DeltaTime { get; private set; }

        public void SetDeltaTime(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }

    public interface IDeltaTimeService
    {
        public float DeltaTime { get; }
    }
}