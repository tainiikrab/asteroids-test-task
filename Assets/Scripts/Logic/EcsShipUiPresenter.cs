namespace AsteroidsGame.Logic
{
    using System;
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;

    public sealed class EcsShipUiPresenter : IShipUiPresenter
    {
        private readonly IShipUiView _uiView;
        private readonly float _laserInterval;

        private readonly ProtoIt _playerIterator;
        private readonly ProtoPool<PlayerCmp> _playerPool;
        private readonly ProtoPool<PositionCmp> _positionPool;
        private readonly ProtoPool<RotationCmp> _rotationPool;
        private readonly ProtoPool<VelocityCmp> _velocityPool;

        private readonly ProtoPool<LaserShooterCmp> _laserShooterPool;

        public EcsShipUiPresenter(ProtoWorld world, IShipUiView uiView, float laserInterval)
        {
            _uiView = uiView;
            _laserInterval = laserInterval;

            var entityAspect = world.Aspect(typeof(EntityAspect)) as EntityAspect;
            var transformAspect = world.Aspect(typeof(TransformAspect)) as TransformAspect;

            _playerPool = entityAspect?.PlayerPool;
            _positionPool = transformAspect?.PositionPool;
            _rotationPool = transformAspect?.RotationPool;
            _velocityPool = transformAspect?.VelocityPool;
            _laserShooterPool = entityAspect?.LaserShooterPool;

            _playerIterator = new ProtoIt(new[]
            {
                typeof(PlayerCmp), typeof(PositionCmp), typeof(RotationCmp), typeof(VelocityCmp)
            });
            _playerIterator.Init(world);
        }

        public void UpdateUI()
        {
            if (_uiView == null)
                return;

            foreach (var entity in _playerIterator)
            {
                ref var laserShooter = ref _laserShooterPool.Get(entity);
                ref var position = ref _positionPool.Get(entity);
                ref var rotation = ref _rotationPool.Get(entity);
                ref var velocity = ref _velocityPool.Get(entity);

                var speed = MathF.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
                var cooldown = _laserInterval - laserShooter.laserReloadTimer;
                if (cooldown < 0f)
                    cooldown = 0f;

                _uiView.RenderUI(new ShipUiData
                {
                    health = 1,
                    x = position.x,
                    y = position.y,
                    angle = Math.Abs(rotation.angle % 360f),
                    speed = speed,
                    laserCharges = laserShooter.laserCount,
                    laserCooldown = cooldown
                });
                return;
            }

            _uiView.RenderUI(new ShipUiData());
        }
    }

    public interface IShipUiPresenter
    {
        void UpdateUI();
    }
}