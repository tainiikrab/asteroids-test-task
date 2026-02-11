using System;
using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using System.Diagnostics;

namespace AsteroidsGame.Logic
{
    public sealed class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem, IDeltaTimeUser
    {
        private PositionAspect _positionAspect;
        private EntityAspect _entityAspect;
        private ProtoWorld _world;
        private InputData _currentInput;
        private ProtoIt _iterator;

        private float _maxSpeed = 6f;
        private float _turnSpeed = 180f;
        private float _acceleration = 10f;

        public float DeltaTime { get; set; }

        public void SetInput(InputData input)
        {
            _currentInput = input;
        }

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _positionAspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _iterator = new ProtoIt(new[]
            {
                typeof(VelocityData),
                typeof(RotationData),
                typeof(AngularVelocityData),
                typeof(PlayerData),
                typeof(EntityIdComponent)
            });
            _iterator.Init(_world);

            foreach (var e in _iterator)
            {
                ref var entityId = ref _entityAspect.EntityIdPool.Get(e);
                entityId.Type = EntityType.Player;
            }
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var v = ref _positionAspect.VelocityPool.Get(e);
                ref var rot = ref _positionAspect.RotationPool.Get(e);
                ref var ang = ref _positionAspect.AngularVelocityPool.Get(e);

                ang.omega = _currentInput.turn * _turnSpeed;

                var currentAcceleration = _currentInput.forward * _acceleration;
                var rad = rot.angle * (Math.PI / 180.0);
                var dirX = (float)Math.Cos(rad);
                var dirY = (float)Math.Sin(rad);

                v.vx += dirX * currentAcceleration * DeltaTime;
                v.vy += dirY * currentAcceleration * DeltaTime;

                //clamp
                var speedSq = v.vx * v.vx + v.vy * v.vy;
                var maxSpeedSq = _maxSpeed * _maxSpeed;

                if (speedSq > maxSpeedSq)
                {
                    var invLength = 1.0f / (float)Math.Sqrt(speedSq);
                    v.vx = v.vx * invLength * _maxSpeed;
                    v.vy = v.vy * invLength * _maxSpeed;
                }
            }
        }
    }
}