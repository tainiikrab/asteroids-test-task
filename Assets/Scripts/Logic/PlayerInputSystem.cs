using System;
using System.Collections.Generic;
using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem
    {
        private readonly List<InputData> _buffer = new();
        private GameAspect _aspect;
        private ProtoWorld _world;

        private const float MoveSpeed = 5f;
        private const float TurnSpeed = 180f;

        public void AddInput(InputData input)
        {
            _buffer.Add(input);
        }

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (GameAspect)_world.Aspect(typeof(GameAspect));
        }

        public void Run()
        {
            InputData input = default;
            if (_buffer.Count > 0) input = _buffer[_buffer.Count - 1];
            _buffer.Clear();

            var it = new ProtoIt(new[]
            {
                typeof(VelocityData),
                typeof(RotationData),
                typeof(AngularVelocityData)
            });

            it.Init(_world);

            foreach (var e in it)
            {
                ref var v = ref _aspect.VelocityPool.Get(e);
                ref var rot = ref _aspect.RotationPool.Get(e);
                ref var ang = ref _aspect.AngularVelocityPool.Get(e);

                // Set angular velocity from horizontal input (turn)
                ang.omega = input.turn * TurnSpeed;

                // Forward thrust along the ship's facing direction
                var speed = input.forward * MoveSpeed;
                var rad = rot.angle * (Math.PI / 180.0);
                var dirX = (float)Math.Cos(rad);
                var dirY = (float)Math.Sin(rad);

                v.vx = dirX * speed;
                v.vy = dirY * speed;
            }
        }
    }
}