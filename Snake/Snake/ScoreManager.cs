namespace Snake
{
    class ScoreManager
    {
        public int Score { get; private set; }
        private int _multiplier = 1;
        private int _ticksAlive = 0;

        public void OnFoodEaten()
        {
            Score += 10 * _multiplier;
        }

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
    }
}
