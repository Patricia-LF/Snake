namespace Snake
{
    enum PowerUpType
    {
        DoublePoints,
        Shrink,
        Ghost
    }

    class PowerUp
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public PowerUpType Type { get; private set; }
        public bool IsActive { get; private set; }

        private Random _random;

        public PowerUp(Random random)
        {
            _random = random;
        }
        private int _ticksSinceLastSpawn = 0;
        private int _spawnInterval = 150;
        private int _effectTicksRemaining = 0;

        public void Update(Grid grid)
        {
            if (_effectTicksRemaining > 0)
                _effectTicksRemaining--;

            _ticksSinceLastSpawn++;
            if (_ticksSinceLastSpawn >= _spawnInterval && !IsActive)
            {
                Spawn(grid);
                _ticksSinceLastSpawn = 0;
            }
        }

        private void Spawn(Grid grid)
        {
            int x, y;
            do
            {
                x = _random.Next(0, grid.Width);
                y = _random.Next(0, grid.Height);
            } while (grid.GetCell(x, y) != 0);

            X = x;
            Y = y;
            IsActive = true;

            // Pick a random power-up type
            var values = Enum.GetValues(typeof(PowerUpType));
            Type = (PowerUpType)values.GetValue(_random.Next(values.Length));
        }

        public void Collect()
        {
            IsActive = false;
            _effectTicksRemaining = 100; // Effect lasts 100 ticks
        }

        public bool EffectIsActive()
        {
            return _effectTicksRemaining > 0;
        }

        public bool IsGhostActive()
        {
            return EffectIsActive() && Type == PowerUpType.Ghost;
        }
    }
}
