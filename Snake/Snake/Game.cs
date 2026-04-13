namespace Snake
{
    /* Main game controller.
     Responsible for game loop, input handling, updates, and rendering.*/
    class Game
    {
        private Random _random;
        private Grid _grid;
        private Snake _snake;
        private Food _food;
        private bool _isRunning;
        private int _tickRate = 150; // Time (ms) between each game update (controls game speed)
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

        //Start screen with info about controls and how to play the game
        private void ShowStartScreen()
        {
            Console.Clear();
            Console.WriteLine("== SNAKE ==");
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("  Arrow keys – move the snake");
            Console.WriteLine("  Escape     – quit the game");
            Console.WriteLine();
            Console.WriteLine("Power-ups:");
            Console.WriteLine("  ? = Double Points – doubles your score multiplier");
            Console.WriteLine("  ? = Shrink        – cuts the snake's body in half");
            Console.WriteLine("  ? = Ghost         – pass through walls temporarily");
            Console.WriteLine();
            Console.WriteLine("Obstacles (#) appear over time – watch out!");
            Console.WriteLine();
            Console.WriteLine("Press any key to start...");
            Console.ReadKey(intercept: true);
        }

        // Starts the game loop and keeps running until the game ends
        public void Run()
        {
            Console.CursorVisible = false;
            ShowStartScreen();
            Console.Clear();

            while (_isRunning)
            {
                HandleInput();
                Update();
                Render();
                Thread.Sleep(_tickRate);
            }

            GameOver();
        }

        // Handles player input without blocking the game loop
        // Uses non-blocking key reading to allow continuous movement
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

        /*Updates game state for each tick:
         - Moves the snake
         - Checks collisions (walls, obstacles, self)
         - Handles food and power-up interactions
         - Updates score and difficulty*/
        private void Update()
        {
            var next = _snake.GetNextPosition();
            bool ghostActive = _powerUp.IsGhostActive();

            // Check wall collision
            if (!ghostActive && !_grid.IsWithinBounds(next.x, next.y))
            {
                _isRunning = false;
                return;
            }

            // Wrap around screen if ghost mode is active (snake exits one side and reappears on the other)
            if (ghostActive)
            {
                next.x = (next.x + _grid.Width) % _grid.Width;
                next.y = (next.y + _grid.Height) % _grid.Height;
            }

            // Check obstacle collision
            if (!ghostActive && _obstacles.CollidesAt(next.x, next.y))
            {
                _isRunning = false;
                return;
            }

            // Check if snake collects a power-up
            // If collected, apply its effect immediately
            if (_powerUp.IsActive && next.x == _powerUp.X && next.y == _powerUp.Y)
            {
                _powerUp.Collect();
                ApplyPowerUp();
            }

            // Check if snake eats food and should grow
            bool eats = next.x == _food.X && next.y == _food.Y;
            _snake.Move(grow: eats, overridePosition: next);

            if (eats)
            {
                _score.OnFoodEaten();
                _food.Spawn(_grid);
            }

            // End game if snake collides with itself
            if (_snake.CollidesWithSelf())
            {
                _isRunning = false;
                return;
            }

            _powerUp.Update(_grid);
            _obstacles.Update(_grid);
            _score.OnTick();

            if (!_powerUp.EffectIsActive())
                _score.ResetMultiplier();
        }

        /* Renders the current game state to the console:
         - Snake
         - Food
         - Obstacles
         - Power-ups
         - Score and active effects*/
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

        // Applies the effect of the currently collected power-up
        private void ApplyPowerUp()
        {
            switch (_powerUp.Type)
            {
                case PowerUpType.DoublePoints:
                    _score.SetMultiplier(2);
                    break;
                case PowerUpType.Shrink:
                    // Shrinks the snake by removing half of its body segments
                    var body = _snake.GetBody().ToList();
                    _snake.Shrink(body.Count / 2);
                    break;
                case PowerUpType.Ghost:
                    // Ghost mode allows passing through walls (handled in Update)
                    break;
            }
        }

        // Displays final score and highscore list
        // Highlights player's position if within top 5
        private void GameOver()
        {
            _score.SaveScore();
            List<int> highscores = _score.LoadScores();

            int place = highscores.IndexOf(_score.Score) + 1;

            Console.Clear();
            Console.WriteLine("== GAME OVER ==");
            Console.WriteLine($"Your score: {_score.Score}");

            if (place > 0 && place <= 5)
                Console.WriteLine($"You placed #{place} on the highscore list!");

            Console.WriteLine();
            Console.WriteLine("== HIGHSCORES ==");

            for (int i = 0; i < highscores.Count; i++)
            {
                string marker = (i + 1 == place) ? " <--" : "";
                Console.WriteLine($"{i + 1}. {highscores[i]}{marker}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
