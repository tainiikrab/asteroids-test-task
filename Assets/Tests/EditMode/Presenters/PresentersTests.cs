namespace AsteroidsGame.Tests.EditMode.Presenters
{
    using AsteroidsGame.Contracts;
    using AsteroidsGame.Logic;
    using Leopotam.EcsProto;
    using NUnit.Framework;

    public class PresentersTests
    {
        [Test]
        public void GameStatePresenter_StaysRunningWhilePlayerExists()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            ref var playerCmp = ref root.EntityAspect.PlayerPool.NewEntity(out var player);

            var presenter = new GameStatePresenter(world);
            presenter.SetScore(42);
            presenter.UpdateState();

            Assert.That(presenter.IsGameOver, Is.False);
            Assert.That(presenter.Score, Is.EqualTo(42));

            world.Destroy();
        }

        [Test]
        public void GameStatePresenter_TriggersGameOverOnceWhenNoPlayersLeft()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            ref var playerCmp = ref root.EntityAspect.PlayerPool.NewEntity(out var player);

            var presenter = new GameStatePresenter(world);
            presenter.SetScore(7);

            var eventsCount = 0;
            var scoreFromEvent = -1;
            presenter.OnGameOverEvent += score =>
            {
                eventsCount++;
                scoreFromEvent = score;
            };

            world.DelEntity(player);
            presenter.UpdateState();
            presenter.UpdateState();

            Assert.That(presenter.IsGameOver, Is.True);
            Assert.That(eventsCount, Is.EqualTo(1));
            Assert.That(scoreFromEvent, Is.EqualTo(7));

            world.Destroy();
        }

        [Test]
        public void EcsShipUiPresenter_RendersPlayerData()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);

            ref var laserShooterCmp = ref root.EntityAspect.LaserShooterPool.NewEntity(out var laserShooter);
            ref var pos = ref root.TransformAspect.PositionPool.Add(laserShooter);
            ref var rot = ref root.TransformAspect.RotationPool.Add(laserShooter);
            ref var vel = ref root.TransformAspect.VelocityPool.Add(laserShooter);

            laserShooterCmp.laserCount = 2;
            laserShooterCmp.laserReloadTimer = 1.5f;
            pos.x = 3f;
            pos.y = -4f;
            rot.angle = -450f;
            vel.x = 3f;
            vel.y = 4f;

            var view = new RecordingShipUiView();
            var presenter = new EcsShipUiPresenter(world, view, 2f);
            presenter.UpdateUI();

            Assert.That(view.RenderCalls, Is.EqualTo(1));
            Assert.That(view.LastData.health, Is.EqualTo(1));
            Assert.That(view.LastData.x, Is.EqualTo(3f));
            Assert.That(view.LastData.y, Is.EqualTo(-4f));
            Assert.That(view.LastData.angle, Is.EqualTo(90f).Within(0.001f));
            Assert.That(view.LastData.speed, Is.EqualTo(5f).Within(0.001f));
            Assert.That(view.LastData.laserCharges, Is.EqualTo(2));
            Assert.That(view.LastData.laserCooldown, Is.EqualTo(0.5f).Within(0.001f));

            world.Destroy();
        }

        [Test]
        public void EcsShipUiPresenter_RendersDefaultWhenNoPlayerExists()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            var view = new RecordingShipUiView();
            var presenter = new EcsShipUiPresenter(world, view, 2f);

            presenter.UpdateUI();

            Assert.That(view.RenderCalls, Is.EqualTo(1));
            Assert.That(view.LastData.health, Is.EqualTo(0));
            Assert.That(view.LastData.laserCharges, Is.EqualTo(0));
            Assert.That(view.LastData.speed, Is.EqualTo(0f));

            world.Destroy();
        }
    }

    internal sealed class RecordingShipUiView : IShipUiView
    {
        public int RenderCalls { get; private set; }
        public ShipUiData LastData { get; private set; }

        public void RenderUI(in ShipUiData data)
        {
            RenderCalls++;
            LastData = data;
        }
    }
}