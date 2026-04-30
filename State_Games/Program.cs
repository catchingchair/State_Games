var context = new GameContext(new List<State>
{
    new TwentyFortyEightState(),
    new BlackJackState(),
    new WordleState()
});

while (!context.IsFinished())
{
    context.Play();
}