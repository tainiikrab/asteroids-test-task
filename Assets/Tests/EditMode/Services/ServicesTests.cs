namespace AsteroidsGame.Tests.EditMode.Services
{
    using AsteroidsGame.Contracts;
    using AsteroidsGame.Logic;
    using Leopotam.EcsProto;
    using NUnit.Framework;

    public class ServicesTests
    {
        [Test]
        public void SequentialIdGeneratorService_ReturnsSequentialIds()
        {
            var service = new SequentialIdGeneratorService();

            Assert.That(service.GetNextId(), Is.EqualTo(0));
            Assert.That(service.GetNextId(), Is.EqualTo(1));
            Assert.That(service.GetNextId(), Is.EqualTo(2));
        }

        [Test]
        public void InputService_ReturnsLastSetInput()
        {
            var service = new InputService();
            var input = new InputData
            {
                forward = 1f,
                turn = -0.5f,
                shootBullet = true,
                shootLaser = false
            };

            service.SetInput(input);
            var result = service.GetInput();

            Assert.That(result.forward, Is.EqualTo(1f));
            Assert.That(result.turn, Is.EqualTo(-0.5f));
            Assert.That(result.shootBullet, Is.True);
            Assert.That(result.shootLaser, Is.False);
        }

        [Test]
        public void DeltaTimeService_StoresLastSetValue()
        {
            var service = new DeltaTimeService();

            service.SetDeltaTime(0.25f);
            Assert.That(service.DeltaTime, Is.EqualTo(0.25f));

            service.SetDeltaTime(0.016f);
            Assert.That(service.DeltaTime, Is.EqualTo(0.016f));
        }

        [Test]
        public void GameViewSizeService_ComputesHalfExtents()
        {
            var service = new GameViewSizeService();

            service.SetSize(24f, 10f);

            Assert.That(service.Width, Is.EqualTo(24f));
            Assert.That(service.Height, Is.EqualTo(10f));
            Assert.That(service.HalfWidth, Is.EqualTo(12f));
            Assert.That(service.HalfHeight, Is.EqualTo(5f));
        }

        [Test]
        public void RandomService_ProducesValuesInExpectedRanges()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            var systems = new ProtoSystems(world);

            var config = new TestConfigService();
            systems
                .AddService(config, typeof(IConfigService))
                .AddService(new GameViewSizeService(), typeof(IGameViewSizeService));

            var service = new RandomService(systems);

            for (var i = 0; i < 200; i++)
            {
                Assert.That(service.RandomSign, Is.EqualTo(-1f).Or.EqualTo(1f));
                Assert.That(service.RandomNormalizedFloat, Is.InRange(-1f, 1f));
                Assert.That(service.NextFloat, Is.InRange(0f, 1f));
                Assert.That(service.RandomAngleRad, Is.InRange(0f, 2f * System.MathF.PI));
            }

            world.Destroy();
        }

        [Test]
        public void RandomService_CalculateRandomSpawnPosition_SpawnsOnViewportBorderWithMargin()
        {
            var root = new RootAspect();
            var world = new ProtoWorld(root);
            var systems = new ProtoSystems(world);

            var config = new TestConfigService();
            ((TestWorldConfig)config.WorldConfig).ScreenWrapMargin = 2f;

            var viewSize = new GameViewSizeService();
            viewSize.SetSize(20f, 12f);

            systems
                .AddService(config, typeof(IConfigService))
                .AddService(viewSize, typeof(IGameViewSizeService));

            var service = new RandomService(systems);
            var expectedX = viewSize.HalfWidth + config.WorldConfig.ScreenWrapMargin;
            var expectedY = viewSize.HalfHeight + config.WorldConfig.ScreenWrapMargin;

            for (var i = 0; i < 500; i++)
            {
                var (x, y) = service.CalculateRandomSpawnPosition();

                var onVerticalEdge = System.MathF.Abs(System.MathF.Abs(x) - expectedX) < 0.001f;
                var onHorizontalEdge = System.MathF.Abs(System.MathF.Abs(y) - expectedY) < 0.001f;

                Assert.That(onVerticalEdge || onHorizontalEdge, Is.True);
                Assert.That(x, Is.InRange(-expectedX, expectedX));
                Assert.That(y, Is.InRange(-expectedY, expectedY));
            }

            world.Destroy();
        }
    }
}
