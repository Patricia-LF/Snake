namespace Snake
{
    class Game
    {
        private Random _random = new Random();
        private Grid _grid;
        private Snake _snake;
        private Food _food;
        private bool _isRunning;
        private int _tickRate = 150; // Milliseconds between each update
        private ScoreManager _score;
        private Obstacle _obstacles;
        private PowerUp _powerUp;

        public Game()
        {
            _random = new Random();
            _grid = new Grid(30, 20);
            _snake = new Snake(15, 10);
            _food = new Food(_grid, _random);
            _obstacles = new Obstacle(_random);
            _powerUp = new PowerUp(_random);
            _score = new ScoreManager();
            _isRunning = true;
        }

        public void Run()
        {
            Console.CursorVisible = false;

            while (_isRunning)
            {
                HandleInput();
                Update();
                Render();
                Thread.Sleep(_tickRate);
            }

            GameOver();
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: _snake.ChangeDirection(0, -1); break;
                case ConsoleKey.DownArrow: _snake.ChangeDirection(0, 1); break;
                case ConsoleKey.LeftArrow: _snake.ChangeDirection(-1, 0); break;
                case ConsoleKey.RightArrow: _snake.ChangeDirection(1, 0); break;
                case ConsoleKey.Escape: _isRunning = false; break;
            }
        }

        private void Update()
        {
            var next = _snake.GetNextPosition();

            // Check wall collision
            if (!_grid.IsWithinBounds(next.x, next.y))
            {
                _isRunning = false;
                return;
            }

            // Check if snake eats food
            bool eats = next.x == _food.X && next.y == _food.Y;

            _snake.Move(grow: eats);

            if (eats)
                _food.Spawn(_grid);

            // Check self collision
            if (_snake.CollidesWithSelf())
            {
                _isRunning = false;
                return;
            }

            _score.OnTick();

            // When the snake eats food:
            if (eats)
            {
                _score.OnFoodEaten();
                _food.Spawn(_grid);
            }

            _obstacles.Update(_grid);

            // Check obstacle collision
            if (_obstacles.CollidesAt(next.x, next.y))
            {
                _isRunning = false;
                return;
            }

            _powerUp.Update(_grid);

            // Check if snake collects power-up
            if (_powerUp.IsActive && next.x == _powerUp.X && next.y == _powerUp.Y)
            {
                _powerUp.Collect();
                ApplyPowerUp();
            }

            // Reset multiplier when effect runs out
            if (!_powerUp.EffectIsActive())
                _score.ResetMultiplier();
        }

        private void Render()
        {
            _grid.Clear();

            foreach (var segment in _snake.GetBody())
            {
                _grid.SetCell(segment.x, segment.y, 1);
            }

            _grid.SetCell(_food.X, _food.Y, 2);

            foreach (var obs in _obstacles.GetObstacles())
            {
                _grid.SetCell(obs.x, obs.y, 3);
            }

            if (_powerUp.IsActive)
                _grid.SetCell(_powerUp.X, _powerUp.Y, 4);

            _grid.Draw();

            Console.WriteLine($"Score: {_score.Score}          ");

            if (_powerUp.EffectIsActive())
                Console.WriteLine($"Power-up: {_powerUp.Type}!          ");
            else
                Console.WriteLine("                              ");
        }

        private void ApplyPowerUp()
        {
            switch (_powerUp.Type)
            {
                case PowerUpType.DoublePoints:
                    _score.SetMultiplier(2);
                    break;
                case PowerUpType.Shrink:
                    // Remove half the snake's body
                    var body = _snake.GetBody().ToList();
                    _snake.Shrink(body.Count / 2);
                    break;
                case PowerUpType.Ghost:
                    // Handled in Update - skip wall collision check
                    break;
            }
        }

        private void GameOver()
        {
            _score.SaveScore();
            List<int> highscores = _score.LoadScores();

            Console.Clear();
            Console.WriteLine("== GAME OVER ==");
            Console.WriteLine($"Your score: {_score.Score}");
            Console.WriteLine();
            Console.WriteLine("== HIGHSCORES ==");

            for (int i = 0; i < highscores.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {highscores[i]}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
