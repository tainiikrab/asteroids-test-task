namespace AsteroidsGame.Contracts
{
    public struct InputData
    {
        public float forward;
        public float turn;
        public bool shootLaser;
        public bool shootBullet;
    }

    public struct PositionData
    {
        public float x, y;
    }

    public struct VelocityData
    {
        public float vx, vy;
    }

    public struct RotationData
    {
        public float angle;
    }

    public struct AngularVelocityData
    {
        public float omega;
    }

    public struct ViewData
    {
        public int id;
        public float x, y;
        public float angle;
        public EntityType type;
    }

    public enum EntityType
    {
        Player,
        Asteroid,
        Saucer,
        Bullet,
        Laser
    }

    public struct PlayerData
    {
        public int id;
    }
}