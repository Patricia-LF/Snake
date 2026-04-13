namespace Snake
{
    // Represents the game board as a 2D grid
    // Each cell stores a value indicating what is rendered
    class Grid
    {
        public int Width { get; }
        public int Height { get; }
        private int[,] _cells;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new int[width, height];
        }

        public int GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        public void SetCell(int x, int y, int value)
        {
            _cells[x, y] = value;
        }

        public void Clear()
        {
            _cells = new int[Width, Height];
        }

        // Draws the grid to the console using different symbols for each entity
        public void Draw()
        {
            /* Maps cell values to visual characters:
            0 = empty, 1 = snake, 2 = food, 3 = obstacle, 4 = power-up*/
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char symbol;
                    switch (_cells[x, y])
                    {
                        case 1: symbol = 'O'; break;  // snake
                        case 2: symbol = '*'; break;  // food
                        case 3: symbol = '#'; break;  // obstacle
                        case 4: symbol = '?'; break;  // power-up
                        default: symbol = '.'; break; // empty
                    }
                    Console.Write(symbol);
                }
                Console.WriteLine();
            }
        }

        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
