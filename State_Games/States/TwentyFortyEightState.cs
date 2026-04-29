public class TwentyFortyEightState : State
{
    public override void Play()
    {
        // implement 2048 game logic

        bool playerWon = false;

        if (playerWon)
        {
            Console.WriteLine("You won 2048! Starting next game...");
            _context.TransitionToNext();
        }
    }
}