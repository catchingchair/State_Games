public abstract class State
{
    protected GameContext _context;

    public void SetContext(GameContext context)
    {
        _context = context;
    }

    public abstract void Play();
}