namespace Snake
{
    // Represents food that the snake can eat to grow and gain points
    class Food
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private Random _random;

        public Food(Grid grid, Random random)
        {
            _random = random;
            Spawn(grid);
        }

        // Spawns food in a random empty cell on the grid
        public void Spawn(Grid grid)
        {
            // Keep trying until we find an empty cell
            do
            {
                X = _random.Next(0, grid.Width);
                Y = _random.Next(0, grid.Height);
            } while (grid.GetCell(X, Y) != 0);
        }
    }
}
