namespace AsteroidsGame.Logic
{
    using Contracts;
    using Leopotam.EcsProto.QoL;

    public struct PositionCmp
    {
        public float x, y;
    }

    public struct VelocityCmp
    {
        public float x, y;
        public float deceleration;
    }

    public struct RotationCmp
    {
        public float angle;
    }

    public struct AngularVelocityCmp
    {
        public float omega;
    }

    public struct PlayerCmp
    {
        public bool isShootingBullet;
        public bool isShootingLaser;

        public float bulletIntervalTime;
        public float laserIntervalTime;
    }

    public struct TeleportCounterCmp
    {
        public int teleportationCount;
        public int teleportationLimit;
    }

    public struct EntityIdCmp
    {
        public int id;
        public ProtoPackedEntity packedEntity;
        public EntityType type;
    }

    public struct ColliderCmp
    {
        public float radius;
    }

    public struct CollisionSensorCmp
    {
    }

    public struct CollisionEventCmp
    {
        public ProtoPackedEntity SensorEntity;
        public ProtoPackedEntity OtherEntity;
    }

    public struct DestroyTagCmp
    {
    }

    public struct BulletCmp
    {
        public ProtoPackedEntity owner;
    }

    public struct AsteroidCmp
    {
        public bool isFragment;
    }
}