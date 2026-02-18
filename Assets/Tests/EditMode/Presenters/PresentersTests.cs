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
            var systems = new ProtoSystems(world);

            var scoreService = new ScoreService();
            systems.AddService(scoreService, typeof(IScoreService));

            ref var playerCmp = ref root.EntityAspect.PlayerPool.NewEntity(out var player);

            var presenter = new GameStatePresenter(systems);
            scoreService.SetScore(42);
            presenter.UpdateState();

            Assert.That(presenter.IsGameOver, Is.False);
            Assert.That(presenter.Score, Is.EqualTo(0));

            world.Destroy();
        }

        [Test]
        public void GameStatePresenter_TriggersGameOverOnceWhenNoPlayersLeft()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            var systems = new ProtoSystems(world);

            var scoreService = new ScoreService();
            systems.AddService(scoreService, typeof(IScoreService));
            scoreService.SetScore(42);

            ref var playerCmp = ref root.EntityAspect.PlayerPool.NewEntity(out var player);

            var presenter = new GameStatePresenter(systems);

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
            Assert.That(scoreFromEvent, Is.EqualTo(42));

            world.Destroy();
        }

        [Test]
        public void EcsShipUiPresenter_RendersPlayerData()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);

            ref var playerCmp = ref root.EntityAspect.PlayerPool.NewEntity(out var playerEntity);
            ref var laserShooterCmp = ref root.EntityAspect.LaserShooterPool.Add(playerEntity);
            ref var pos = ref root.TransformAspect.PositionPool.Add(playerEntity);
            ref var rot = ref root.TransformAspect.RotationPool.Add(playerEntity);
            ref var vel = ref root.TransformAspect.VelocityPool.Add(playerEntity);
            ref var health = ref root.EntityAspect.HealthPool.Add(playerEntity);

            laserShooterCmp.laserCount = 2;
            health.current = 47;
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
            Assert.That(view.LastData.health, Is.EqualTo(47));
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