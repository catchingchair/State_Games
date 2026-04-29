public class GameContext
{
    private State _state;
    private readonly List<State> _states;
    private int _currentIndex = 0;

    public GameContext(List<State> states)
    {
        _states = states;
        TransitionTo(_states[0]);
    }

    public void TransitionTo(State state)
    {
        _state = state;
        _state.SetContext(this);
    }

    public void TransitionToNext()
    {
        _currentIndex++;
        if (_currentIndex < _states.Count)
            TransitionTo(_states[_currentIndex]);
        else
            Console.WriteLine("All games have been completed");
    }

    public void Play() => _state.Play();

    public bool IsFinished() => _currentIndex >= _states.Count;
}