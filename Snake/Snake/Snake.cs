namespace Snake
{
    class Snake
    {
        private Queue<(int x, int y)> _body;
        public (int x, int y) Head => _body.Last();
        public int Length => _body.Count;

        private int _directionX;
        private int _directionY;

        public Snake(int startX, int startY)
        {
            _body = new Queue<(int x, int y)>();
            _body.Enqueue((startX, startY));

            // Starts moving to the right
            _directionX = 1;
            _directionY = 0;
        }

        public void ChangeDirection(int dx, int dy)
        {
            // Prevents the snake to turn right back
            if (dx == -_directionX && dy == -_directionY) return;

            _directionX = dx;
            _directionY = dy;
        }

        public (int x, int y) GetNextPosition()
        {
            return (Head.x + _directionX, Head.y + _directionY);
        }

        public void Move(bool grow)
        {
            var next = GetNextPosition();
            _body.Enqueue(next);

            if (!grow)
                _body.Dequeue(); // Remove tail if the snake doesn't grow
        }

        public bool CollidesWithSelf()
        {
            var body = _body.ToList();
            var head = body.Last();

            for (int i = 0; i < body.Count - 1; i++)
            {
                if (body[i] == head)
                    return true;
            }
            return false;
        }

        public IEnumerable<(int x, int y)> GetBody()
        {
            return _body;
        }

        public void Shrink(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_body.Count > 1)
                    _body.Dequeue();
            }
        }
    }
}
