namespace AsteroidsGame.Contracts
{
    public struct ShipUiData
    {
        public int health;
        public float x;
        public float y;
        public float angle;
        public float speed;
        public int laserCharges;
        public float laserCooldown;
    }

    public interface IShipUiView
    {
        void RenderUI(in ShipUiData data);
    }
}