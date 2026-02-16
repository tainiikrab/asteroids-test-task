namespace AsteroidsGame.Contracts
{
    public struct InputData
    {
        public float forward;
        public float turn;

        public bool shootBullet;
        public bool shootLaser;
    }

    public enum EntityType
    {
        Player,
        Asteroid,
        AsteroidFragment,
        Saucer,
        Bullet,
        Laser
    }

    public interface IInputReader
    {
        InputData ReadInput();
        void Enable();
        void Disable();
    }
}