public class TwentyFortyEightState : State
{
    public override void Play()
    {
        var game = new Game();
        bool quitting = false;

        while (true)
        {
            game.Render();

            if (game.Won || game.Lost || quitting)
            {
                if (game.Won)
                    Console.WriteLine("\n     You reached 2048!");
                else if (game.Lost)
                    Console.WriteLine($"\n     No moves left. Final score: {game.Score}");
                else
                    Console.WriteLine($"\n  Quitting...    Score: {game.Score}");

                Console.WriteLine("     [R] Play again    [Any] Next game");

                var response = Console.ReadKey(intercept: true).Key;
                if (response == ConsoleKey.R)
                {
                    game = new Game();
                    quitting = false;
                    continue;
                }

                _context.TransitionToNext();
                return;
            }

            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Q) { quitting = true; continue; }

            game.HandleKey(key);
        }
    }

    //Game engine
    private class Game
    {
        private const int Size = 4;
        private const int WinTile = 2048;

        private readonly int[,] _board = new int[Size, Size];
        private readonly Random _rng = new();

        public int Score { get; private set; }
        public bool Won { get; private set; }
        public bool Lost { get; private set; }

        public Game()
        {
            SpawnTile();
            SpawnTile();
        }

        // Input
        public bool HandleKey(ConsoleKey key)
        {
            bool moved = key switch
            {
                ConsoleKey.UpArrow or ConsoleKey.W => SlideUp(),
                ConsoleKey.DownArrow or ConsoleKey.S => SlideDown(),
                ConsoleKey.LeftArrow or ConsoleKey.A => SlideLeft(),
                ConsoleKey.RightArrow or ConsoleKey.D => SlideRight(),
                _ => false
            };

            if (!moved) return false;

            SpawnTile();
            if (HasTile(WinTile)) Won = true;
            if (!CanMove()) Lost = true;
            return true;
        }

        // Rendering
        public void Render()
        {
            Console.Clear();
            Console.WriteLine($"  2048          Score: {Score,6}");
            Console.WriteLine(" ┌──────┬──────┬──────┬──────┐");

            for (int r = 0; r < Size; r++)
            {
                Console.Write(" │");
                for (int c = 0; c < Size; c++)
                {
                    int val = _board[r, c];
                    Console.ForegroundColor = TileColor(val);
                    Console.Write(val == 0 ? "      " : $"{val,6}");
                    Console.ResetColor();
                    Console.Write("│");
                }
                Console.WriteLine();

                if (r < Size - 1)
                    Console.WriteLine(" ├──────┼──────┼──────┼──────┤");
            }

            Console.WriteLine(" └──────┴──────┴──────┴──────┘");
            Console.WriteLine("\n  ← ↑ → ↓  or  W A S D  to move   Q to quit");
        }

        // Slide logic
        private bool SlideLeft() => Transform(r => MergeRow(r));
        private bool SlideRight() => Transform(r => Reverse(MergeRow(Reverse(r))));
        private bool SlideUp() => TransformTransposed(r => MergeRow(r));
        private bool SlideDown() => TransformTransposed(r => Reverse(MergeRow(Reverse(r))));

        private bool Transform(Func<int[], int[]> rowOp)
        {
            bool changed = false;
            for (int r = 0; r < Size; r++)
            {
                var original = GetRow(r);
                var merged = rowOp(original);
                if (!RowEqual(original, merged)) { SetRow(r, merged); changed = true; }
            }
            return changed;
        }

        private bool TransformTransposed(Func<int[], int[]> rowOp)
        {
            Transpose();
            bool changed = Transform(rowOp);
            Transpose();
            return changed;
        }

        private int[] MergeRow(int[] row)
        {
            var tiles = new List<int>();
            foreach (int v in row) if (v != 0) tiles.Add(v);

            for (int i = 0; i < tiles.Count - 1; i++)
            {
                if (tiles[i] == tiles[i + 1])
                {
                    tiles[i] *= 2;
                    Score += tiles[i];
                    tiles.RemoveAt(i + 1);
                }
            }

            while (tiles.Count < Size) tiles.Add(0);
            return tiles.ToArray();
        }

        // Board helpers
        private int[] GetRow(int r)
        {
            var row = new int[Size];
            for (int c = 0; c < Size; c++) row[c] = _board[r, c];
            return row;
        }

        private void SetRow(int r, int[] row)
        {
            for (int c = 0; c < Size; c++) _board[r, c] = row[c];
        }

        private void Transpose()
        {
            for (int r = 0; r < Size; r++)
                for (int c = r + 1; c < Size; c++)
                    (_board[r, c], _board[c, r]) = (_board[c, r], _board[r, c]);
        }

        private static int[] Reverse(int[] row)
        {
            var copy = (int[])row.Clone();
            Array.Reverse(copy);
            return copy;
        }

        private static bool RowEqual(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        // Spawn & game-over
        private void SpawnTile()
        {
            var empty = new List<(int r, int c)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_board[r, c] == 0) empty.Add((r, c));

            if (empty.Count == 0) return;
            var (row, col) = empty[_rng.Next(empty.Count)];
            _board[row, col] = _rng.Next(10) == 0 ? 4 : 2;
        }

        private bool HasTile(int value)
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_board[r, c] == value) return true;
            return false;
        }

        private bool CanMove()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                {
                    if (_board[r, c] == 0) return true;
                    if (c + 1 < Size && _board[r, c] == _board[r, c + 1]) return true;
                    if (r + 1 < Size && _board[r, c] == _board[r + 1, c]) return true;
                }
            return false;
        }

        // Colors
        private static ConsoleColor TileColor(int value) => value switch
        {
            2 => ConsoleColor.White,
            4 => ConsoleColor.Yellow,
            8 => ConsoleColor.DarkYellow,
            16 => ConsoleColor.Cyan,
            32 => ConsoleColor.DarkCyan,
            64 => ConsoleColor.Green,
            128 => ConsoleColor.DarkGreen,
            256 => ConsoleColor.Magenta,
            512 => ConsoleColor.DarkMagenta,
            1024 => ConsoleColor.Red,
            2048 => ConsoleColor.DarkRed,
            _ => ConsoleColor.Gray
        };
    }
}