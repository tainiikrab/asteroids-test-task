namespace AsteroidsGame.Logic
{
    public sealed class ScoreService : IScoreService
    {
        public int currentScore { get; private set; }

        public void AddScore(int score)
        {
            currentScore += score;
        }

        public void SetScore(int score)
        {
            currentScore = score;
        }
    }

    public interface IScoreService
    {
        int currentScore { get; }
        void AddScore(int score);
        void SetScore(int score);
    }
}