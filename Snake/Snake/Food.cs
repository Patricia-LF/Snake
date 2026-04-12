namespace Snake
{
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
