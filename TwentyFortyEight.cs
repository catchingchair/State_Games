class TwentyFortyEight
{
    public ulong Score { get; private set; }
    public ulong[,] Board { get; private set; }

    private readonly int nRows;
    private readonly int nCols;

    public Game()
    {
        this.Board = new ulong[4, 4];
        this.nRows = 4;
        this.nCols = 4;
        this.Score = 0;
    }

    public Play()
    {
        // Spawn two blocks

        // Start game loop

        // Wait for input

        // Check win condition
        // End game if won

        // Spawn block
    }
}