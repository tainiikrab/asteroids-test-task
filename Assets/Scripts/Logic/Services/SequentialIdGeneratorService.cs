namespace AsteroidsGame.Logic
{
    public sealed class SequentialIdGeneratorService : IIdGeneratorService
    {
        private int _next = 0;

        public int GetNextId()
        {
            return _next++;
        }
    }
    public interface IIdGeneratorService
    {
        int GetNextId();
    }

}