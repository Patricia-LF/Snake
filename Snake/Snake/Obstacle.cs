namespace Snake
{
    // Manages dynamic obstacles that appear over time
    class Obstacle
    {
        private List<(int x, int y)> _obstacles = new List<(int x, int y)>();
        private Random _random;

        public Obstacle(Random random)
        {
            _random = random;
        }
        private int _ticksSinceLastSpawn = 0;
        private int _spawnInterval = 100; // Ticks between each new obstacle

        public IEnumerable<(int x, int y)> GetObstacles()
        {
            return _obstacles;
        }

        public void Update(Grid grid)
        {
            _ticksSinceLastSpawn++;

            if (_ticksSinceLastSpawn >= _spawnInterval)
            {
                Spawn(grid);
                _ticksSinceLastSpawn = 0;
                // Gradually increases difficulty by spawning obstacles more frequently
                if (_spawnInterval > 30)
                    _spawnInterval -= 5;
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

            _obstacles.Add((x, y));
        }

        public bool CollidesAt(int x, int y)
        {
            return _obstacles.Contains((x, y));
        }
    }
}
