namespace AsteroidsGame.Contracts
{
    public struct InputData
    {
        public float forward;
        public float turn;
        public bool shootLaser;
        public bool shootBullet;
    }

    public enum EntityType
    {
        Player,
        Asteroid,
        Saucer,
        Bullet,
        Laser
    }

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
    }
}