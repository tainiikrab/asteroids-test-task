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
    }

    public struct BulletShooterCmp
    {
        public bool isShootingBullet;
        public float bulletReloadTimer;
    }

    public struct LaserShooterCmp
    {
        public bool isShootingLaser;
        public float laserReloadTimer;
        public int laserCount;
        public float timeSinceLaserShot;
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

    public struct CircleColliderCmp
    {
        public float radius;
    }

    public struct LaserColliderCmp
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

    public struct ChildCmp
    {
        public ProtoPackedEntity parent;
        public bool followsParent;
    }

    public struct AsteroidCmp
    {
        public bool isFragment;
    }

    public struct FollowerCmp
    {
        public ProtoPackedEntity Target;
    }

    public struct TimerCmp
    {
        public float time;
        public float interval;
    }

    public struct HealthCmp
    {
        public int current;
        public int max;
    }

    public struct ScoreCmp
    {
        public int score;
    }
}