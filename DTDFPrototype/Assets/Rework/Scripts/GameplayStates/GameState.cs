public abstract class GameState
{
    protected GameplayManager manager;

    public GameState(GameplayManager manager)
    {
        this.manager = manager;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}