namespace Snake
{
    // Handles scoring, multipliers, and highscore persistence
    class ScoreManager
    {
        public int Score { get; private set; }
        private int _multiplier = 1;
        private int _ticksAlive = 0;
        private string _filePath = "highscores.txt";

        // Adds score when food is eaten, applying multiplier
        public void OnFoodEaten()
        {
            Score += 10 * _multiplier;
        }

        // Awards passive score over time for survival
        public void OnTick()
        {
            _ticksAlive++;
            // Add 1 point every 5 ticks for surviving
            if (_ticksAlive % 5 == 0)
                Score += 1 * _multiplier;
        }

        public void SetMultiplier(int multiplier)
        {
            _multiplier = multiplier;
        }

        public void ResetMultiplier()
        {
            _multiplier = 1;
        }

        // Saves top 5 highscores to file
        public void SaveScore()
        {
            List<int> scores = LoadScores();
            scores.Add(Score);
            scores.Sort((a, b) => b.CompareTo(a)); // Sort highest first

            // Keep only top 5
            if (scores.Count > 5)
                scores = scores.GetRange(0, 5);

            File.WriteAllLines(_filePath, scores.Select(s => s.ToString()));
        }

        public List<int> LoadScores()
        {
            if (!File.Exists(_filePath))
                return new List<int>();

            return File.ReadAllLines(_filePath)
                       .Select(line => int.Parse(line))
                       .ToList();
        }
    }
}

 
