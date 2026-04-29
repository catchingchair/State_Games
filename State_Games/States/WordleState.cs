public class WordleState : State
{
    public override void Play()
    {
        // implement Wordle game logic

        bool playerWon = false;

        if (playerWon)
        {
            Console.WriteLine("You won Wordle!");
            _context.TransitionToNext();
        }
    }
}