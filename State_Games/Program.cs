var context = new GameContext(new List<State>
{
    new TwentyFortyEightState(),
    new JeopardyState(),
    new WordleState()
});

while (!context.IsFinished())
{
    context.Play();
}