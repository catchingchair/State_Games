public class TwentyFortyEightState : State
{
    // Entry point for the 2048 game state. Manages the game loop, input handling,
    // and transitions to the next state on exit.
    public override void Play()
    {
        var game = new Game();
        bool quitting = false;

        while (true)
        {
            game.Render();

            // Handle end-of-game conditions: win, loss, or quit
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
                    // Restart with a fresh board
                    game = new Game();
                    quitting = false;
                    continue;
                }

                // Any other key exits to the next game
                _context.TransitionToNext();
                return;
            }

            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Q) { quitting = true; continue; }

            game.HandleKey(key);
        }
    }

    // 2048 game engine. Manages board state, tile spawning,
    // slide/merge logic, win/loss detection, and rendering
    private class Game
    {
        private const int Size = 4;        // Board is 4×4
        private const int WinTile = 2048;  // Target tile value to win

        private readonly int[,] _board = new int[Size, Size];
        private readonly Random _rng = new();

        public int Score { get; private set; }
        public bool Won { get; private set; }
        public bool Lost { get; private set; }

        // Initializes a new board and spawns two starting tiles
        public Game()
        {
            SpawnTile();
            SpawnTile();
        }

        // Processes a keypress, sliding the board in the corresponding direction.
        // Spawns a new tile and checks win/loss conditions if the board changed.
        // True if the move caused any tiles to shift or merge.
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

        // Clears the console and redraws the board with colored tile values,
        // the current score, and the control hint line.
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
                    // Empty cells render as blank; occupied cells are right-aligned in 6 chars
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

        // Each direction is implemented by reusing MergeRow (which merges left).
        // Right = reverse the row, merge left, reverse back.
        // Up/Down = transpose the board, merge left/right, transpose back.

        private bool SlideLeft() => Transform(r => MergeRow(r));
        private bool SlideRight() => Transform(r => Reverse(MergeRow(Reverse(r))));
        private bool SlideUp() => TransformTransposed(r => MergeRow(r));
        private bool SlideDown() => TransformTransposed(r => Reverse(MergeRow(Reverse(r))));

        // <summary>
        // Applies <paramref name="rowOp"/> to every row in place.
        // </summary>
        // <returns>True if any row changed after the operation.</returns>
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

        // Transposes the board, runs rowOp on each row,
        // then transposes back, effectively operating on columns.
        private bool TransformTransposed(Func<int[], int[]> rowOp)
        {
            Transpose();
            bool changed = Transform(rowOp);
            Transpose();
            return changed;
        }

        // Slides all non-zero tiles to the left, merging adjacent equal values once
        // per merge pass, then pads the result to Size with zeros.
        // Merged tile values are added to the score.
        private int[] MergeRow(int[] row)
        {
            // Compact: strip zeros so tiles are contiguous
            var tiles = new List<int>();
            foreach (int v in row) if (v != 0) tiles.Add(v);

            // Merge adjacent equal tiles left-to-right (each tile merges at most once)
            for (int i = 0; i < tiles.Count - 1; i++)
            {
                if (tiles[i] == tiles[i + 1])
                {
                    tiles[i] *= 2;
                    Score += tiles[i];
                    tiles.RemoveAt(i + 1); // Remove the consumed tile
                }
            }

            // Pad remaining slots with zeros to maintain row length
            while (tiles.Count < Size) tiles.Add(0);
            return tiles.ToArray();
        }

        // Board helpers

        // Copies row "r" from the board into a new array.
        private int[] GetRow(int r)
        {
            var row = new int[Size];
            for (int c = 0; c < Size; c++) row[c] = _board[r, c];
            return row;
        }

        // Writes "row" back into row "r" of the board.
        private void SetRow(int r, int[] row)
        {
            for (int c = 0; c < Size; c++) _board[r, c] = row[c];
        }

        // Transposes the board in place (swaps rows and columns)
        private void Transpose()
        {
            for (int r = 0; r < Size; r++)
                for (int c = r + 1; c < Size; c++)
                    (_board[r, c], _board[c, r]) = (_board[c, r], _board[r, c]);
        }

        // Returns a reversed copy of row"
        private static int[] Reverse(int[] row)
        {
            var copy = (int[])row.Clone();
            Array.Reverse(copy);
            return copy;
        }

        // Returns true if arrays "a" and "b" are element-wise equal
        private static bool RowEqual(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        // Spawn & game-over

        // Places a new tile on a random empty cell.
        // The tile is a 4 with 10% probability, otherwise a 2.
        // Does nothing if the board is full.
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

        // Returns true if any cell on the board contains a value
        private bool HasTile(int value)
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_board[r, c] == value) return true;
            return false;
        }

        // Returns true if at least one move is still possible: either an empty
        // cell exists, or two adjacent cells (horizontal or vertical) share a value.
        private bool CanMove()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                {
                    if (_board[r, c] == 0) return true;
                    if (c + 1 < Size && _board[r, c] == _board[r, c + 1]) return true; // Horizontal neighbour
                    if (r + 1 < Size && _board[r, c] == _board[r + 1, c]) return true; // Vertical neighbour
                }
            return false;
        }

        // Colors

        // Maps a tile value to a console color..
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
            _ => ConsoleColor.Gray  // Fallback for tiles beyond 2048
        };
    }
}