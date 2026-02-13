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
    public interface IInputReader
    {
        InputData ReadInput();
        void Enable();
        void Disable();
    }
    
}