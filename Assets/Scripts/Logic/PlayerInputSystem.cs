using System;
using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem
    {
        private GameAspect _aspect;
        private ProtoWorld _world;
        private InputData _currentInput;
        private ProtoIt _iterator;

        private const float MoveSpeed = 5f;
        private const float TurnSpeed = 180f;

        public void SetInput(InputData input) => _currentInput = input;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (GameAspect)_world.Aspect(typeof(GameAspect));
            _iterator = new ProtoIt(new[]
            {
                typeof(VelocityData),
                typeof(RotationData),
                typeof(AngularVelocityData)
            });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var v = ref _aspect.VelocityPool.Get(e);
                ref var rot = ref _aspect.RotationPool.Get(e);
                ref var ang = ref _aspect.AngularVelocityPool.Get(e);

                // Set angular velocity from horizontal input (turn)
                ang.omega = _currentInput.turn * TurnSpeed;

                // Forward thrust along the ship's facing direction
                var speed = _currentInput.forward * MoveSpeed;
                var rad = rot.angle * (Math.PI / 180.0);
                var dirX = (float)Math.Cos(rad);
                var dirY = (float)Math.Sin(rad);

                v.vx = dirX * speed;
                v.vy = dirY * speed;
            }
        }
    }
}