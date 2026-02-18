using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Tests.EditMode.Systems
{
    using AsteroidsGame.Logic;
    using NUnit.Framework;

    public class SystemsTests
    {
        [Test]
        public void MovementSystem_AppliesVelocityAndDeceleration()
        {
            using var harness = new EcsTestHarness(new MovementSystem());
            harness.GameViewSizeService.SetSize(100f, 100f);

            ref var pos = ref harness.RootAspect.TransformAspect.PositionPool.NewEntity(out var entity);
            ref var vel = ref harness.RootAspect.TransformAspect.VelocityPool.Add(entity);

            pos.x = 0f;
            pos.y = 0f;
            vel.x = 10f;
            vel.y = -5f;
            vel.deceleration = 4f;

            harness.Run(0.5f);

            Assert.That(pos.x, Is.EqualTo(5f).Within(0.001f));
            Assert.That(pos.y, Is.EqualTo(-2.5f).Within(0.001f));
            Assert.That(vel.x, Is.EqualTo(8f).Within(0.001f));
            Assert.That(vel.y, Is.EqualTo(-3f).Within(0.001f));
        }

        [Test]
        public void MovementSystem_WrapsPositionAndIncrementsTeleportCounter()
        {
            using var harness = new EcsTestHarness(new MovementSystem());
            harness.GameViewSizeService.SetSize(10f, 10f);
            ((TestWorldConfig)harness.ConfigService.WorldConfig).ScreenWrapMargin = 1f;

            ref var pos = ref harness.RootAspect.TransformAspect.PositionPool.NewEntity(out var entity);
            ref var vel = ref harness.RootAspect.TransformAspect.VelocityPool.Add(entity);
            ref var counter = ref harness.RootAspect.TransformAspect.TeleportCounterPool.Add(entity);

            pos.x = 12f;
            vel.x = 0f;
            vel.y = 0f;
            counter.teleportationLimit = 2;
            counter.teleportationCount = 0;

            harness.Run(0.016f);

            Assert.That(pos.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(counter.teleportationCount, Is.EqualTo(1));
        }

        [Test]
        public void RotationSystem_AppliesAngularVelocity()
        {
            using var harness = new EcsTestHarness(new RotationSystem());

            ref var rotation = ref harness.RootAspect.TransformAspect.RotationPool.NewEntity(out var entity);
            ref var angularVelocity = ref harness.RootAspect.TransformAspect.AngularVelocityPool.Add(entity);

            rotation.angle = 90f;
            angularVelocity.omega = 30f;

            harness.Run(0.5f);

            Assert.That(rotation.angle, Is.EqualTo(75f).Within(0.001f));
        }

        [Test]
        public void RotationSystem_ChildWithFollowParent_UsesParentRotation()
        {
            using var harness = new EcsTestHarness(new RotationSystem());

            ref var parentRotation = ref harness.RootAspect.TransformAspect.RotationPool.NewEntity(out var parent);
            ref var parentOmega = ref harness.RootAspect.TransformAspect.AngularVelocityPool.Add(parent);

            parentRotation.angle = 30f;
            parentOmega.omega = 0f;

            ref var childRotation = ref harness.RootAspect.TransformAspect.RotationPool.NewEntity(out var child);
            ref var childOmega = ref harness.RootAspect.TransformAspect.AngularVelocityPool.Add(child);
            ref var childCmp = ref harness.RootAspect.EntityAspect.ChildPool.Add(child);

            childRotation.angle = 0f;
            childOmega.omega = 999f;
            childCmp.parent = harness.World.PackEntity(parent);
            childCmp.followsParent = true;

            harness.Run(1f);

            Assert.That(childRotation.angle, Is.EqualTo(30f).Within(0.001f));
        }

        [Test]
        public void TimerCleanupSystem_AddsDestroyTagWhenIntervalIsReached()
        {
            using var harness = new EcsTestHarness(new TimerCleanupSystem());

            ref var timer = ref harness.RootAspect.EntityAspect.TimerPool.NewEntity(out var entity);

            timer.time = 0.4f;
            timer.interval = 1f;

            harness.Run(0.6f);

            Assert.That(harness.RootAspect.EntityAspect.DestroyTagPool.Has(entity), Is.True);
        }

        [Test]
        public void TeleportCounterCleanupSystem_AddsDestroyTagOnlyAboveLimit()
        {
            using var harness = new EcsTestHarness(new TeleportCounterCleanupSystem());

            ref var keepCounter =
                ref harness.RootAspect.TransformAspect.TeleportCounterPool.NewEntity(out var keepEntity);
            keepCounter.teleportationCount = 2;
            keepCounter.teleportationLimit = 2;

            ref var destroyCounter =
                ref harness.RootAspect.TransformAspect.TeleportCounterPool.NewEntity(out var destroyEntity);
            destroyCounter.teleportationCount = 3;
            destroyCounter.teleportationLimit = 2;

            harness.Run(0.016f);

            Assert.That(harness.RootAspect.EntityAspect.DestroyTagPool.Has(keepEntity), Is.False);
            Assert.That(harness.RootAspect.EntityAspect.DestroyTagPool.Has(destroyEntity), Is.True);
        }

        [Test]
        public void DestroyByTagSystem_RemovesTaggedEntities()
        {
            using var harness = new EcsTestHarness(new DestroyByTagSystem());

            ref var taggedPos = ref harness.RootAspect.EntityAspect.DestroyTagPool.NewEntity(out var tagged);
            var taggedPacked = harness.World.PackEntity(tagged);

            ref var player = ref harness.RootAspect.EntityAspect.PlayerPool.NewEntity(out var untouched);
            var untouchedPacked = harness.World.PackEntity(untouched);

            harness.Run(0.016f);

            Assert.That(taggedPacked.TryUnpack(harness.World, out _), Is.False);
            Assert.That(untouchedPacked.TryUnpack(harness.World, out _), Is.True);
        }


        [Test]
        public void CollisionResolutionSystem_DestroysBothEntitiesForRegularCollision()
        {
            using var harness = new EcsTestHarness(new CollisionResolutionSystem());

            ref var aPos = ref harness.RootAspect.TransformAspect.PositionPool.NewEntity(out var a);
            ref var bPos = ref harness.RootAspect.TransformAspect.PositionPool.NewEntity(out var b);
            ref var collision =
                ref harness.RootAspect.CollisionAspect.CollisionEventPool.NewEntity(out var eventEntity);

            collision.SensorEntity = harness.World.PackEntity(a);
            collision.OtherEntity = harness.World.PackEntity(b);

            harness.Run(0.016f);

            Assert.That(harness.RootAspect.EntityAspect.DestroyTagPool.Has(a), Is.True);
            Assert.That(harness.RootAspect.EntityAspect.DestroyTagPool.Has(b), Is.True);
        }
    }
}