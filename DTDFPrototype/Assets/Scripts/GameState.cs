public abstract class GameState
{
    protected CoreGameplayManager manager;

    public GameState(CoreGameplayManager manager)
    {
        this.manager = manager;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}