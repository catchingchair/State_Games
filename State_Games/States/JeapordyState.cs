public class JeopardyState : State
{
    public override void Play()
    {
        // implement Jeopardy game logic

        bool playerWon = false;

        if (playerWon)
        {
            Console.WriteLine("You won Jeopardy! Moving on...");
            _context.TransitionToNext();
        }
    }
}