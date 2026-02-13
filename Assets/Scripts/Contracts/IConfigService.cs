namespace AsteroidsGame.Contracts
{
    public interface IConfigService
    {
        public float PlayerSpeed { get; }
        public float PlayerRotationSpeed { get; }
        public float PlayerAcceleration { get; }
        public float PlayerDeceleration { get; }

        public float AsteroidSpeed { get; }
        public float AsteroidRotationSpeed { get; }
        public float AsteroidSpawnInterval { get; }
        public int AsteroidSpawnAmount { get; }
        public float AsteroidRandomnessWeight { get; }
        
        public float ScreenWrapMargin { get; }
        public int AsteroidTeleportationLimit { get; }
    }
}